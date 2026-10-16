using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Results;
using Files.Application.Abstractions.Persistence;
using Files.Application.Abstractions.Security;
using Files.Application.Abstractions.Storage;
using Files.Domain.Files;
using FluentValidation;

namespace Files.Application.Features;

public static class DownloadFile
{
    private const string NotFoundCode = "Files.NotFound";

    public sealed record Query(Guid Id, Guid? UserId = null) : IQuery<ServiceResult<Response>>;

    public sealed record Response(
        string FileName,
        string ContentType,
        Stream ContentStream);

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }

    public sealed class Handler(
        IFileRepository fileRepository,
        IFileStorageReader storageReader,
        IFileAuthorizationService authorizationService)
        : IQueryHandler<Query, ServiceResult<Response>>
    {
        public async Task<ServiceResult<Response>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var file = await fileRepository.GetByIdAsync(
                new FileId(request.Id),
                cancellationToken);

            if (file is null ||
                file.Status is not FileStatus.Active ||
                file.StorageKey is null)
            {
                return ServiceResult<Response>.NotFound(
                    new Error(NotFoundCode, ErrorType.NotFound));
            }

            var canRead = await authorizationService.CanReadAsync(
                file,
                request.UserId,
                cancellationToken);

            if (!canRead)
            {
                return ServiceResult<Response>.Forbidden(
                    new Error("Files.AccessDenied", ErrorType.Forbidden));
            }

            var stream = await storageReader.OpenReadStreamAsync(
                file.StorageKey,
                cancellationToken);

            return ServiceResult<Response>.Ok(
                new Response(
                    file.Name.Value,
                    file.ContentType.Value,
                    stream));
        }
    }
}
