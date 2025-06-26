namespace SBDDirectIP;

/// <summary>
/// Status of a confirmation message returned by the gateway.
/// This mirrors the enum defined in the original Rust implementation.
/// </summary>
public enum MessageStatus
{
    SuccessfulQueueOrder = 0,
    InvalidImei = -1,
    UnknownImei = -2,
    PayloadOversized = -3,
    PayloadMissing = -4,
    MtQueueFull = -5,
    MtResourcesUnavailable = -6,
    ProtocolViolation = -7,
    RingAlertsDisabled = -8,
    SsdNotAttached = -9,
    SourceAddressRejected = -10,
    MtmsnOutOfRange = -11,
    CertificateRejected = -12
}
