using CampusLife.Domain.Clubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusLife.Infrastructure.Persistence.Configurations;

public sealed class StudentClubConfiguration : IEntityTypeConfiguration<StudentClub>
{
    public void Configure(EntityTypeBuilder<StudentClub> builder)
    {
        builder.ToTable("student_clubs", "campus_life");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).HasMaxLength(StudentClub.NameMaxLength).IsRequired();
        builder.Property(x => x.Code).HasMaxLength(StudentClub.CodeMaxLength).IsRequired();
        builder.Property(x => x.Category).HasMaxLength(StudentClub.CategoryMaxLength);
        builder.Property(x => x.Description).HasMaxLength(StudentClub.DescriptionMaxLength);
        builder.Property(x => x.AdvisorUserId);
        builder.Property(x => x.PresidentStudentId);
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.UpdatedAtUtc);

        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasIndex(x => x.IsActive);
    }
}
