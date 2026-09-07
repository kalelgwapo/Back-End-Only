using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
    public DbSet<LeaveApplication> LeaveApplications => Set<LeaveApplication>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(user => user.Id);
            entity.Property(user => user.Username).HasMaxLength(50).IsRequired();
            entity.HasIndex(user => user.Username).IsUnique();
            entity.Property(user => user.FullName).HasMaxLength(200).IsRequired();
            entity.Property(user => user.Email).HasMaxLength(320).IsRequired();
            entity.HasIndex(user => user.Email).IsUnique();
            entity.Property(user => user.PasswordHash).HasMaxLength(500).IsRequired();
            entity.Property(user => user.Role).HasMaxLength(50).IsRequired();
            entity.Property(user => user.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<LeaveType>(entity =>
        {
            entity.ToTable("LeaveType", "Lookup");
            entity.HasKey(leaveType => leaveType.Id);
            entity.Property(leaveType => leaveType.Name).HasMaxLength(50).IsRequired();
            entity.HasIndex(leaveType => leaveType.Name).IsUnique();
            entity.Property(leaveType => leaveType.MaxDaysAllowed).IsRequired();
        });

        modelBuilder.Entity<LeaveApplication>(entity =>
        {
            entity.ToTable("LeaveApplications");
            entity.HasKey(application => application.Id);
            entity.Property(application => application.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();
            entity.Property(application => application.GeneralComments).HasMaxLength(2000);
            entity.Property(application => application.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(application => new { application.ApplicantId, application.Status });
            entity.HasIndex(application => application.ManagerId);
            entity.HasIndex(application => application.LeaveTypeId);

            entity.HasOne(application => application.Applicant)
                .WithMany(user => user.Applications)
                .HasForeignKey(application => application.ApplicantId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(application => application.Manager)
                .WithMany(user => user.ManagedApplications)
                .HasForeignKey(application => application.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(application => application.LeaveType)
                .WithMany(leaveType => leaveType.Applications)
                .HasForeignKey(application => application.LeaveTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Username = "jdoe", FullName = "Jane Doe", Email = "jane.doe@example.com", PasswordHash = "AQAAAAIAAYagAAAAEKmyBZSOdiRCMd5DIuHtL0bfJ4gHH+CaF1tfDxKSrGczzKlfmYrk66lRfODdGYRQAQ==", Role = "Employee", IsActive = true, CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero) },
            new User { Id = 2, Username = "jsmith", FullName = "John Smith", Email = "john.smith@example.com", PasswordHash = "AQAAAAIAAYagAAAAEKmyBZSOdiRCMd5DIuHtL0bfJ4gHH+CaF1tfDxKSrGczzKlfmYrk66lRfODdGYRQAQ==", Role = "Employee", IsActive = true, CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero) },
            new User { Id = 3, Username = "msantos", FullName = "Maria Santos", Email = "maria.santos@example.com", PasswordHash = "AQAAAAIAAYagAAAAEKmyBZSOdiRCMd5DIuHtL0bfJ4gHH+CaF1tfDxKSrGczzKlfmYrk66lRfODdGYRQAQ==", Role = "Employee", IsActive = true, CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero) },
            new User { Id = 4, Username = "rhale", FullName = "Robert Hale", Email = "robert.hale@example.com", PasswordHash = "AQAAAAIAAYagAAAAEKmyBZSOdiRCMd5DIuHtL0bfJ4gHH+CaF1tfDxKSrGczzKlfmYrk66lRfODdGYRQAQ==", Role = "Manager", IsActive = true, CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero) },
            new User { Id = 5, Username = "pnair", FullName = "Priya Nair", Email = "priya.nair@example.com", PasswordHash = "AQAAAAIAAYagAAAAEKmyBZSOdiRCMd5DIuHtL0bfJ4gHH+CaF1tfDxKSrGczzKlfmYrk66lRfODdGYRQAQ==", Role = "Manager", IsActive = true, CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero) },
            new User { Id = 6, Username = "dkim", FullName = "David Kim", Email = "david.kim@example.com", PasswordHash = "AQAAAAIAAYagAAAAEKmyBZSOdiRCMd5DIuHtL0bfJ4gHH+CaF1tfDxKSrGczzKlfmYrk66lRfODdGYRQAQ==", Role = "Manager", IsActive = true, CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero) });

        modelBuilder.Entity<LeaveType>().HasData(
            new LeaveType { Id = 1, Name = "annual", MaxDaysAllowed = 20, IsActive = true },
            new LeaveType { Id = 2, Name = "sick", MaxDaysAllowed = 7, IsActive = true },
            new LeaveType { Id = 3, Name = "emergency", MaxDaysAllowed = 3, IsActive = true });
    }
}