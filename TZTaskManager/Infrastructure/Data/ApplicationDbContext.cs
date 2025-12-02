using Microsoft.EntityFrameworkCore;
using TZTaskManager.Domain.Entities;
using TaskEntity = TZTaskManager.Domain.Entities.Task;

namespace TZTaskManager.Infrastructure.Data
{
    /// <summary>
    /// Контекст базы данных приложения
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<TaskType> TaskTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaskType>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(e => e.Description)
                    .HasMaxLength(1000);
                entity.HasIndex(e => e.Name)
                    .IsUnique();
            });

            modelBuilder.Entity<TaskEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(500);
                entity.Property(e => e.Description)
                    .HasMaxLength(2000);
                entity.Property(e => e.Status)
                    .HasConversion<int>();

                entity.HasOne(e => e.TaskType)
                    .WithMany(t => t.Tasks)
                    .HasForeignKey(e => e.TaskTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TaskType>().HasData(
                new TaskType { Id = 1, Name = "Работа", Description = "Рабочие задачи", CreatedAt = DateTime.UtcNow },
                new TaskType { Id = 2, Name = "Личное", Description = "Личные задачи", CreatedAt = DateTime.UtcNow },
                new TaskType { Id = 3, Name = "Учеба", Description = "Учебные задачи", CreatedAt = DateTime.UtcNow }
            );
        }
    }
}

