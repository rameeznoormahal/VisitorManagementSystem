using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VMS.Web.ViewModels.Users;

public class UserCreateViewModel
{
    [Required]
    [StringLength(50)]
    [Display(Name = "Employee Code")]
    public string EmployeeCode { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [StringLength(150)]
    [Display(Name = "Job Title")]
    public string? JobTitle { get; set; }

    [Display(Name = "Department")]
    public int? DepartmentId { get; set; }

    [Display(Name = "Reporting Manager")]
    public string? ManagerUserId { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Temporary Password")]
    public string TemporaryPassword { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(TemporaryPassword))]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Groups")]
    public List<int> SelectedGroupIds { get; set; } = new();

    public List<SelectListItem> Departments { get; set; } = new();

    public List<SelectListItem> Managers { get; set; } = new();

    public List<SelectListItem> Groups { get; set; } = new();
}