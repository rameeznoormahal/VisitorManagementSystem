namespace VMS.Web.ViewModels.Reception;

public class ReceptionVisitorViewModel
{
    public long VisitVisitorId { get; set; }

    public long VisitorId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string IdNumber { get; set; } = string.Empty;

    public string? CompanyName { get; set; }

    public string? Designation { get; set; }

    public bool IsCurrentlyInside { get; set; }

    public DateTime? LastCheckInTime { get; set; }

    public DateTime? LastCheckOutTime { get; set; }
}