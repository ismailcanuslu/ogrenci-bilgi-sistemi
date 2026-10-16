using Academic.Application.Abstractions;
using Academic.Application.Common;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Results;
using FluentValidation;

namespace Academic.Application.Features.AcademicYears;

public static class GetAcademicYear
{
    public sealed record Query(Guid Id) : IQuery<ServiceResult<AcademicYearDto>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator() => RuleFor(x => x.Id).NotEmpty();
    }

    public sealed class Handler(IAcademicYearRepository repository)
        : IQueryHandler<Query, ServiceResult<AcademicYearDto>>
    {
        public async Task<ServiceResult<AcademicYearDto>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var year = await repository.GetByIdAsync(request.Id, cancellationToken);
            return year is null
                ? ServiceResult<AcademicYearDto>.NotFound(AcademicErrors.AcademicYear.NotFound())
                : ServiceResult<AcademicYearDto>.Ok(AcademicYearDto.From(year));
        }
    }
}
