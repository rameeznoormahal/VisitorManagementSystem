namespace VMS.Web.ViewModels.Reception;

public class AccessHistoryViewModel
{
    public string VisitReference { get; set; } = string.Empty;

    public string VisitorName { get; set; } = string.Empty;

    public string IdNumber { get; set; } = string.Empty;

    public DateTime EntryTime { get; set; }

    public DateTime? ExitTime { get; set; }

    public string? EntryProcessedBy { get; set; }

    public string? ExitProcessedBy { get; set; }

    public string? EntryLocation { get; set; }

    public string? ExitLocation { get; set; }
}