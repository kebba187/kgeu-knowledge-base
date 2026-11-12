using KbKgeu.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace KbKgeu.Api.Data;

/// <summary>
/// Контекст EF Core. Имена таблиц и колонок заданы явно,
/// чтобы схема совпадала с db/schema.sql из ЛР9 (snake_case).
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<User> Users => Set<User>();

    public DbSet<PublicationRequest> PublicationRequests => Set<PublicationRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("departments");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Id).HasColumnName("id");
            entity.Property(item => item.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
            entity.HasIndex(item => item.Name).IsUnique();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users", table => table.HasCheckConstraint(
                "users_role_check",
                "role IN ('Author', 'Coordinator', 'Editor')"));
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Id).HasColumnName("id");
            entity.Property(item => item.Login).HasColumnName("login").HasMaxLength(50).IsRequired();
            entity.Property(item => item.FullName).HasColumnName("full_name").HasMaxLength(150).IsRequired();
            entity.Property(item => item.Role).HasColumnName("role").HasMaxLength(20).IsRequired();
            entity.Property(item => item.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
            entity.HasIndex(item => item.Login).IsUnique();
        });

        modelBuilder.Entity<PublicationRequest>(entity =>
        {
            entity.ToTable("publication_requests", table => table.HasCheckConstraint(
                "publication_requests_status_check",
                "status IN ('New', 'InProgress', 'Closed', 'Cancelled')"));
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Id).HasColumnName("id");
            entity.Property(item => item.Number).HasColumnName("number").HasMaxLength(20).IsRequired();
            entity.Property(item => item.Title).HasColumnName("title").HasMaxLength(80).IsRequired();
            entity.Property(item => item.Description).HasColumnName("description").HasMaxLength(500).IsRequired();
            entity.Property(item => item.DepartmentId).HasColumnName("department_id");
            entity.Property(item => item.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
            entity.Property(item => item.CreatedByUserId).HasColumnName("created_by_user_id");
            // NULL разрешён: у новой заявки редактора ещё нет.
            entity.Property(item => item.AssigneeUserId).HasColumnName("assignee_user_id");
            entity.Property(item => item.CreatedAt).HasColumnName("created_at");
            entity.Property(item => item.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(item => item.Number).IsUnique();
            entity.HasIndex(item => item.Status).HasDatabaseName("publication_requests_status_idx");
            entity.HasIndex(item => item.DepartmentId).HasDatabaseName("publication_requests_department_id_idx");
            entity.HasIndex(item => item.AssigneeUserId).HasDatabaseName("publication_requests_assignee_user_id_idx");

            entity.HasOne<Department>()
                .WithMany()
                .HasForeignKey(item => item.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(item => item.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(item => item.AssigneeUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
