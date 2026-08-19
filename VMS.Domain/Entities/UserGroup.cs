namespace VMS.Domain.Entities;

public class UserGroup
{
    public string UserId { get; set; } = string.Empty;

    public int GroupId { get; set; }

    public Group Group { get; set; } = null!;
}