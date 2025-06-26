using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace SBDDirectIP;

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
