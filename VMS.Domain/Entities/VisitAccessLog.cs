namespace VMS.Domain.Entities;

public class VisitAccessLog
{
    public long VisitAccessLogId { get; set; }

    public long VisitRequestId { get; set; }

    public VisitRequest VisitRequest { get; set; } = null!;

    public long VisitVisitorId { get; set; }

    public VisitVisitor VisitVisitor { get; set; } = null!;

    public DateTime EntryTime { get; set; }

    public DateTime? ExitTime { get; set; }

    public string EntryProcessedByUserId { get; set; } = string.Empty;

    public string? ExitProcessedByUserId { get; set; }

    public string? EntryGateOrLocation { get; set; }

    public string? ExitGateOrLocation { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}