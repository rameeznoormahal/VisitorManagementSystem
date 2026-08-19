namespace VMS.Web.ViewModels.Groups;

public class PermissionSelectionViewModel
{
    public int PermissionId { get; set; }

    public string PermissionCode { get; set; } = string.Empty;

    public string PermissionName { get; set; } = string.Empty;

    public bool IsSelected { get; set; }
}