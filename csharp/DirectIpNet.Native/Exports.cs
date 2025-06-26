using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SBDDirectIP;

namespace SBDDirectIP.Native;

public static unsafe class Exports
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ConfirmationInfo
    {
        public uint ClientMsgId;
        public uint IdReference;
        public MessageStatus Status;
    }

    [UnmanagedCallersOnly(EntryPoint = "directip_send_mt")]
    public static int SendMt(byte* host, int port, uint clientMsgId, byte* imei, byte* payload, int payloadLen, ConfirmationInfo* confirmation)
    {
        try
        {
            var hostStr = Marshal.PtrToStringUTF8((IntPtr)host)!;
            var imeiStr = Marshal.PtrToStringUTF8((IntPtr)imei)!;
            var data = new byte[payloadLen];
            if (payload != null && payloadLen > 0)
            {
                Marshal.Copy((IntPtr)payload, data, 0, payloadLen);
            }

            var client = new DirectIpClient(hostStr, port);
            var msg = MTMessage.Builder()
                .ClientMsgId(clientMsgId)
                .Imei(imeiStr)
                .Payload(data)
                .Build();

            Task<Confirmation> task = client.SendAsync(msg);
            task.Wait();
            Confirmation conf = task.Result;

            if (confirmation != null)
            {
                confirmation->ClientMsgId = conf.ClientMsgId;
                confirmation->IdReference = conf.IdReference;
                confirmation->Status = conf.Status;
            }

            return 0;
        }
        catch
        {
            return -1;
        }
    }
}
