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
            // Ограничения те же, что в db/schema.sql: длины проверяет база,
            // а не только DTO и сервис.
            entity.ToTable("publication_requests", table =>
            {
                table.HasCheckConstraint(
                    "publication_requests_status_check",
                    "status IN ('New', 'InProgress', 'Closed', 'Cancelled')");
                table.HasCheckConstraint(
                    "publication_requests_title_check",
                    "char_length(title) BETWEEN 5 AND 80");
                table.HasCheckConstraint(
                    "publication_requests_description_check",
                    "char_length(description) BETWEEN 10 AND 500");
            });
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Id).HasColumnName("id");
            entity.Property(item => item.Number).HasColumnName("number").HasMaxLength(20).IsRequired();
            entity.Property(item => item.Title).HasColumnName("title").HasMaxLength(80).IsRequired();
            entity.Property(item => item.Description).HasColumnName("description").HasMaxLength(500).IsRequired();
            entity.Property(item => item.DepartmentId).HasColumnName("department_id");
            entity.Property(item => item.Status)
                .HasColumnName("status")
                .HasMaxLength(20)
                .IsRequired()
                .HasDefaultValue(RequestStatus.New);
            entity.Property(item => item.CreatedByUserId).HasColumnName("created_by_user_id");
            // NULL разрешён: у новой заявки редактора ещё нет.
            entity.Property(item => item.AssigneeUserId).HasColumnName("assignee_user_id");
            entity.Property(item => item.CreatedAt).HasColumnName("created_at");
            entity.Property(item => item.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(item => item.Number).IsUnique();
            entity.HasIndex(item => item.Status).HasDatabaseName("publication_requests_status_idx");
            entity.HasIndex(item => item.DepartmentId).HasDatabaseName("publication_requests_department_id_idx");
            entity.HasIndex(item => item.AssigneeUserId).HasDatabaseName("publication_requests_assignee_user_id_idx");

            entity.HasIndex(item => item.CreatedByUserId).HasDatabaseName("publication_requests_created_by_user_id_idx");
            entity.HasIndex(item => item.CreatedAt)
                .HasDatabaseName("publication_requests_created_at_idx")
                .IsDescending();

            // Связи заданы через навигационные свойства: теперь доступны
            // .Include(...) и JOIN в проекции (docs/known-defects.md, D-02).
            entity.HasOne(item => item.Department)
                .WithMany()
                .HasForeignKey(item => item.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(item => item.CreatedByUser)
                .WithMany()
                .HasForeignKey(item => item.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(item => item.AssigneeUser)
                .WithMany()
                .HasForeignKey(item => item.AssigneeUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
