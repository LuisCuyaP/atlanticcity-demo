using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using events.backend.Domain.EventsAggregates;

namespace events.backend.Infrastructure.EventsAggregates;

internal sealed class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("EVENTS").HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasColumnName("EVENT_ID");

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsUnicode(false)
            .HasColumnName("NAME");

        builder.Property(x => x.EventDate)
            .HasColumnType("datetime")
            .HasColumnName("EVENT_DATE");

        builder.Property(x => x.Place)
            .HasMaxLength(200)
            .IsUnicode(false)
            .HasColumnName("PLACE");

        builder.Property(x => x.Status)
            .HasMaxLength(20)
            .IsUnicode(false)
            .HasColumnName("STATUS");

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(50)
            .IsUnicode(false)
            .HasColumnName("CREATED_BY");

        builder.Property(x => x.CreatedDate)
            .HasColumnType("datetime")
            .HasColumnName("CREATED_DATE");

        builder.Property(x => x.ModifiedBy)
            .HasMaxLength(50)
            .IsUnicode(false)
            .HasColumnName("MODIFIED_BY");

        builder.Property(x => x.ModifiedDate)
            .HasColumnType("datetime")
            .HasColumnName("MODIFIED_DATE");

        builder.HasMany(x => x.Zones)
            .WithOne()
            .HasForeignKey(z => z.EventId);
    }
}
