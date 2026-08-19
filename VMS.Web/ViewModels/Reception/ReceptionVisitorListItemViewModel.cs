namespace VMS.Web.ViewModels.Reception;

public class ReceptionVisitorListItemViewModel
{
    public long VisitRequestId { get; set; }

    public long VisitVisitorId { get; set; }

    public long VisitorId { get; set; }

    public string VisitReference { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string IdNumber { get; set; } = string.Empty;

    public string? CompanyName { get; set; }

    public DateTime VisitFromDateTime { get; set; }

    public DateTime VisitToDateTime { get; set; }

    public DateTime? LastCheckInTime { get; set; }

    public DateTime? LastCheckOutTime { get; set; }

    public bool IsCurrentlyInside { get; set; }
}