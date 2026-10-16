using Academic.Application.Abstractions;
using Academic.Application.Common;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;

namespace Academic.Application.Features.AcademicYears;

public static class SetTermRegistrationStatus
{
    public sealed record Command(Guid TermId, bool IsRegistrationOpen)
        : ICommand<ServiceResult<AcademicTermDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator() => RuleFor(x => x.TermId).NotEmpty();
    }

    public sealed class Handler(
        AcademicAccess access,
        IAcademicYearRepository repository,
        IAcademicUnitOfWork unitOfWork,
        IAuditService auditService)
        : ICommandHandler<Command, ServiceResult<AcademicTermDto>>
    {
        public async Task<ServiceResult<AcademicTermDto>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var denied = await access.CheckAcademicTermManageAsync(cancellationToken);
            if (denied is not null)
            {
                return AuthorizationFailure.To<AcademicTermDto>(denied);
            }

            var term = await repository.GetTermByIdAsync(request.TermId, cancellationToken);
            if (term is null)
            {
                return ServiceResult<AcademicTermDto>.NotFound(AcademicErrors.AcademicTerm.NotFound());
            }

            var wasOpen = term.IsRegistrationOpen;
            term.SetRegistrationStatus(request.IsRegistrationOpen);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            await auditService.RecordAsync(
                request.IsRegistrationOpen ? "AcademicTerm.RegistrationOpened" : "AcademicTerm.RegistrationClosed",
                "Academic", "AcademicTerm", term.Id.ToString(),
                oldValues: new { IsRegistrationOpen = wasOpen },
                newValues: new { term.IsRegistrationOpen },
                cancellationToken: cancellationToken);

            return ServiceResult<AcademicTermDto>.Ok(AcademicTermDto.From(term));
        }
    }
}
