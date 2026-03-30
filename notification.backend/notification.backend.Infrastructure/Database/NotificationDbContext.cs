using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using notification.backend.Domain.Models;
using notification.backend.Infrastructure.Database.Configurations;

namespace notification.backend.Infrastructure.Database;

public class NotificationDbContext(DbContextOptions<NotificationDbContext> options) : DbContext(options)
{
    public DbSet<ProcessedMessage> ProcessedMessages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ProcessedMessageConfiguration());
    }
}
