using Academic.Domain.AcademicCalendars;
using Academic.Domain.Courses;
using Academic.Domain.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academic.Infrastructure.Persistence.Configurations;

public sealed class StudentTuitionConfiguration : IEntityTypeConfiguration<StudentTuition>
{
    public void Configure(EntityTypeBuilder<StudentTuition> builder)
    {
        builder.ToTable("StudentTuitions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TermName).IsRequired().HasMaxLength(StudentTuition.TermNameMaxLength);
        builder.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.PaidAmount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Notes).HasMaxLength(StudentTuition.NotesMaxLength);
        builder.HasIndex(x => new { x.StudentId, x.TermName }).IsUnique();
    }
}

public sealed class StudentMilitaryStatusConfiguration : IEntityTypeConfiguration<StudentMilitaryStatus>
{
    public void Configure(EntityTypeBuilder<StudentMilitaryStatus> builder)
    {
        builder.ToTable("StudentMilitaryStatuses");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.StatusCode).IsRequired().HasMaxLength(StudentMilitaryStatus.StatusCodeMaxLength);
        builder.Property(x => x.AsalDocumentNo).HasMaxLength(StudentMilitaryStatus.DocumentNoMaxLength);
        builder.Property(x => x.Notes).HasMaxLength(StudentMilitaryStatus.NotesMaxLength);
        builder.HasIndex(x => x.StudentId).IsUnique();
    }
}

public sealed class StudentDisciplinaryRecordConfiguration : IEntityTypeConfiguration<StudentDisciplinaryRecord>
{
    public void Configure(EntityTypeBuilder<StudentDisciplinaryRecord> builder)
    {
        builder.ToTable("StudentDisciplinaryRecords");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PenaltyType).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.Reason).IsRequired().HasMaxLength(StudentDisciplinaryRecord.ReasonMaxLength);
        builder.Property(x => x.DecisionNumber).HasMaxLength(StudentDisciplinaryRecord.DecisionNumberMaxLength);
        builder.HasIndex(x => x.StudentId);
    }
}

public sealed class StudentBankDetailsConfiguration : IEntityTypeConfiguration<StudentBankDetails>
{
    public void Configure(EntityTypeBuilder<StudentBankDetails> builder)
    {
        builder.ToTable("StudentBankDetails");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.BankName).IsRequired().HasMaxLength(StudentBankDetails.BankNameMaxLength);
        builder.Property(x => x.Iban).IsRequired().HasMaxLength(StudentBankDetails.IbanMaxLength);
        builder.Property(x => x.AccountHolderName).IsRequired().HasMaxLength(StudentBankDetails.AccountHolderMaxLength);
        builder.HasIndex(x => x.StudentId).IsUnique();
    }
}

public sealed class StudentStudyExtensionConfiguration : IEntityTypeConfiguration<StudentStudyExtension>
{
    public void Configure(EntityTypeBuilder<StudentStudyExtension> builder)
    {
        builder.ToTable("StudentStudyExtensions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Notes).HasMaxLength(StudentStudyExtension.NotesMaxLength);
        builder.HasIndex(x => x.StudentId).IsUnique();
    }
}

public sealed class StudentHistoryRecordConfiguration : IEntityTypeConfiguration<StudentHistoryRecord>
{
    public void Configure(EntityTypeBuilder<StudentHistoryRecord> builder)
    {
        builder.ToTable("StudentHistoryRecords");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EventTitle).IsRequired().HasMaxLength(StudentHistoryRecord.TitleMaxLength);
        builder.Property(x => x.EventDescription).IsRequired().HasMaxLength(StudentHistoryRecord.DescriptionMaxLength);
        builder.Property(x => x.ActionBy).HasMaxLength(StudentHistoryRecord.ActionByMaxLength);
        builder.HasIndex(x => x.StudentId);
    }
}

public sealed class StudentProfilePictureConfiguration : IEntityTypeConfiguration<StudentProfilePicture>
{
    public void Configure(EntityTypeBuilder<StudentProfilePicture> builder)
    {
        builder.ToTable("StudentProfilePictures");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ImageUrl).IsRequired().HasMaxLength(StudentProfilePicture.ImageUrlMaxLength);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.RejectionReason).HasMaxLength(StudentProfilePicture.RejectionReasonMaxLength);
        builder.HasIndex(x => x.StudentId).IsUnique();
    }
}

public sealed class StudentAlumniInfoConfiguration : IEntityTypeConfiguration<StudentAlumniInfo>
{
    public void Configure(EntityTypeBuilder<StudentAlumniInfo> builder)
    {
        builder.ToTable("StudentAlumniInfos");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CurrentCompany).HasMaxLength(StudentAlumniInfo.CompanyMaxLength);
        builder.Property(x => x.CurrentPosition).HasMaxLength(StudentAlumniInfo.PositionMaxLength);
        builder.Property(x => x.LinkedInUrl).HasMaxLength(StudentAlumniInfo.UrlMaxLength);
        builder.Property(x => x.PersonalEmail).HasMaxLength(StudentAlumniInfo.EmailMaxLength);
        builder.Property(x => x.PhoneNumber).HasMaxLength(StudentAlumniInfo.PhoneMaxLength);
        builder.HasIndex(x => x.StudentId).IsUnique();
    }
}

public sealed class StudentAdvisorAssignmentConfiguration : IEntityTypeConfiguration<StudentAdvisorAssignment>
{
    public void Configure(EntityTypeBuilder<StudentAdvisorAssignment> builder)
    {
        builder.ToTable("StudentAdvisorAssignments");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.StudentId, x.IsActive });
    }
}

public sealed class CourseOfferingLiveSessionConfiguration : IEntityTypeConfiguration<CourseOfferingLiveSession>
{
    public void Configure(EntityTypeBuilder<CourseOfferingLiveSession> builder)
    {
        builder.ToTable("CourseOfferingLiveSessions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(CourseOfferingLiveSession.TitleMaxLength);
        builder.Property(x => x.SessionType).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.Platform).IsRequired().HasMaxLength(CourseOfferingLiveSession.PlatformMaxLength);
        builder.Property(x => x.MeetingUrl).IsRequired().HasMaxLength(CourseOfferingLiveSession.UrlMaxLength);
        builder.Property(x => x.AccessPassword).HasMaxLength(CourseOfferingLiveSession.PasswordMaxLength);
        builder.HasIndex(x => x.CourseOfferingId);
    }
}

public sealed class AcademicCalendarEventConfiguration : IEntityTypeConfiguration<AcademicCalendarEvent>
{
    public void Configure(EntityTypeBuilder<AcademicCalendarEvent> builder)
    {
        builder.ToTable("AcademicCalendarEvents");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(AcademicCalendarEvent.TitleMaxLength);
        builder.Property(x => x.AcademicYear).IsRequired().HasMaxLength(AcademicCalendarEvent.AcademicYearMaxLength);
        builder.Property(x => x.Semester).IsRequired().HasMaxLength(30);
    }
}
