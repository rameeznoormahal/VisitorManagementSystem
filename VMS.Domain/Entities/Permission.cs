namespace VMS.Domain.Entities;

public class Permission
{
    public int PermissionId { get; set; }

    public string PermissionCode { get; set; } = string.Empty;

    public string PermissionName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<GroupPermission> GroupPermissions { get; set; }
        = new List<GroupPermission>();
}