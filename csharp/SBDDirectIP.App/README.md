# SBDDirectIP Windows App

This WinForms application demonstrates how to send MT messages using `SBDDirectIP.dll`.

## Building

```
dotnet build
```

## Usage

Fill in the IMEI, AES key (64 hex characters), gateway host and port,
and the payload as space‑separated hexadecimal byte values (for example
`AA 55 10 01`) with a maximum of 30 bytes. Use the **Encrypt** and **Decrypt**
checkboxes to toggle AES256 processing. The states of these options are saved.
Press **Send** to transmit the message. Incoming MO messages are accepted on
port 10800 and displayed in the log box; payloads are decrypted only when the
**Decrypt** option is enabled.
