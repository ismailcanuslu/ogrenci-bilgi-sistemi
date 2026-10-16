using BuildingBlocks.Application.Results;

namespace ImportExport.Domain;

public static class ImportErrors
{
    public static readonly Error JobNotFound =
        new(
            Code: "Import.JobNotFound",
            Type: ErrorType.NotFound);

    public static readonly Error UnsupportedFormat =
        new(
            Code: "Import.UnsupportedFormat",
            Type: ErrorType.Validation);

    public static readonly Error EmptyData =
        new(
            Code: "Import.EmptyData",
            Type: ErrorType.Validation);

    public static readonly Error ProcessingFailed =
        new(
            Code: "Import.ProcessingFailed",
            Type: ErrorType.Failure);
}
