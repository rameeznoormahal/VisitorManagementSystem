namespace VMS.Domain.Entities;

public class VisitVisitor
{
    public long VisitVisitorId { get; set; }

    public long VisitRequestId { get; set; }

    public VisitRequest VisitRequest { get; set; } = null!;

    public long VisitorId { get; set; }

    public Visitor Visitor { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public ICollection<VisitAccessLog> AccessLogs { get; set; }
        = new List<VisitAccessLog>();
}