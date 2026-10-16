using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Persistence;
using BuildingBlocks.Application.Results;
using Files.Application.Abstractions.Persistence;
using Files.Application.Abstractions.Security;
using Files.Domain.Files;
using FluentValidation;

namespace Files.Application.Features;

public static class DeleteFile
{
    private const string NotFoundCode = "Files.NotFound";

    public sealed record Command(Guid Id, Guid? UserId = null) : ICommand<ServiceResult>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }

    public sealed class Handler(
        IFileRepository fileRepository,
        IFileAuthorizationService authorizationService,
        IUnitOfWork unitOfWork)
        : ICommandHandler<Command, ServiceResult>
    {
        public async Task<ServiceResult> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var file = await fileRepository.GetByIdAsync(
                new FileId(request.Id),
                cancellationToken);

            if (file is null)
            {
                return ServiceResult.NotFound(
                    new Error(NotFoundCode, ErrorType.NotFound));
            }

            var canDelete = await authorizationService.CanDeleteAsync(
                file,
                request.UserId,
                cancellationToken);

            if (!canDelete)
            {
                return ServiceResult.Forbidden(
                    new Error("Files.AccessDenied", ErrorType.Forbidden));
            }

            var deleteResult = file.Delete(DateTimeOffset.UtcNow);
            if (!deleteResult.IsSuccess)
            {
                return ServiceResult.Conflict(
                    deleteResult.Errors.ToArray());
            }

            fileRepository.Update(file);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult.NoContent();
        }
    }
}
