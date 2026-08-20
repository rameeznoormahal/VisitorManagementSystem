namespace VMS.Web.ViewModels.Reports;

public class DepartmentSummaryViewModel
{
    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public List<DepartmentSummaryItemViewModel> Items { get; set; }
        = new();
}

public class DepartmentSummaryItemViewModel
{
    public int DepartmentId { get; set; }

    public string DepartmentName { get; set; } = string.Empty;

    public int TotalVisits { get; set; }

    public int TotalVisitors { get; set; }

    public int PendingVisits { get; set; }

    public int ApprovedVisits { get; set; }

    public int RejectedVisits { get; set; }

    public int CompletedVisits { get; set; }
}