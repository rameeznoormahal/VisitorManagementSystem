namespace VMS.Web.ViewModels.Visits;

public class QrPreviewViewModel
{
    public long VisitVisitorId { get; set; }

    public long VisitRequestId { get; set; }

    public string VisitReference { get; set; } = string.Empty;

    public string VisitorName { get; set; } = string.Empty;

    public DateTime VisitFromDateTime { get; set; }

    public DateTime VisitToDateTime { get; set; }

    public string QrBase64 { get; set; } = string.Empty;
}