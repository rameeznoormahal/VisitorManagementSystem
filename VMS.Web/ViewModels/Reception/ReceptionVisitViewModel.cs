namespace VMS.Web.ViewModels.Reception;

public class ReceptionVisitViewModel
{
    public long VisitRequestId { get; set; }

    public string VisitReference { get; set; } = string.Empty;

    public DateTime VisitFromDateTime { get; set; }

    public DateTime VisitToDateTime { get; set; }

    public string Purpose { get; set; } = string.Empty;

    public string? MeetingLocation { get; set; }

    public string? DepartmentName { get; set; }

    public string HostName { get; set; } = string.Empty;

    public bool IsWithinValidPeriod { get; set; }

    public List<ReceptionVisitorViewModel> Visitors { get; set; }
        = new();
}