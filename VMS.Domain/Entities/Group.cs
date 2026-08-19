namespace VMS.Domain.Entities;

public class Group
{
    public int GroupId { get; set; }

    public string GroupName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }

    public ICollection<GroupPermission> GroupPermissions { get; set; }
        = new List<GroupPermission>();
}