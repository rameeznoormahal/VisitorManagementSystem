using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VMS.Domain.Entities;
using VMS.Infrastructure.Identity;

namespace VMS.Infrastructure.Data;

public class VmsDbContext : IdentityDbContext<ApplicationUser>
{
    public VmsDbContext(DbContextOptions<VmsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<GroupPermission> GroupPermissions => Set<GroupPermission>();
    public DbSet<UserGroup> UserGroups => Set<UserGroup>();
    public DbSet<Visitor> Visitors => Set<Visitor>();
    public DbSet<VisitRequest> VisitRequests => Set<VisitRequest>();
    public DbSet<VisitVisitor> VisitVisitors => Set<VisitVisitor>();
    public DbSet<VisitAccessLog> VisitAccessLogs => Set<VisitAccessLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Department>(entity =>
        {
            entity.HasKey(x => x.DepartmentId);

            entity.Property(x => x.DepartmentCode)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.DepartmentName)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(500);

            entity.HasIndex(x => x.DepartmentCode)
                .IsUnique();

            entity.HasIndex(x => x.DepartmentName)
                .IsUnique();
        });

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(x => x.EmployeeCode)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.FullName)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.JobTitle)
                .HasMaxLength(150);

            entity.HasIndex(x => x.EmployeeCode)
                .IsUnique();

            entity.HasOne(x => x.Department)
                .WithMany()
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Manager)
                .WithMany(x => x.DirectReports)
                .HasForeignKey(x => x.ManagerUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Group>(entity =>
        {
            entity.HasKey(x => x.GroupId);

            entity.Property(x => x.GroupName)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(500);

            entity.HasIndex(x => x.GroupName)
                .IsUnique();
        });

        builder.Entity<Permission>(entity =>
        {
            entity.HasKey(x => x.PermissionId);

            entity.Property(x => x.PermissionCode)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.PermissionName)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(500);

            entity.HasIndex(x => x.PermissionCode)
                .IsUnique();
        });

        builder.Entity<GroupPermission>(entity =>
        {
            entity.HasKey(x => new
            {
                x.GroupId,
                x.PermissionId
            });

            entity.HasOne(x => x.Group)
                .WithMany(x => x.GroupPermissions)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Permission)
                .WithMany(x => x.GroupPermissions)
                .HasForeignKey(x => x.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<UserGroup>(entity =>
        {
            entity.HasKey(x => new
            {
                x.UserId,
                x.GroupId
            });

            entity.Property(x => x.UserId)
                .HasMaxLength(450)
                .IsRequired();

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Group)
                .WithMany()
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Visitor>(entity =>
        {
            entity.HasKey(x => x.VisitorId);

            entity.Property(x => x.IdType)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.IdNumber)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.FullName)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.PhoneNumber)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(x => x.Email)
                .HasMaxLength(200);

            entity.Property(x => x.CompanyName)
                .HasMaxLength(200);

            entity.Property(x => x.Designation)
                .HasMaxLength(150);

            entity.Property(x => x.Nationality)
                .HasMaxLength(100);

            entity.HasIndex(x => x.IdNumber)
                .IsUnique();

            entity.HasIndex(x => x.PhoneNumber);

            entity.HasIndex(x => x.Email);
        });

        builder.Entity<VisitRequest>(entity =>
        {
            entity.Property(x => x.VisitFromDateTime)
                .IsRequired();

            entity.Property(x => x.VisitToDateTime)
                .IsRequired();

            entity.HasIndex(x => x.VisitFromDateTime);
            entity.HasIndex(x => x.VisitToDateTime);

            entity.Property(x => x.QRTokenHash)
                .HasMaxLength(64);

            entity.Property(x => x.QRTokenProtected)
                .HasMaxLength(2000);

            entity.Property(x => x.QRGeneratedByUserId)
                .HasMaxLength(450);

            entity.HasIndex(x => x.QRTokenHash)
                .IsUnique()
                .HasFilter("[QRTokenHash] IS NOT NULL");

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.QRGeneratedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<VisitVisitor>(entity =>
        {
            entity.HasKey(x => x.VisitVisitorId);

            entity.HasOne(x => x.VisitRequest)
                .WithMany(x => x.VisitVisitors)
                .HasForeignKey(x => x.VisitRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Visitor)
                .WithMany(x => x.VisitVisitors)
                .HasForeignKey(x => x.VisitorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new
            {
                x.VisitRequestId,
                x.VisitorId
            })
            .IsUnique();
        });

        builder.Entity<VisitAccessLog>(entity =>
        {
            entity.HasKey(x => x.VisitAccessLogId);

            entity.Property(x => x.EntryProcessedByUserId)
                .HasMaxLength(450)
                .IsRequired();

            entity.Property(x => x.ExitProcessedByUserId)
                .HasMaxLength(450);

            entity.Property(x => x.EntryGateOrLocation)
                .HasMaxLength(200);

            entity.Property(x => x.ExitGateOrLocation)
                .HasMaxLength(200);

            entity.HasOne(x => x.VisitRequest)
                .WithMany(x => x.AccessLogs)
                .HasForeignKey(x => x.VisitRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.VisitVisitor)
                .WithMany(x => x.AccessLogs)
                .HasForeignKey(x => x.VisitVisitorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.EntryProcessedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.ExitProcessedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.EntryTime);

            entity.HasIndex(x => x.ExitTime);

            entity.HasIndex(x => new
            {
                x.VisitVisitorId,
                x.EntryTime
            });
        });
    }
}