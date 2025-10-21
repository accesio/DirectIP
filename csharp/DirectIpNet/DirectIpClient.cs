using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SBDDirectIP;

public class DirectIpClient
{
    private readonly string _host;
    private readonly int _port;

    public DirectIpClient(string host, int port)
    {
        _host = host;
        _port = port;
    }

    public async Task<Confirmation> SendAsync(MTMessage message)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(_host, _port);
        using var stream = client.GetStream();
        var data = message.ToArray();
        await stream.WriteAsync(data, 0, data.Length);
        var buffer = new byte[56];
        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
        return Confirmation.FromArray(buffer.AsSpan(3));
    }
}
