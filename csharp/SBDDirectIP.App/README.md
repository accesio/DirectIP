# SBDDirectIP Windows App

This WinForms application demonstrates how to send MT messages using `SBDDirectIP.dll`.

## Building

```
dotnet build
```

## Usage

Fill in the IMEI, AES key (64 hex characters), gateway host and port,
and the payload. By default the payload must be space‑separated hexadecimal
byte values (for example `AA 55 10 01`) with a maximum of 30 bytes. Enable the
**Send ASCII** option to send the payload as literal ASCII text instead.
Use the **Encrypt** and **Decrypt** checkboxes to toggle AES256 processing. The
state of all checkboxes is stored in the user settings. Press **Send** to
transmit the message. Incoming MO messages are accepted on port 10800 and
displayed in the log box in both hex and ASCII forms; payloads are decrypted
only when the **Decrypt** option is enabled.
