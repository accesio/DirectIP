# DirectIpNet.Native

This project exposes the `SBDDirectIP` API as a native C library. It uses
`UnmanagedCallersOnly` so the resulting binary can be consumed from any language
that can call C functions.

## Building

Use `dotnet publish` with NativeAOT enabled to produce a shared library:

```shell
cd csharp/DirectIpNet.Native
dotnet publish -c Release -r linux-x64
```

The compiled library will be placed under `bin/Release/net8.0/linux-x64/native`.

## Usage

Include `directip.h` in your C or C++ project and link against the produced
library. Example:

```c
#include "directip.h"

int main() {
    ConfirmationInfo conf;
    const char payload[] = "Hello";
    int rc = directip_send_mt("127.0.0.1", 10800, 1,
                              "012345678901234",
                              (const uint8_t*)payload, sizeof(payload)-1,
                              &conf);
    return rc;
}
```
