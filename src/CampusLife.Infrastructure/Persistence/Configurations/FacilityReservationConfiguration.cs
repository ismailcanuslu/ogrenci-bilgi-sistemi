using CampusLife.Domain.Reservations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusLife.Infrastructure.Persistence.Configurations;

public sealed class FacilityReservationConfiguration : IEntityTypeConfiguration<FacilityReservation>
{
    public void Configure(EntityTypeBuilder<FacilityReservation> builder)
    {
        builder.ToTable("facility_reservations", "campus_life");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.FacilityType).IsRequired();
        builder.Property(x => x.FacilityName)
            .HasMaxLength(FacilityReservation.FacilityNameMaxLength)
            .IsRequired();
        builder.Property(x => x.ReservationDate).IsRequired();
        builder.Property(x => x.StartTime).IsRequired();
        builder.Property(x => x.EndTime).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(FacilityReservation.NotesMaxLength);
        builder.Property(x => x.ReviewedByUserId);
        builder.Property(x => x.ReviewNotes).HasMaxLength(FacilityReservation.ReviewNotesMaxLength);
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.ReviewedAtUtc);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => new { x.FacilityName, x.ReservationDate });
        builder.HasIndex(x => new { x.FacilityType, x.Status });
    }
}
