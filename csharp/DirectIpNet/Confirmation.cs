using System;
using System.Buffers.Binary;

namespace SBDDirectIP;

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
        ushort len = BinaryPrimitives.ReadUInt16BigEndian(data.Slice(offset,2));
        offset += 2;
        if (len != 25) throw new ArgumentException("Unexpected confirmation length");
        uint clientId = BinaryPrimitives.ReadUInt32BigEndian(data.Slice(offset,4));
        offset += 4;
        string imei = System.Text.Encoding.ASCII.GetString(data.Slice(offset,15));
        offset += 15;
        uint refId = BinaryPrimitives.ReadUInt32BigEndian(data.Slice(offset,4));
        offset += 4;
        short statusCode = BinaryPrimitives.ReadInt16BigEndian(data.Slice(offset,2));
        var status = (MessageStatus)statusCode;
        return new Confirmation(clientId, imei, refId, status);
    }
}
