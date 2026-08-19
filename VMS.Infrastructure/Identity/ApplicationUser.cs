using Microsoft.AspNetCore.Identity;
using VMS.Domain.Entities;

namespace VMS.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string EmployeeCode { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string? JobTitle { get; set; }

    public int? DepartmentId { get; set; }

    public Department? Department { get; set; }

    public string? ManagerUserId { get; set; }

    public ApplicationUser? Manager { get; set; }

    public ICollection<ApplicationUser> DirectReports { get; set; }
        = new List<ApplicationUser>();

    public bool IsActive { get; set; } = true;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }
}