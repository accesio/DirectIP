using System;
using System.Buffers.Binary;

namespace SBDDirectIP;

/// <summary>
/// Represents a Mobile Originated (MO) message received from a remote device.
/// Only the header fields required to locate the payload are parsed.
/// </summary>
public class MOMessage
{
    public string Imei { get; }
    public byte[] Payload { get; }

    private MOMessage(string imei, byte[] payload)
    {
        Imei = imei;
        Payload = payload;
    }

    /// <summary>
    /// Parse a raw DirectIP MO message.
    /// </summary>
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
        string imei = System.Text.Encoding.ASCII.GetString(data.Slice(offset, 15));
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
