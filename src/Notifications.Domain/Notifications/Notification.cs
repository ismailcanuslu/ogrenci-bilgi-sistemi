using BuildingBlocks.Application.Results;
using BuildingBlocks.Domain.Primitives;
using Notifications.Domain.Errors;
using Notifications.Domain.ValueObjects;

namespace Notifications.Domain.Notifications;

public sealed class Notification : Entity<NotificationId>
{
    private Notification(
        NotificationId id,
        NotificationType type,
        string recipient,
        Subject? subject,
        Body body,
        DateTimeOffset createdAt,
        string? templateCode = null,
        string? templateDataJson = null)
        : base(id)
    {
        Type = type;
        Recipient = recipient;
        Subject = subject;
        Body = body;
        Status = NotificationStatus.Pending;
        CreatedAt = createdAt;
        TemplateCode = templateCode;
        TemplateDataJson = templateDataJson;
        RetryCount = 0;
    }

    private Notification()
    {
    }

    public NotificationType Type { get; private set; }

    public NotificationStatus Status { get; private set; }

    public string Recipient { get; private set; } = default!;

    public Subject? Subject { get; private set; }

    public Body Body { get; private set; } = default!;

    public string? TemplateCode { get; private set; }

    public string? TemplateDataJson { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? SentAt { get; private set; }

    public string? FailureReason { get; private set; }

    public int RetryCount { get; private set; }

    public static Notification CreateEmail(
        EmailAddress recipient,
        Subject subject,
        Body body,
        DateTimeOffset createdAt)
    {
        ArgumentNullException.ThrowIfNull(recipient);
        ArgumentNullException.ThrowIfNull(subject);
        ArgumentNullException.ThrowIfNull(body);

        return new Notification(
            NotificationId.New(),
            NotificationType.Email,
            recipient.Value,
            subject,
            body,
            createdAt);
    }

    public static Notification CreateSms(
        PhoneNumber recipient,
        Body body,
        DateTimeOffset createdAt)
    {
        ArgumentNullException.ThrowIfNull(recipient);
        ArgumentNullException.ThrowIfNull(body);

        return new Notification(
            NotificationId.New(),
            NotificationType.Sms,
            recipient.Value,
            null,
            body,
            createdAt);
    }

    public static Notification CreateTemplatedEmail(
        EmailAddress recipient,
        Subject subject,
        string templateCode,
        string? templateDataJson,
        Body renderedBody,
        DateTimeOffset createdAt)
    {
        ArgumentNullException.ThrowIfNull(recipient);
        ArgumentNullException.ThrowIfNull(subject);
        ArgumentNullException.ThrowIfNull(renderedBody);
        ArgumentException.ThrowIfNullOrWhiteSpace(templateCode);

        return new Notification(
            NotificationId.New(),
            NotificationType.Email,
            recipient.Value,
            subject,
            renderedBody,
            createdAt,
            templateCode,
            templateDataJson);
    }

    public Result MarkAsSent(DateTimeOffset sentAt)
    {
        if (Status is NotificationStatus.Sent)
        {
            return Result.Failure(NotificationErrors.AlreadySent);
        }

        if (Status is NotificationStatus.Cancelled)
        {
            return Result.Failure(NotificationErrors.AlreadyCancelled);
        }

        Status = NotificationStatus.Sent;
        SentAt = sentAt;
        FailureReason = null;

        return Result.Success();
    }

    public Result MarkAsFailed(string reason)
    {
        if (Status is NotificationStatus.Sent)
        {
            return Result.Failure(NotificationErrors.AlreadySent);
        }

        if (Status is NotificationStatus.Cancelled)
        {
            return Result.Failure(NotificationErrors.AlreadyCancelled);
        }

        Status = NotificationStatus.Failed;
        FailureReason = reason;

        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status is NotificationStatus.Sent)
        {
            return Result.Failure(NotificationErrors.AlreadySent);
        }

        Status = NotificationStatus.Cancelled;

        return Result.Success();
    }

    public void RecordRetry()
    {
        RetryCount++;
    }
}
