using System.Buffers.Binary;

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
