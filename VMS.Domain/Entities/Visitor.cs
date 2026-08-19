namespace VMS.Domain.Entities;

public class Visitor
{
    public long VisitorId { get; set; }

    public string IdType { get; set; } = string.Empty;

    public string IdNumber { get; set; } = string.Empty;

    public DateOnly IdExpiryDate { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? CompanyName { get; set; }

    public string? Designation { get; set; }

    public string? Nationality { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }

    public ICollection<VisitVisitor> VisitVisitors { get; set; }
        = new List<VisitVisitor>();
}