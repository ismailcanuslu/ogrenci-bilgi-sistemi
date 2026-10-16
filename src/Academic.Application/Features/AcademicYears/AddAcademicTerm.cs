using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.AcademicYears;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;

namespace Academic.Application.Features.AcademicYears;

public static class AddAcademicTerm
{
    public sealed record Command(
        Guid AcademicYearId,
        TermType TermType,
        string Name,
        DateOnly StartDate,
        DateOnly EndDate,
        bool IsRegistrationOpen)
        : ICommand<ServiceResult<AcademicTermDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.AcademicYearId).NotEmpty();
            RuleFor(x => x.TermType).IsInEnum();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(AcademicTerm.NameMaxLength);
            RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate);
        }
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

            var year = await repository.GetByIdAsync(request.AcademicYearId, cancellationToken);
            if (year is null)
            {
                return ServiceResult<AcademicTermDto>.NotFound(AcademicErrors.AcademicYear.NotFound());
            }

            if (!year.IsActive)
            {
                return ServiceResult<AcademicTermDto>.Conflict(AcademicErrors.AcademicYear.Inactive);
            }

            if (year.Terms.Any(t => t.TermType == request.TermType && t.IsActive))
            {
                return ServiceResult<AcademicTermDto>.Conflict(AcademicErrors.AcademicTerm.TermTypeAlreadyExistsInYear);
            }

            var term = year.AddTerm(
                request.TermType,
                request.Name,
                request.StartDate,
                request.EndDate,
                request.IsRegistrationOpen);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            await auditService.RecordAsync(
                "AcademicTerm.Created", "Academic", "AcademicTerm", term.Id.ToString(),
                newValues: new { term.AcademicYearId, term.TermType, term.Name, term.IsRegistrationOpen },
                cancellationToken: cancellationToken);

            return ServiceResult<AcademicTermDto>.Created(AcademicTermDto.From(term));
        }
    }
}
