using VMS.Web.ViewModels.Permit;

namespace VMS.Web.Services;

public interface IVisitPermitPdfService
{
    byte[] Generate(VisitPermitViewModel model);
}