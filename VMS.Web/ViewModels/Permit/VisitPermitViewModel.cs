namespace VMS.Web.ViewModels.Permit;

public class VisitPermitViewModel
{
    public long VisitRequestId { get; set; }

    public string VisitReference { get; set; } = string.Empty;

    public DateTime VisitFromDateTime { get; set; }

    public DateTime VisitToDateTime { get; set; }

    public string Purpose { get; set; } = string.Empty;

    public string? MeetingLocation { get; set; }

    public string? DepartmentName { get; set; }

    public string HostName { get; set; } = string.Empty;

    public string RequestedBy { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime? QRGeneratedDate { get; set; }

    public string QrBase64 { get; set; } = string.Empty;

    public List<VisitPermitVisitorViewModel> Visitors { get; set; }
        = new();
}