using Academic.Domain.Courses;
using Academic.Domain.Students;
using BuildingBlocks.Domain.Exceptions;
using Xunit;

namespace UniversalSystem.UnitTests;

public class StudentPortalDomainTests
{
    [Fact]
    public void StudentTuition_PaymentCalculation_ShouldReflectRemainingDebt()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var tuition = new StudentTuition(Guid.NewGuid(), studentId, "2026-2027 Güz", 1250m, DateTime.UtcNow.AddMonths(1));

        // Assert 1: Başlangıçta ödenmemiş ve borç tam
        Assert.Equal(1250m, tuition.TotalAmount);
        Assert.Equal(0m, tuition.PaidAmount);
        Assert.Equal(1250m, tuition.RemainingDebt);
        Assert.False(tuition.IsPaid);

        // Act 1: Kısmi ödeme yap
        tuition.RecordPayment(500m);
        Assert.Equal(500m, tuition.PaidAmount);
        Assert.Equal(750m, tuition.RemainingDebt);
        Assert.False(tuition.IsPaid);

        // Act 2: Kalan borcu öde
        tuition.RecordPayment(750m);
        Assert.Equal(1250m, tuition.PaidAmount);
        Assert.Equal(0m, tuition.RemainingDebt);
        Assert.True(tuition.IsPaid);
    }

    [Fact]
    public void StudentBankDetails_Update_ShouldNormalizeIban()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var bank = new StudentBankDetails(
            Guid.NewGuid(),
            studentId,
            "Ziraat Bankası",
            "tr12 0001 0002 0003 0004 0005 06",
            "Ahmet Yılmaz",
            DateTime.UtcNow);

        // Assert
        Assert.Equal("TR120001000200030004000506", bank.Iban);
        Assert.Equal("Ziraat Bankası", bank.BankName);
        Assert.Equal("Ahmet Yılmaz", bank.AccountHolderName);
    }

    [Fact]
    public void StudentStudyExtension_ShouldDetectExtendedPeriodCorrectly()
    {
        // Arrange: 8 dönemlik lisans programında 9. dönemini okuyan öğrenci (görseldeki durum)
        var studentId = Guid.NewGuid();
        var ext = new StudentStudyExtension(
            Guid.NewGuid(),
            studentId,
            8,
            9,
            14,
            true,
            2,
            "Azami süre kapsamında ek süre.");

        // Assert
        Assert.True(ext.IsInExtendedPeriod);
        Assert.Equal(9, ext.CompletedTerms);
        Assert.Equal(5, ext.RemainingTerms);
        Assert.True(ext.HasAdditionalExamRights);
    }

    [Fact]
    public void StudentProfilePicture_ApproveAndReject_ShouldUpdateStatus()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var pic = new StudentProfilePicture(
            Guid.NewGuid(),
            studentId,
            "https://cdn.university.edu/photos/student.jpg",
            DateTime.UtcNow);

        Assert.Equal(ProfilePictureStatus.Pending, pic.Status);

        // Act & Assert 1: Reject
        pic.Reject("Fotoğraf biyometrik standartlara uygun değil.", DateTime.UtcNow);
        Assert.Equal(ProfilePictureStatus.Rejected, pic.Status);
        Assert.NotNull(pic.RejectionReason);

        // Act & Assert 2: Approve
        pic.Approve(DateTime.UtcNow);
        Assert.Equal(ProfilePictureStatus.Approved, pic.Status);
        Assert.Null(pic.RejectionReason);
    }

    [Fact]
    public void CourseOfferingLiveSession_InvalidTimeRange_ShouldThrow()
    {
        // Arrange & Act & Assert
        Assert.Throws<DomainException>(() => new CourseOfferingLiveSession(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Hafta 1 Canlı Ders",
            LiveSessionType.LiveLecture,
            "Zoom",
            "https://zoom.us/j/123",
            null,
            DateTime.UtcNow.AddHours(2),
            DateTime.UtcNow.AddHours(1))); // Bitiş başlangıçtan önce
    }
}
