namespace VMS.Web.ViewModels.Reports;

public class VisitorReportViewModel
{
    public string? Search { get; set; }

    public string? Status { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public List<VisitorReportItemViewModel> Items { get; set; } = new();
}

public class VisitorReportItemViewModel
{
    public long VisitorId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string IdType { get; set; } = string.Empty;

    public string IdNumber { get; set; } = string.Empty;

    public DateOnly IdExpiryDate { get; set; }

    public string? Nationality { get; set; }

    public string? CompanyName { get; set; }

    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; }

    public int TotalVisits { get; set; }

    public DateTime? LastVisitDate { get; set; }
}