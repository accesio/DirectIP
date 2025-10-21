using System.Configuration;

namespace SBDDirectIP.App.Properties;

internal sealed partial class Settings : ApplicationSettingsBase
{
    private static readonly Settings defaultInstance = (Settings)Synchronized(new Settings());

    public static Settings Default => defaultInstance;

    [UserScopedSetting]
    [DefaultSettingValue("")]
    public string IMEI
    {
        get => (string)this[nameof(IMEI)];
        set => this[nameof(IMEI)] = value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("")]
    public string AESKey
    {
        get => (string)this[nameof(AESKey)];
        set => this[nameof(AESKey)] = value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("directip.sbd.iridium.com:10800")]
    public string ServerHostPort
    {
        get => (string)this[nameof(ServerHostPort)];
        set => this[nameof(ServerHostPort)] = value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("True")]
    public bool EncryptOnSend
    {
        get => (bool)this[nameof(EncryptOnSend)];
        set => this[nameof(EncryptOnSend)] = value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("True")]
    public bool DecryptOnReceive
    {
        get => (bool)this[nameof(DecryptOnReceive)];
        set => this[nameof(DecryptOnReceive)] = value;
    }
}
