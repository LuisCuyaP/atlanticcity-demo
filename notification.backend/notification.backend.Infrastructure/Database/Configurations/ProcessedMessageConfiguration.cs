using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using notification.backend.Domain.Models;

namespace notification.backend.Infrastructure.Database.Configurations;

internal sealed class ProcessedMessageConfiguration : IEntityTypeConfiguration<ProcessedMessage>
{
    public void Configure(EntityTypeBuilder<ProcessedMessage> builder)
    {
        builder.ToTable("PROCESSED_MESSAGES").HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasColumnName("ID");

        builder.Property(x => x.MessageId)
            .HasColumnName("MESSAGE_ID")
            .IsRequired();

        builder.Property(x => x.EventType)
            .HasMaxLength(100)
            .IsUnicode(false)
            .HasColumnName("EVENT_TYPE")
            .IsRequired();

        builder.Property(x => x.ProcessedAt)
            .HasColumnType("datetime")
            .HasColumnName("PROCESSED_AT")
            .IsRequired();

        // Índice único para deduplicación (messageId + eventType)
        builder.HasIndex(x => new { x.MessageId, x.EventType })
            .IsUnique()
            .HasDatabaseName("UX_PROCESSED_MESSAGES_MSG_TYPE");
    }
}
