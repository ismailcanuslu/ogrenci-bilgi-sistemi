using Notifications.Domain.Notifications;
using Notifications.Domain.ValueObjects;
using Xunit;

namespace UniversalSystem.UnitTests.Notifications;

public class NotificationDomainTests
{
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("student.test@university.edu.tr")]
    [InlineData("ADMIN@DOMAIN.COM")]
    public void EmailAddress_WithValidAddress_ShouldSucceed(string input)
    {
        // Act
        var result = EmailAddress.Create(input);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(input.Trim().ToLowerInvariant(), result.Value!.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("notanemail")]
    [InlineData("@missinguser.com")]
    [InlineData("missingdomain@")]
    public void EmailAddress_WithInvalidAddress_ShouldFail(string input)
    {
        // Act
        var result = EmailAddress.Create(input);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Theory]
    [InlineData("+905551234567")]
    [InlineData("+14155552671")]
    [InlineData("905551234567")]
    public void PhoneNumber_WithValidNumber_ShouldSucceed(string input)
    {
        // Act
        var result = PhoneNumber.Create(input);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("abc-def")]
    public void PhoneNumber_WithInvalidNumber_ShouldFail(string input)
    {
        // Act
        var result = PhoneNumber.Create(input);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Notification_CreateEmail_ShouldInitializePendingStatus()
    {
        // Arrange
        var email = EmailAddress.Create("student@uni.edu").Value!;
        var subject = Subject.Create("Ders Kayıt Onayı").Value!;
        var body = Body.Create("Kayıt işleminiz tamamlandı.", isHtml: false).Value!;
        var now = DateTimeOffset.UtcNow;

        // Act
        var notification = Notification.CreateEmail(email, subject, body, now);

        // Assert
        Assert.NotEqual(Guid.Empty, notification.Id.Value);
        Assert.Equal(NotificationType.Email, notification.Type);
        Assert.Equal(NotificationStatus.Pending, notification.Status);
        Assert.Equal(email.Value, notification.Recipient);
        Assert.Equal(subject.Value, notification.Subject!.Value);
        Assert.Equal(body.Value, notification.Body.Value);
        Assert.False(notification.Body.IsHtml);
        Assert.Null(notification.SentAt);
    }

    [Fact]
    public void Notification_MarkAsSent_ShouldUpdateStatusAndSentAt()
    {
        // Arrange
        var email = EmailAddress.Create("student@uni.edu").Value!;
        var subject = Subject.Create("Başlık").Value!;
        var body = Body.Create("İçerik").Value!;
        var notification = Notification.CreateEmail(email, subject, body, DateTimeOffset.UtcNow);
        var sentAt = DateTimeOffset.UtcNow;

        // Act
        var result = notification.MarkAsSent(sentAt);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(NotificationStatus.Sent, notification.Status);
        Assert.Equal(sentAt, notification.SentAt);
    }

    [Fact]
    public void Notification_MarkAsSent_WhenAlreadySent_ShouldFail()
    {
        // Arrange
        var email = EmailAddress.Create("student@uni.edu").Value!;
        var subject = Subject.Create("Başlık").Value!;
        var body = Body.Create("İçerik").Value!;
        var notification = Notification.CreateEmail(email, subject, body, DateTimeOffset.UtcNow);
        notification.MarkAsSent(DateTimeOffset.UtcNow);

        // Act
        var result = notification.MarkAsSent(DateTimeOffset.UtcNow);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Notification_MarkAsFailed_ShouldUpdateStatusAndReason()
    {
        // Arrange
        var email = EmailAddress.Create("student@uni.edu").Value!;
        var subject = Subject.Create("Başlık").Value!;
        var body = Body.Create("İçerik").Value!;
        var notification = Notification.CreateEmail(email, subject, body, DateTimeOffset.UtcNow);

        // Act
        var result = notification.MarkAsFailed("SMTP sunucusuna bağlanılamadı.");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(NotificationStatus.Failed, notification.Status);
        Assert.Equal("SMTP sunucusuna bağlanılamadı.", notification.FailureReason);
    }
}
