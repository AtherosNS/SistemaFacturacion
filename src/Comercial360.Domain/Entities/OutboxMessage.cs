namespace Comercial360.Domain.Entities;

public sealed class OutboxMessage
{
    private OutboxMessage() { }

    private OutboxMessage(Guid electronicDocumentId, string payload, DateTimeOffset createdAtUtc)
    {
        Id = Guid.NewGuid();
        ElectronicDocumentId = electronicDocumentId;
        EventType = "ElectronicDocumentRequested";
        Payload = payload;
        Status = "Pending";
        CreatedAtUtc = createdAtUtc.ToUniversalTime();
    }

    public static OutboxMessage Create(Guid electronicDocumentId, string payload, DateTimeOffset createdAtUtc)
    {
        if (electronicDocumentId == Guid.Empty || string.IsNullOrWhiteSpace(payload))
        {
            throw new ArgumentException("El mensaje de outbox requiere un documento electrónico y un payload.");
        }

        return new OutboxMessage(electronicDocumentId, payload, createdAtUtc);
    }

    public Guid Id { get; private set; }
    public Guid ElectronicDocumentId { get; private set; }
    public string EventType { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public int Attempts { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? NextAttemptAtUtc { get; private set; }
}
