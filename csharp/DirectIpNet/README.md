# SBDDirectIP

This is a lightweight C# implementation of Iridium's Short Burst Data Direct-IP protocol.
It provides the ability to create and send Mobile Terminated (MT) messages,
parse confirmation responses from the gateway and decode Mobile Originated (MO)
messages. The code mirrors the functionality of the Rust library contained in
the repository but targets .NET applications.

## Building

```
dotnet build
```

## Usage

```
var client = new SBDDirectIP.DirectIpClient("127.0.0.1", 10800);
var msg = MTMessage.Builder()
    .ClientMsgId(123)
    .Imei("012345678901234")
    .Payload(System.Text.Encoding.ASCII.GetBytes("Hello"))
    .Build();
var confirmation = await client.SendAsync(msg);
Console.WriteLine(confirmation.Status);
```

MO messages received from a device can be parsed using `MOMessage.FromArray` to
extract the encrypted payload:

```
var mo = MOMessage.FromArray(data);
byte[] payload = mo.Payload;
```

This library can be referenced from any .NET language. For integration with
native applications, see the `DirectIpNet.Native` project which exports a C API
around this library.
