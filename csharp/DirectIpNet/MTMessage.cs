using System;
using System.Buffers.Binary;
using System.Text;

namespace SBDDirectIP;

public class MTMessage
{
    private readonly MTHeader _header;
    private readonly MTPayload _payload;

    private MTMessage(MTHeader header, MTPayload payload)
    {
        _header = header;
        _payload = payload;
    }

    public static MTMessageBuilder Builder() => new MTMessageBuilder();

    public byte[] ToArray()
    {
        var headerBytes = _header.ToArray();
        var payloadBytes = _payload.ToArray();
        var length = (ushort)(headerBytes.Length + payloadBytes.Length);

        var buffer = new byte[3 + length];
        int offset = 0;
        buffer[offset++] = 1; // protocol version
        BinaryPrimitives.WriteUInt16BigEndian(buffer.AsSpan(offset), length);
        offset += 2;
        headerBytes.CopyTo(buffer.AsSpan(offset));
        offset += headerBytes.Length;
        payloadBytes.CopyTo(buffer.AsSpan(offset));
        return buffer;
    }

    public static MTMessage FromArray(ReadOnlySpan<byte> data)
    {
        if (data.Length < 3) throw new ArgumentException("Invalid message");
        if (data[0] != 1) throw new ArgumentException("Unsupported protocol version");
        ushort len = BinaryPrimitives.ReadUInt16BigEndian(data.Slice(1, 2));
        if (data.Length - 3 < len) throw new ArgumentException("Incomplete message");
        int offset = 3;
        // parse header
        if (data[offset] != 0x41) throw new ArgumentException("Missing header");
        offset += 1;
        ushort hLen = BinaryPrimitives.ReadUInt16BigEndian(data.Slice(offset,2));
        offset += 2;
        uint clientMsgId = BinaryPrimitives.ReadUInt32BigEndian(data.Slice(offset,4));
        offset += 4;
        string imei = System.Text.Encoding.ASCII.GetString(data.Slice(offset,15));
        offset += 15;
        ushort flags = BinaryPrimitives.ReadUInt16BigEndian(data.Slice(offset,2));
        offset += 2;
        var header = new MTHeader(clientMsgId, imei, flags);
        // parse payload
        if (data[offset] != 0x42) throw new ArgumentException("Missing payload");
        offset += 1;
        ushort pLen = BinaryPrimitives.ReadUInt16BigEndian(data.Slice(offset,2));
        offset += 2;
        var payload = new byte[pLen];
        data.Slice(offset, pLen).CopyTo(payload);
        var pl = new MTPayload(payload);
        return new MTMessage(header, pl);
    }

    public class MTMessageBuilder
    {
        private uint _clientMsgId;
        private string? _imei;
        private byte[] _payload = Array.Empty<byte>();
        private ushort _flags = 0;

        public MTMessageBuilder ClientMsgId(uint id) { _clientMsgId = id; return this; }
        public MTMessageBuilder Imei(string imei) { _imei = imei; return this; }
        public MTMessageBuilder Payload(byte[] data) { _payload = data ?? Array.Empty<byte>(); return this; }
        public MTMessageBuilder DispositionFlags(ushort flags) { _flags = flags; return this; }

        public MTMessage Build()
        {
            if (_imei == null) throw new InvalidOperationException("IMEI missing");
            var header = new MTHeader(_clientMsgId, _imei, _flags);
            var payload = new MTPayload(_payload);
            return new MTMessage(header, payload);
        }
    }
}

/// <summary>
/// Status of a confirmation message returned by the gateway.
/// Mirrors the enum from the original Rust implementation.
/// </summary>
public enum MessageStatus
{
    SuccessfulQueueOrder = 0,
    InvalidImei = -1,
    UnknownImei = -2,
    PayloadOversized = -3,
    PayloadMissing = -4,
    MtQueueFull = -5,
    MtResourcesUnavailable = -6,
    ProtocolViolation = -7,
    RingAlertsDisabled = -8,
    SsdNotAttached = -9,
    SourceAddressRejected = -10,
    MtmsnOutOfRange = -11,
    CertificateRejected = -12
}

internal class MTHeader
{
    public uint ClientMsgId { get; }
    public string Imei { get; }
    public ushort DispositionFlags { get; }

    public MTHeader(uint clientMsgId, string imei, ushort dispositionFlags)
    {
        if (imei.Length != 15) throw new ArgumentException("IMEI must be 15 digits", nameof(imei));
        ClientMsgId = clientMsgId;
        Imei = imei;
        DispositionFlags = dispositionFlags;
    }

    public byte[] ToArray()
    {
        var buffer = new byte[24];
        int offset = 0;
        buffer[offset++] = 0x41;
        BinaryPrimitives.WriteUInt16BigEndian(buffer.AsSpan(offset), 21);
        offset += 2;
        BinaryPrimitives.WriteUInt32BigEndian(buffer.AsSpan(offset), ClientMsgId);
        offset += 4;
        Encoding.ASCII.GetBytes(Imei).CopyTo(buffer.AsSpan(offset));
        offset += 15;
        BinaryPrimitives.WriteUInt16BigEndian(buffer.AsSpan(offset), DispositionFlags);
        return buffer;
    }
}

internal class MTPayload
{
    public byte[] Data { get; }
    public MTPayload(byte[] data)
    {
        Data = data ?? Array.Empty<byte>();
    }

    public byte[] ToArray()
    {
        var buffer = new byte[3 + Data.Length];
        int offset = 0;
        buffer[offset++] = 0x42;
        BinaryPrimitives.WriteUInt16BigEndian(buffer.AsSpan(offset), (ushort)Data.Length);
        offset += 2;
        Data.CopyTo(buffer.AsSpan(offset));
        return buffer;
    }
}

public class Confirmation
{
    public uint ClientMsgId { get; }
    public string Imei { get; }
    public uint IdReference { get; }
    public MessageStatus Status { get; }

    public Confirmation(uint clientMsgId, string imei, uint idReference, MessageStatus status)
    {
        ClientMsgId = clientMsgId;
        Imei = imei;
        IdReference = idReference;
        Status = status;
    }

    public static Confirmation FromArray(ReadOnlySpan<byte> data)
    {
        int offset = 0;
        if (data[offset++] != 0x44) throw new ArgumentException("Invalid confirmation element");
        ushort len = BinaryPrimitives.ReadUInt16BigEndian(data.Slice(offset, 2));
        offset += 2;
        if (len != 25) throw new ArgumentException("Unexpected confirmation length");
        uint clientId = BinaryPrimitives.ReadUInt32BigEndian(data.Slice(offset, 4));
        offset += 4;
        string imei = Encoding.ASCII.GetString(data.Slice(offset, 15));
        offset += 15;
        uint refId = BinaryPrimitives.ReadUInt32BigEndian(data.Slice(offset, 4));
        offset += 4;
        short statusCode = BinaryPrimitives.ReadInt16BigEndian(data.Slice(offset, 2));
        var status = (MessageStatus)statusCode;
        return new Confirmation(clientId, imei, refId, status);
    }
}

public class MOMessage
{
    public string Imei { get; }
    public byte[] Payload { get; }

    private MOMessage(string imei, byte[] payload)
    {
        Imei = imei;
        Payload = payload;
    }

    public static MOMessage FromArray(ReadOnlySpan<byte> data)
    {
        if (data.Length < 3)
            throw new ArgumentException("Invalid data length", nameof(data));
        if (data[0] != 1)
            throw new ArgumentException("Unsupported protocol version", nameof(data));

        ushort length = BinaryPrimitives.ReadUInt16BigEndian(data.Slice(1, 2));
        if (data.Length - 3 < length)
            throw new ArgumentException("Incomplete message", nameof(data));

        int offset = 3;
        if (data[offset++] != 0x01)
            throw new ArgumentException("Missing MO-Header", nameof(data));
        ushort hlen = BinaryPrimitives.ReadUInt16BigEndian(data.Slice(offset, 2));
        offset += 2;
        if (hlen != 28)
            throw new ArgumentException("Unexpected header length", nameof(data));

        offset += 4; // CDR reference
        string imei = Encoding.ASCII.GetString(data.Slice(offset, 15));
        offset += 15;
        offset += 1; // session status
        offset += 2; // MOMSN
        offset += 2; // MTMSN
        offset += 4; // time of session

        if (data[offset++] != 0x02)
            throw new ArgumentException("Missing MO-Payload", nameof(data));
        ushort plen = BinaryPrimitives.ReadUInt16BigEndian(data.Slice(offset, 2));
        offset += 2;
        byte[] payload = data.Slice(offset, plen).ToArray();

        return new MOMessage(imei, payload);
    }
}

