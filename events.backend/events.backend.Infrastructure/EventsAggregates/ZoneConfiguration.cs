using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using events.backend.Domain.EventsAggregates;

namespace events.backend.Infrastructure.EventsAggregates;

internal sealed class ZoneConfiguration : IEntityTypeConfiguration<Zone>
{
    public void Configure(EntityTypeBuilder<Zone> builder)
    {
        builder.ToTable("EVENT_ZONES").HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasColumnName("ZONE_ID");

        builder.Property(x => x.EventId)
            .HasColumnName("EVENT_ID");

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsUnicode(false)
            .HasColumnName("NAME");

        builder.Property(x => x.Price)
            .HasColumnType("decimal(18,2)")
            .HasColumnName("PRICE");

        builder.Property(x => x.Capacity)
            .HasColumnName("CAPACITY");

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

        builder.HasIndex(x => x.EventId);
        builder.HasIndex(x => new { x.EventId, x.Name }).IsUnique();
    }
}
