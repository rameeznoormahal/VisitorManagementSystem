using System.ComponentModel.DataAnnotations;

namespace VMS.Web.ViewModels.Groups;

public class GroupViewModel
{
    public int GroupId { get; set; }

    [Required]
    [StringLength(150)]
    [Display(Name = "Group Name")]
    public string GroupName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public List<PermissionSelectionViewModel> Permissions { get; set; }
        = new();
}