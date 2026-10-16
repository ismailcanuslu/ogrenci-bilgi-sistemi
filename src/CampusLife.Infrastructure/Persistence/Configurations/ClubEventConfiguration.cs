using CampusLife.Domain.Clubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusLife.Infrastructure.Persistence.Configurations;

public sealed class ClubEventConfiguration : IEntityTypeConfiguration<ClubEvent>
{
    public void Configure(EntityTypeBuilder<ClubEvent> builder)
    {
        builder.ToTable("club_events", "campus_life");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ClubId).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(ClubEvent.TitleMaxLength).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(ClubEvent.DescriptionMaxLength);
        builder.Property(x => x.EventDateUtc).IsRequired();
        builder.Property(x => x.Location).HasMaxLength(ClubEvent.LocationMaxLength).IsRequired();
        builder.Property(x => x.Capacity).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.ReviewedByUserId);
        builder.Property(x => x.ReviewNotes).HasMaxLength(ClubEvent.ReviewNotesMaxLength);
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.ReviewedAtUtc);

        builder.HasIndex(x => x.ClubId);
        builder.HasIndex(x => x.EventDateUtc);
        builder.HasIndex(x => x.Status);

        builder.HasOne<StudentClub>()
            .WithMany()
            .HasForeignKey(x => x.ClubId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
