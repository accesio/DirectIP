using System;
using System.Windows.Forms;
using SBDDirectIP;
using System.Security.Cryptography;

namespace SBDDirectIP.App;

public partial class MainForm : Form
{
    private TextBox imeiTextBox = null!;
    private TextBox keyTextBox = null!;
    private TextBox serverTextBox = null!;
    private TextBox payloadTextBox = null!;
    private Button sendButton = null!;
    private TextBox outputTextBox = null!;
    private CheckBox encryptCheckBox = null!;
    private CheckBox decryptCheckBox = null!;
    private System.Threading.CancellationTokenSource? listenerCts;

    public MainForm()
    {
        InitializeComponent();
        Load += (_, _) => StartListener();
        FormClosing += (_, _) => listenerCts?.Cancel();

        imeiTextBox.Text = Properties.Settings.Default.IMEI;
        keyTextBox.Text = Properties.Settings.Default.AESKey;
        serverTextBox.Text = Properties.Settings.Default.ServerHostPort;
        encryptCheckBox.Checked = Properties.Settings.Default.EncryptOnSend;
        decryptCheckBox.Checked = Properties.Settings.Default.DecryptOnReceive;
    }

    private async void OnSend(object? sender, EventArgs e)
    {
        try
        {
            sendButton.Enabled = false;
            Log("Preparing message");
            string imei = imeiTextBox.Text.Trim();
            string keyHex = keyTextBox.Text.Trim();
            string server = serverTextBox.Text.Trim();
            string payloadHex = payloadTextBox.Text.Trim();
            var tokens = payloadHex.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0 || tokens.Length > 30)
            {
                Log("Invalid payload length");
                MessageBox.Show("Payload must be space separated hex bytes (max 30 bytes).");
                return;
            }
            byte[] payload = new byte[tokens.Length];
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i].Length != 2 || !byte.TryParse(tokens[i], System.Globalization.NumberStyles.HexNumber, null, out payload[i]))
                {
                    Log("Invalid payload format");
                    MessageBox.Show("Payload must be space separated hex bytes like 'AA 55'.");
                    return;
                }
            }
            byte[] key = Array.Empty<byte>();
            if (encryptCheckBox.Checked)
            {
                try { key = Convert.FromHexString(keyHex); } catch { }
                if (key.Length != 32)
                {
                    Log("Invalid AES key length");
                    MessageBox.Show("AES Key must be 32 bytes (64 hex characters, no spaces).");
                    return;
                }
                Log($"Encrypting {payload.Length} bytes");
                using var aes = Aes.Create();
                aes.Key = key;
                aes.IV = new byte[16];
                using var encryptor = aes.CreateEncryptor();
                payload = encryptor.TransformFinalBlock(payload, 0, payload.Length);
            }

            var settings = Properties.Settings.Default;
            settings.IMEI = imei;
            settings.AESKey = keyHex;
            settings.ServerHostPort = server;
            settings.EncryptOnSend = encryptCheckBox.Checked;
            settings.DecryptOnReceive = decryptCheckBox.Checked;
            settings.Save();

            string host = server; int port = 10800;
            if (server.Contains(":"))
            {
                var parts = server.Split(':',2);
                host = parts[0];
                port = int.Parse(parts[1]);
            }

            Log($"Connecting to {host}:{port}");
            var client = new DirectIpClient(host, port);
            var msg = MTMessage.Builder()
                .ClientMsgId(1)
                .Imei(imei)
                .Payload(payload)
                .Build();
            Log("Sending message");
            var conf = await client.SendAsync(msg);
            Log($"Status: {conf.Status} Ref: {conf.IdReference}");
            outputTextBox.Text = $"Status: {conf.Status}";
        }
        catch (Exception ex)
        {
            Log($"Error: {ex.Message}");
            MessageBox.Show(ex.Message);
        }
        finally
        {
            sendButton.Enabled = true;
        }
    }

    private void Log(string message)
    {
        if (InvokeRequired)
            Invoke(new Action(() => Log(message)));
        else
            outputTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
    }

    private void StartListener()
    {
        listenerCts = new System.Threading.CancellationTokenSource();
        var ct = listenerCts.Token;
        System.Threading.Tasks.Task.Run(async () =>
        {
            var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Any, 10800);
            listener.Start();
            Log("Listening on port 10800");
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    var client = await listener.AcceptTcpClientAsync(ct);
                    _ = HandleClientAsync(client, ct);
                }
            }
            catch (OperationCanceledException) { }
            finally
            {
                listener.Stop();
            }
        }, ct);
    }

    private async System.Threading.Tasks.Task HandleClientAsync(System.Net.Sockets.TcpClient client, System.Threading.CancellationToken ct)
    {
        using (client)
        {
            try
            {
                var stream = client.GetStream();
                byte[] prefix = new byte[3];
                if (await ReadExactAsync(stream, prefix.AsMemory(), ct) != 3) return;
                ushort len = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(prefix.AsSpan(1));
                byte[] buffer = new byte[3 + len];
                prefix.CopyTo(buffer, 0);
                if (await ReadExactAsync(stream, buffer.AsMemory(3, len), ct) != len) return;

                var mo = MOMessage.FromArray(buffer);
                string keyHex = "";
                bool decrypt = true;
                this.Invoke(new Action(() =>
                {
                    keyHex = keyTextBox.Text.Trim();
                    decrypt = decryptCheckBox.Checked;
                }));

                byte[] plain = mo.Payload;
                if (decrypt)
                {
                    byte[] key = Array.Empty<byte>();
                    try { key = Convert.FromHexString(keyHex); } catch { }
                    if (key.Length != 32)
                    {
                        Invoke(new Action(() => Log("Received message but key invalid")));
                        return;
                    }
                    using var aes = Aes.Create();
                    aes.Key = key;
                    aes.IV = new byte[16];
                    using var decryptor = aes.CreateDecryptor();
                    plain = decryptor.TransformFinalBlock(mo.Payload, 0, mo.Payload.Length);
                }
                string hex = BitConverter.ToString(plain).Replace("-", " ");
                Invoke(new Action(() => Log($"RX from {mo.Imei}: {hex}")));
            }
            catch (Exception ex)
            {
                Invoke(new Action(() => Log($"Receive error: {ex.Message}")));
            }
        }
    }

    private static async System.Threading.Tasks.Task<int> ReadExactAsync(System.Net.Sockets.NetworkStream stream, Memory<byte> buffer, System.Threading.CancellationToken ct)
    {
        int read = 0;
        while (read < buffer.Length)
        {
            int n = await stream.ReadAsync(buffer.Slice(read), ct);
            if (n == 0) break;
            read += n;
        }
        return read;
    }
}
