using BuildingBlocks.Application.Results;

namespace Files.Domain.Errors;

public static class FileErrors
{
    public static readonly Error InvalidName =
        new(
            Code: "Files.InvalidName",
            Type: ErrorType.Validation);

    public static readonly Error InvalidContentType =
        new(
            Code: "Files.InvalidContentType",
            Type: ErrorType.Validation);

    public static readonly Error InvalidSize =
        new(
            Code: "Files.InvalidSize",
            Type: ErrorType.Validation);

    public static readonly Error InvalidHash =
        new(
            Code: "Files.InvalidHash",
            Type: ErrorType.Validation);

    public static readonly Error InvalidStorageKey =
        new(
            Code: "Files.InvalidStorageKey",
            Type: ErrorType.Validation);

    public static readonly Error InvalidStatusTransition =
        new(
            Code: "Files.InvalidStatusTransition",
            Type: ErrorType.Conflict);

    public static readonly Error AlreadyDeleted =
        new(
            Code: "Files.AlreadyDeleted",
            Type: ErrorType.Conflict);

    public static readonly Error CannotModifyDeletedFile =
        new(
            Code: "Files.CannotModifyDeletedFile",
            Type: ErrorType.Conflict);

    public static readonly Error HashRequiredForActivation =
        new(
            Code: "Files.HashRequiredForActivation",
            Type: ErrorType.Conflict);

    public static readonly Error StorageKeyRequiredForActivation =
        new(
            Code: "Files.StorageKeyRequiredForActivation",
            Type: ErrorType.Conflict);
}