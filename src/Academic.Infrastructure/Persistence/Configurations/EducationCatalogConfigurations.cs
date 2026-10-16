using Academic.Domain.Courses;
using Academic.Domain.Programs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academic.Infrastructure.Persistence.Configurations;

public sealed class EducationCatalogConfigurations :
    IEntityTypeConfiguration<ProgramProfile>,
    IEntityTypeConfiguration<CourseSyllabus>
{
    public void Configure(EntityTypeBuilder<ProgramProfile> builder)
    {
        builder.ToTable("ProgramProfiles", "academic");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.ProgramId).IsUnique();

        builder.Property(x => x.History).HasMaxLength(4000);
        builder.Property(x => x.AwardedDegree).HasMaxLength(300);
        builder.Property(x => x.DegreeLevelDescription).HasMaxLength(300);
        builder.Property(x => x.AdmissionRequirements).HasMaxLength(4000);
        builder.Property(x => x.RecognitionOfPriorLearning).HasMaxLength(4000);
        builder.Property(x => x.QualificationRequirements).HasMaxLength(4000);
        builder.Property(x => x.ProgramAims).HasMaxLength(4000);
        builder.Property(x => x.OccupationalProfiles).HasMaxLength(4000);
        builder.Property(x => x.AccessToFurtherStudies).HasMaxLength(4000);
        builder.Property(x => x.ExaminationRegulations).HasMaxLength(4000);
        builder.Property(x => x.GraduationRequirements).HasMaxLength(4000);
        builder.Property(x => x.ModeOfStudy).HasMaxLength(100);
        builder.Property(x => x.DepartmentHeadAndContact).HasMaxLength(500);
        builder.Property(x => x.Facilities).HasMaxLength(4000);
        builder.Property(x => x.ProgramEducationalObjectives).HasMaxLength(4000);
        builder.Property(x => x.ProgramOutcomesJson).HasMaxLength(8000);
    }

    public void Configure(EntityTypeBuilder<CourseSyllabus> builder)
    {
        builder.ToTable("CourseSyllabi", "academic");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.CourseCode).IsUnique();

        builder.Property(x => x.CourseCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Language).HasMaxLength(100);
        builder.Property(x => x.Level).HasMaxLength(100);
        builder.Property(x => x.Type).HasMaxLength(100);
        builder.Property(x => x.PrerequisitesText).HasMaxLength(1000);
        builder.Property(x => x.Aim).HasMaxLength(4000);
        builder.Property(x => x.Content).HasMaxLength(4000);
        builder.Property(x => x.TextbooksAndResources).HasMaxLength(4000);
        builder.Property(x => x.PlannedLearningActivities).HasMaxLength(4000);
        builder.Property(x => x.RecommendedOtherTopics).HasMaxLength(2000);
        builder.Property(x => x.InstructorsText).HasMaxLength(1000);
        builder.Property(x => x.LearningOutcomesJson).HasMaxLength(8000);
    }
}
