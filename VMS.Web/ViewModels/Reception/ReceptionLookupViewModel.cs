using System.ComponentModel.DataAnnotations;

namespace VMS.Web.ViewModels.Reception;

public class ReceptionLookupViewModel
{
    [Required]
    [Display(Name = "QR Code / Token")]
    public string Token { get; set; } = string.Empty;
}