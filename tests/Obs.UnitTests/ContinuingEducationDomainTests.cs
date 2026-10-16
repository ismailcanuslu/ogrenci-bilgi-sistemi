using ContinuingEducation.Domain.Programs;
using Xunit;

namespace UniversalSystem.UnitTests;

public class ContinuingEducationDomainTests
{
    [Fact]
    public void Create_EducationProgram_ShouldInitializeCorrectly()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var startDate = DateTime.UtcNow.AddDays(7);
        var endDate = startDate.AddDays(30);

        // Act
        var program = EducationProgram.Create(
            "C# ile İleri Seviye Mikroservis Mimarisi",
            "SEM-CS-001",
            "Modern mimari kursu",
            instructorId,
            30,
            startDate,
            endDate,
            40,
            1500m,
            false,
            80,
            70,
            DateTime.UtcNow);

        // Assert
        Assert.NotNull(program);
        Assert.Equal("C# ile İleri Seviye Mikroservis Mimarisi", program.Title);
        Assert.Equal("SEM-CS-001", program.Code);
        Assert.Equal(30, program.Capacity);
        Assert.Equal(40, program.TotalHours);
        Assert.Equal(80, program.MinAttendancePercentage);
        Assert.Equal(70, program.MinPassingGrade);
        Assert.Equal(CourseProgramStatus.Active, program.Status);
    }

    [Fact]
    public void FinalizeEvaluation_WhenAttendanceAndGradeMeetCriteria_ShouldEarnCertificate()
    {
        // Arrange
        var programId = Guid.NewGuid();
        var applicantId = Guid.NewGuid();
        var application = ProgramApplication.Create(programId, applicantId, DateTime.UtcNow);
        application.Approve(DateTime.UtcNow);

        // Toplam 40 saatlik kurs. Kursiyer 35 saat katıldı (87.5% > 80%). Not: 85 (> 70)
        application.AddAttendedHours(35);

        // Act
        application.FinalizeEvaluation(85m, 40, 80, 70, DateTime.UtcNow);

        // Assert
        Assert.Equal(ApplicationStatus.Completed, application.Status);
        Assert.True(application.IsCertificateEarned);
        Assert.False(string.IsNullOrWhiteSpace(application.CertificateNumber));
        Assert.StartsWith("SEM-", application.CertificateNumber);
        Assert.Equal(85m, application.FinalGrade);
    }

    [Fact]
    public void FinalizeEvaluation_WhenAttendanceIsInsufficient_ShouldFailAndNotEarnCertificate()
    {
        // Arrange
        var programId = Guid.NewGuid();
        var applicantId = Guid.NewGuid();
        var application = ProgramApplication.Create(programId, applicantId, DateTime.UtcNow);
        application.Approve(DateTime.UtcNow);

        // Toplam 40 saatlik kurs. Kursiyer 20 saat katıldı (50% < 80%). Not: 95 (> 70)
        application.AddAttendedHours(20);

        // Act
        application.FinalizeEvaluation(95m, 40, 80, 70, DateTime.UtcNow);

        // Assert
        Assert.Equal(ApplicationStatus.Failed, application.Status);
        Assert.False(application.IsCertificateEarned);
        Assert.Null(application.CertificateNumber);
    }
}
