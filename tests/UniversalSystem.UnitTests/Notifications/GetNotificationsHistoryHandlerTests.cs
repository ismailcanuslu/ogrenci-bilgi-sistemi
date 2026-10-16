using Notifications.Application.Abstractions.Persistence;
using Notifications.Application.Features;
using Notifications.Domain.Notifications;
using Notifications.Domain.ValueObjects;
using NSubstitute;
using Xunit;

namespace UniversalSystem.UnitTests.Notifications;

public class GetNotificationsHistoryHandlerTests
{
    private readonly INotificationRepository _repository = Substitute.For<INotificationRepository>();

    [Fact]
    public async Task Handle_ShouldReturnPagedListAndMapToDto()
    {
        // Arrange
        var n1 = Notification.CreateEmail(
            EmailAddress.Create("student1@uni.edu").Value!,
            Subject.Create("Konu 1").Value!,
            Body.Create("Mesaj 1", false).Value!,
            DateTimeOffset.UtcNow);

        var n2 = Notification.CreateSms(
            PhoneNumber.Create("+905551234567").Value!,
            Body.Create("OTP: 123456", false).Value!,
            DateTimeOffset.UtcNow);

        var items = new List<Notification> { n1, n2 };

        _repository.GetPagedAsync(
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string?>(),
            Arg.Any<NotificationStatus?>(),
            Arg.Any<CancellationToken>())
            .Returns((items, 2));

        var handler = new GetNotificationsHistory.Handler(_repository);
        var query = new GetNotificationsHistory.Query(Page: 1, PageSize: 10);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(2, result.Data!.TotalCount);
        Assert.Equal(2, result.Data.Items.Count);
        Assert.Equal("Email", result.Data.Items[0].Type);
        Assert.Equal("student1@uni.edu", result.Data.Items[0].Recipient);
        Assert.Equal("Sms", result.Data.Items[1].Type);
        Assert.Equal("+905551234567", result.Data.Items[1].Recipient);
    }

    [Fact]
    public async Task Handle_WithFilters_ShouldPassParsedFiltersToRepository()
    {
        // Arrange
        var items = new List<Notification>();

        _repository.GetPagedAsync(
            1,
            20,
            "student",
            NotificationStatus.Sent,
            Arg.Any<CancellationToken>())
            .Returns((items, 0));

        var handler = new GetNotificationsHistory.Handler(_repository);
        var query = new GetNotificationsHistory.Query(
            Page: 1,
            PageSize: 20,
            Recipient: "student",
            Status: "Sent");

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(200, result.StatusCode);

        await _repository.Received(1).GetPagedAsync(
            1,
            20,
            "student",
            NotificationStatus.Sent,
            Arg.Any<CancellationToken>());
    }
}
