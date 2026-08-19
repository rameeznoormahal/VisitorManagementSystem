using System.ComponentModel.DataAnnotations;

namespace VMS.Web.ViewModels.Users;

public class ResetPasswordViewModel
{
    public string UserId { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "New Temporary Password")]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword))]
    [Display(Name = "Confirm Temporary Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}