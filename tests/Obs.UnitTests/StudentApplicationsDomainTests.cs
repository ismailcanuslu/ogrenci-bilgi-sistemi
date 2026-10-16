using BuildingBlocks.Domain.Exceptions;
using StudentApplications.Domain.Applications;
using Xunit;

namespace UniversalSystem.UnitTests;

public class StudentApplicationsDomainTests
{
    [Fact]
    public void Create_DoubleMajorApplication_ShouldInitializeCorrectly()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var targetDepartmentId = Guid.NewGuid();

        // Act
        var app = StudentApplication.Create(
            studentId,
            ApplicationType.DoubleMajor,
            targetDepartmentId,
            null,
            "Bilgisayar Mühendisliği ÇAP başvurusu talebi.",
            "https://cdn.university.edu/transkript.pdf",
            3.65m,
            DateTime.UtcNow);

        // Assert
        Assert.NotNull(app);
        Assert.Equal(studentId, app.StudentId);
        Assert.Equal(ApplicationType.DoubleMajor, app.Type);
        Assert.Equal(ApplicationState.Submitted, app.State);
        Assert.Equal(targetDepartmentId, app.TargetDepartmentId);
        Assert.Equal(3.65m, app.Gpa);
        Assert.StartsWith("APP-", app.ApplicationNumber);
        Assert.Null(app.ReviewerUserId);
        Assert.Null(app.ReviewedAtUtc);
    }

    [Fact]
    public void Create_DoubleMajorApplication_WithoutTargetDepartment_ShouldThrow()
    {
        // Arrange
        var studentId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<DomainException>(() => StudentApplication.Create(
            studentId,
            ApplicationType.DoubleMajor,
            null,
            null,
            "Hedef bölümsüz ÇAP",
            null,
            3.5m,
            DateTime.UtcNow));
    }

    [Fact]
    public void ReviewApplication_WhenApproved_ShouldSetApprovedState()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var targetDepartmentId = Guid.NewGuid();
        var reviewerUserId = Guid.NewGuid();

        var app = StudentApplication.Create(
            studentId,
            ApplicationType.Minor,
            targetDepartmentId,
            null,
            "Yandal başvurusu.",
            null,
            3.20m,
            DateTime.UtcNow);

        // Act
        app.Approve(reviewerUserId, "Kontenjan ve not ortalaması şartı sağlandı.", DateTime.UtcNow);

        // Assert
        Assert.Equal(ApplicationState.Approved, app.State);
        Assert.Equal(reviewerUserId, app.ReviewerUserId);
        Assert.NotNull(app.ReviewedAtUtc);
        Assert.Equal("Kontenjan ve not ortalaması şartı sağlandı.", app.ReviewNotes);
    }

    [Fact]
    public void ReviewApplication_WhenRejectedWithoutReason_ShouldThrow()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var reviewerUserId = Guid.NewGuid();

        var app = StudentApplication.Create(
            studentId,
            ApplicationType.SummerSchool,
            null,
            "İTÜ",
            "Yaz Okulu Diferansiyel Denklemler dersi denklik onayı.",
            null,
            null,
            DateTime.UtcNow);

        // Act & Assert
        Assert.Throws<DomainException>(() => app.Reject(reviewerUserId, "  ", DateTime.UtcNow));
    }

    [Fact]
    public void Cancel_WhenSubmitted_ShouldChangeStateToCancelled()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var app = StudentApplication.Create(
            studentId,
            ApplicationType.DocumentRequest,
            null,
            null,
            "İngilizce Resmi Transkript Talebi",
            null,
            null,
            DateTime.UtcNow);

        // Act
        app.Cancel();

        // Assert
        Assert.Equal(ApplicationState.Cancelled, app.State);
    }
}
