using VMS.Domain.Enums;

namespace VMS.Domain.Entities;

public class VisitRequest
{
    public long VisitRequestId { get; set; }

    public string VisitReference { get; set; } = string.Empty;

    public string HostUserId { get; set; } = string.Empty;

    public int? DepartmentId { get; set; }

    public Department? Department { get; set; }

    public string Purpose { get; set; } = string.Empty;

    public string? MeetingLocation { get; set; }

    public string? Notes { get; set; }

    public VisitStatus Status { get; set; } = VisitStatus.Draft;

    public string CreatedByUserId { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }

    public string? DecisionByUserId { get; set; }

    public DateTime? DecisionDate { get; set; }

    public string? DecisionComments { get; set; }

    public DateTime VisitFromDateTime { get; set; }

    public DateTime VisitToDateTime { get; set; }

    // One QR per request
    public string? QRTokenHash { get; set; }

    public string? QRTokenProtected { get; set; }

    public DateTime? QRGeneratedDate { get; set; }

    public string? QRGeneratedByUserId { get; set; }

    // Multiple visitors under one request
    public ICollection<VisitVisitor> VisitVisitors { get; set; }
        = new List<VisitVisitor>();

    // Multiple entry / exit sessions across multiple days
    public ICollection<VisitAccessLog> AccessLogs { get; set; }
        = new List<VisitAccessLog>();
}