using BuildingBlocks.Application.Results;

namespace Notifications.Domain.Errors;

public static class NotificationErrors
{
    public static readonly Error InvalidEmail =
        new("Notifications.InvalidEmail", ErrorType.Validation);

    public static readonly Error InvalidPhoneNumber =
        new("Notifications.InvalidPhoneNumber", ErrorType.Validation);

    public static readonly Error InvalidSubject =
        new("Notifications.InvalidSubject", ErrorType.Validation);

    public static readonly Error InvalidBody =
        new("Notifications.InvalidBody", ErrorType.Validation);

    public static readonly Error AlreadySent =
        new("Notifications.AlreadySent", ErrorType.Conflict);

    public static readonly Error AlreadyCancelled =
        new("Notifications.AlreadyCancelled", ErrorType.Conflict);

    public static readonly Error NotFound =
        new("Notifications.NotFound", ErrorType.NotFound);

    public static readonly Error SendFailed =
        new("Notifications.SendFailed", ErrorType.Failure);

    public static readonly Error DeliveryFailed =
        new("Notifications.DeliveryFailed", ErrorType.Failure);

    public static readonly Error SmsDeliveryFailed =
        new("Notifications.SmsDeliveryFailed", ErrorType.Failure);

    public static readonly Error RetryFailed =
        new("Notifications.RetryFailed", ErrorType.Failure);
}
