using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Results;
using Files.Application.Abstractions.Persistence;
using Files.Application.Abstractions.Security;
using Files.Domain.Files;
using FluentValidation;

namespace Files.Application.Features;

public static class GetFileMetadata
{
    private const string NotFoundCode = "Files.NotFound";

    public sealed record Query(Guid Id, Guid? UserId = null) : IQuery<ServiceResult<Response>>;

    public sealed record Response(
        Guid Id,
        string FileName,
        string ContentType,
        long FileSize,
        string? Hash,
        string Status,
        DateTimeOffset CreatedAt,
        DateTimeOffset? DeletedAt,
        Guid? OwnerId = null);

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

            if (file is null || file.Status is FileStatus.Deleted)
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

            return ServiceResult<Response>.Ok(
                new Response(
                    file.Id.Value,
                    file.Name.Value,
                    file.ContentType.Value,
                    file.Size.Bytes,
                    file.Hash?.Value,
                    file.Status.ToString(),
                    file.CreatedAt,
                    file.DeletedAt,
                    file.OwnerId));
        }
    }
}
