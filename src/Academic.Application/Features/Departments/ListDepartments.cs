using Academic.Application.Abstractions;
using Academic.Application.Common;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;

namespace Academic.Application.Features.Departments;

public static class ListDepartments
{
    public static readonly string[] SortableFields = ["code", "name", "createdAt"];

    public sealed record Query(
        Guid? FacultyId = null,
        string? Search = null,
        bool? IsActive = null,
        string SortBy = "name",
        SortDirection SortDirection = SortDirection.Asc,
        int Page = 1,
        int PageSize = 20)
        : IQuery<ServiceResult<PagedResult<DepartmentDto>>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, PagingRequest.MaxPageSize);
            RuleFor(x => x.Search).MaximumLength(100);
            RuleFor(x => x.SortBy).Must(f => SortableFields.Contains(f, StringComparer.OrdinalIgnoreCase));
            RuleFor(x => x.SortDirection).IsInEnum();
        }
    }

    public sealed class Handler(
        AcademicAccess access,
        IDepartmentRepository departmentRepository)
        : IQueryHandler<Query, ServiceResult<PagedResult<DepartmentDto>>>
    {
        public async Task<ServiceResult<PagedResult<DepartmentDto>>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var listAccess = await access.GetListAccessAsync(Permissions.Department.Read, cancellationToken);

            if (listAccess.Denied is not null)
            {
                return AuthorizationFailure.To<PagedResult<DepartmentDto>>(listAccess.Denied);
            }

            var page = await departmentRepository.ListAsync(
                new DepartmentListCriteria(
                    listAccess.Unrestricted, listAccess.FacultyIds, listAccess.DepartmentIds,
                    request.FacultyId, request.Search?.Trim(), request.IsActive,
                    request.SortBy, request.SortDirection, request.Page, request.PageSize),
                cancellationToken);

            return ServiceResult<PagedResult<DepartmentDto>>.Ok(
                new PagedResult<DepartmentDto>(
                    page.Items.Select(DepartmentDto.From).ToArray(),
                    page.Page, page.PageSize, page.TotalCount));
        }
    }
}
