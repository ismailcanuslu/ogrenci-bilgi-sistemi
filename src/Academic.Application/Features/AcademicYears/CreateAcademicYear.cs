using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.AcademicYears;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;

namespace Academic.Application.Features.AcademicYears;

public static class CreateAcademicYear
{
    public sealed record Command(
        string Code,
        string Name,
        DateOnly StartDate,
        DateOnly EndDate)
        : ICommand<ServiceResult<AcademicYearDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(AcademicYear.CodeMaxLength);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(AcademicYear.NameMaxLength);
            RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate);
        }
    }

    public sealed class Handler(
        AcademicAccess access,
        IAcademicYearRepository repository,
        IAcademicUnitOfWork unitOfWork,
        IAuditService auditService)
        : ICommandHandler<Command, ServiceResult<AcademicYearDto>>
    {
        public async Task<ServiceResult<AcademicYearDto>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var denied = await access.CheckAcademicTermManageAsync(cancellationToken);
            if (denied is not null)
            {
                return AuthorizationFailure.To<AcademicYearDto>(denied);
            }

            var normalizedCode = AcademicYear.NormalizeCode(request.Code);
            if (await repository.CodeExistsAsync(normalizedCode, null, cancellationToken))
            {
                return ServiceResult<AcademicYearDto>.Conflict(AcademicErrors.AcademicYear.CodeAlreadyExists);
            }

            var year = AcademicYear.Create(
                request.Code,
                request.Name,
                request.StartDate,
                request.EndDate,
                DateTime.UtcNow);

            await repository.AddAsync(year, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await auditService.RecordAsync(
                "AcademicYear.Created", "Academic", "AcademicYear", year.Id.ToString(),
                newValues: new { year.Code, year.Name, year.StartDate, year.EndDate },
                cancellationToken: cancellationToken);

            return ServiceResult<AcademicYearDto>.Created(AcademicYearDto.From(year));
        }
    }
}
