using System.Buffers.Binary;

namespace SBDDirectIP;

internal class MTPayload
{
    public byte[] Data { get; }
    public MTPayload(byte[] data)
    {
        Data = data ?? System.Array.Empty<byte>();
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
