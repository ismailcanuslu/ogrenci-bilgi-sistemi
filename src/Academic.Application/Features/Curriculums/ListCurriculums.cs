using Academic.Application.Abstractions;
using Academic.Application.Common;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;

namespace Academic.Application.Features.Curriculums;

public static class ListCurriculums
{
    public sealed record Query(
        Guid? ProgramId = null,
        Guid? AcademicYearId = null,
        string? Search = null,
        bool? IsActive = null,
        int Page = 1,
        int PageSize = 20)
        : IQuery<ServiceResult<PagedResult<CurriculumDto>>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, PagingRequest.MaxPageSize);
            RuleFor(x => x.Search).MaximumLength(100);
        }
    }

    public sealed class Handler(
        AcademicAccess access,
        ICurriculumRepository curriculumRepository)
        : IQueryHandler<Query, ServiceResult<PagedResult<CurriculumDto>>>
    {
        public async Task<ServiceResult<PagedResult<CurriculumDto>>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var listAccess = await access.GetListAccessAsync(Permissions.Curriculum.Read, cancellationToken);
            if (listAccess.Denied is not null)
            {
                return AuthorizationFailure.To<PagedResult<CurriculumDto>>(listAccess.Denied);
            }

            var page = await curriculumRepository.ListAsync(
                new CurriculumListCriteria(
                    listAccess.Unrestricted,
                    listAccess.FacultyIds,
                    listAccess.DepartmentIds,
                    request.ProgramId,
                    request.AcademicYearId,
                    request.Search?.Trim(),
                    request.IsActive,
                    request.Page,
                    request.PageSize),
                cancellationToken);

            return ServiceResult<PagedResult<CurriculumDto>>.Ok(
                new PagedResult<CurriculumDto>(
                    page.Items.Select(CurriculumDto.From).ToArray(),
                    page.Page,
                    page.PageSize,
                    page.TotalCount));
        }
    }
}
