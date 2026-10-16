using Academic.Application.Abstractions;
using Academic.Application.Common;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;

namespace Academic.Application.Features.Programs;

public static class ListPrograms
{
    public sealed record Query(
        Guid? DepartmentId = null,
        string? Search = null,
        bool? IsActive = null,
        int Page = 1,
        int PageSize = 20)
        : IQuery<ServiceResult<PagedResult<ProgramDto>>>;

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
        IProgramRepository programRepository)
        : IQueryHandler<Query, ServiceResult<PagedResult<ProgramDto>>>
    {
        public async Task<ServiceResult<PagedResult<ProgramDto>>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var listAccess = await access.GetListAccessAsync(Permissions.Curriculum.Read, cancellationToken);
            if (listAccess.Denied is not null)
            {
                return AuthorizationFailure.To<PagedResult<ProgramDto>>(listAccess.Denied);
            }

            var page = await programRepository.ListAsync(
                new ProgramListCriteria(
                    listAccess.Unrestricted,
                    listAccess.FacultyIds,
                    listAccess.DepartmentIds,
                    request.DepartmentId,
                    request.Search?.Trim(),
                    request.IsActive,
                    request.Page,
                    request.PageSize),
                cancellationToken);

            return ServiceResult<PagedResult<ProgramDto>>.Ok(
                new PagedResult<ProgramDto>(
                    page.Items.Select(ProgramDto.From).ToArray(),
                    page.Page,
                    page.PageSize,
                    page.TotalCount));
        }
    }
}
