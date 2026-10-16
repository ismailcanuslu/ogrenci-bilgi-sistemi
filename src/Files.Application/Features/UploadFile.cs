using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Persistence;
using BuildingBlocks.Application.Results;
using Files.Application.Abstractions.Persistence;
using Files.Application.Abstractions.Security;
using Files.Application.Abstractions.Storage;
using Files.Application.Abstractions.Validation;
using Files.Domain.Errors;
using Files.Domain.Files;
using Files.Domain.ValueObjects;
using FluentValidation;
using File = Files.Domain.Files.File;

namespace Files.Application.Features;

public static class UploadFile
{
    public sealed record Command(
        string FileName,
        string ContentType,
        long FileSize,
        Stream ContentStream,
        Guid? OwnerId = null)
        : ICommand<ServiceResult<Response>>;

    public sealed record Response(
        Guid Id,
        string FileName,
        string ContentType,
        long FileSize,
        string Hash,
        string Status,
        DateTimeOffset CreatedAt,
        Guid? OwnerId = null);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.FileName)
                .NotEmpty()
                .MaximumLength(255);

            RuleFor(x => x.ContentType)
                .NotEmpty()
                .MaximumLength(255);

            RuleFor(x => x.FileSize)
                .GreaterThan(0);

            RuleFor(x => x.ContentStream)
                .NotNull();
        }
    }

    public sealed class Handler(
        IFileRepository fileRepository,
        IFileStorage fileStorage,
        IFileValidator fileValidator,
        IFileHashCalculator hashCalculator,
        IFileAuthorizationService authorizationService,
        IUnitOfWork unitOfWork)
        : ICommandHandler<Command, ServiceResult<Response>>
    {
        public async Task<ServiceResult<Response>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var canUpload = await authorizationService.CanUploadAsync(
                request.OwnerId,
                cancellationToken);

            if (!canUpload)
            {
                return ServiceResult<Response>.Forbidden(
                    new Error("Files.UnauthorizedUpload", ErrorType.Forbidden));
            }

            var fileNameResult = FileName.Create(request.FileName);
            if (!fileNameResult.IsSuccess)
            {
                return ServiceResult<Response>.BadRequest(
                    fileNameResult.Errors.ToArray());
            }

            var contentTypeResult = ContentType.Create(request.ContentType);
            if (!contentTypeResult.IsSuccess)
            {
                return ServiceResult<Response>.BadRequest(
                    contentTypeResult.Errors.ToArray());
            }

            var fileSizeResult = FileSize.Create(request.FileSize);
            if (!fileSizeResult.IsSuccess)
            {
                return ServiceResult<Response>.BadRequest(
                    fileSizeResult.Errors.ToArray());
            }

            var validationResult = await fileValidator.ValidateAsync(
                request.ContentStream,
                fileNameResult.Value!,
                contentTypeResult.Value!,
                fileSizeResult.Value,
                cancellationToken);

            if (!validationResult.IsSuccess)
            {
                return ServiceResult<Response>.BadRequest(
                    validationResult.Errors.ToArray());
            }

            var hash = await hashCalculator.ComputeHashAsync(
                request.ContentStream,
                HashAlgorithm.Sha256,
                cancellationToken);

            if (request.ContentStream.CanSeek)
            {
                request.ContentStream.Position = 0;
            }

            var storageKey = await fileStorage.UploadAsync(
                request.ContentStream,
                fileNameResult.Value!,
                contentTypeResult.Value!,
                cancellationToken);

            var nowUtc = DateTimeOffset.UtcNow;
            var file = File.Create(
                fileNameResult.Value!,
                contentTypeResult.Value!,
                fileSizeResult.Value,
                nowUtc,
                request.OwnerId);

            var setHashResult = file.SetHash(hash);
            if (!setHashResult.IsSuccess)
            {
                return ServiceResult<Response>.Conflict(
                    setHashResult.Errors.ToArray());
            }

            var setKeyResult = file.SetStorageKey(storageKey);
            if (!setKeyResult.IsSuccess)
            {
                return ServiceResult<Response>.Conflict(
                    setKeyResult.Errors.ToArray());
            }

            var activateResult = file.Activate();
            if (!activateResult.IsSuccess)
            {
                return ServiceResult<Response>.Conflict(
                    activateResult.Errors.ToArray());
            }

            await fileRepository.AddAsync(file, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult<Response>.Created(
                new Response(
                    file.Id.Value,
                    file.Name.Value,
                    file.ContentType.Value,
                    file.Size.Bytes,
                    file.Hash!.Value,
                    file.Status.ToString(),
                    file.CreatedAt,
                    file.OwnerId));
        }
    }
}
