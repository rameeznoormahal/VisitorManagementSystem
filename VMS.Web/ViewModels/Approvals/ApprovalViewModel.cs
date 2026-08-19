using System.ComponentModel.DataAnnotations;

namespace VMS.Web.ViewModels.Approvals;

public class ApprovalViewModel
{
    public long VisitRequestId { get; set; }

    public string VisitReference { get; set; } = string.Empty;

    public string HostName { get; set; } = string.Empty;

    public string? DepartmentName { get; set; }

    public DateTime VisitFromDateTime { get; set; }

    public DateTime VisitToDateTime { get; set; }

    public string Purpose { get; set; } = string.Empty;

    public string? MeetingLocation { get; set; }

    public string? Notes { get; set; }

    public List<ApprovalVisitorViewModel> Visitors { get; set; } = new();

    [StringLength(1000)]
    [Display(Name = "Comments")]
    public string? Comments { get; set; }
}

public class ApprovalVisitorViewModel
{
    public long VisitorId { get; set; }

    public string IdNumber { get; set; } = string.Empty;

    public string IdType { get; set; } = string.Empty;

    public DateOnly IdExpiryDate { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? CompanyName { get; set; }

    public string? Designation { get; set; }

    public string? Nationality { get; set; }
}