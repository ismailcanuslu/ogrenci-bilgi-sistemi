using Academic.Application.Abstractions;
using Academic.Application.Common;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Results;
using FluentValidation;

namespace Academic.Application.Features.AcademicYears;

public static class ListAcademicYears
{
    public sealed record Query(
        string? Search = null,
        bool? IsActive = null,
        int Page = 1,
        int PageSize = 20)
        : IQuery<ServiceResult<PagedResult<AcademicYearDto>>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, PagingRequest.MaxPageSize);
            RuleFor(x => x.Search).MaximumLength(100);
        }
    }

    public sealed class Handler(IAcademicYearRepository repository)
        : IQueryHandler<Query, ServiceResult<PagedResult<AcademicYearDto>>>
    {
        public async Task<ServiceResult<PagedResult<AcademicYearDto>>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var page = await repository.ListAsync(
                new AcademicYearListCriteria(request.Search?.Trim(), request.IsActive, request.Page, request.PageSize),
                cancellationToken);

            return ServiceResult<PagedResult<AcademicYearDto>>.Ok(
                new PagedResult<AcademicYearDto>(
                    page.Items.Select(AcademicYearDto.From).ToArray(),
                    page.Page,
                    page.PageSize,
                    page.TotalCount));
        }
    }
}
