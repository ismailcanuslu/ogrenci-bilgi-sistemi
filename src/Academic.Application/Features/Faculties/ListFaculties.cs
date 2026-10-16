using Academic.Application.Abstractions;
using Academic.Application.Common;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;

namespace Academic.Application.Features.Faculties;

public static class ListFaculties
{
    public static readonly string[] SortableFields = ["code", "name", "createdAt"];

    public sealed record Query(
        string? Search = null,
        bool? IsActive = null,
        string SortBy = "name",
        SortDirection SortDirection = SortDirection.Asc,
        int Page = 1,
        int PageSize = 20)
        : IQuery<ServiceResult<PagedResult<FacultyDto>>>;

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
        IFacultyRepository facultyRepository)
        : IQueryHandler<Query, ServiceResult<PagedResult<FacultyDto>>>
    {
        public async Task<ServiceResult<PagedResult<FacultyDto>>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var listAccess = await access.GetListAccessAsync(Permissions.Faculty.Read, cancellationToken);

            if (listAccess.Denied is not null)
            {
                return AuthorizationFailure.To<PagedResult<FacultyDto>>(listAccess.Denied);
            }

            // Departman kapsamlı izin fakülte listesini genişletmez.
            var allowed = listAccess.Unrestricted ? null : listAccess.FacultyIds;

            var page = await facultyRepository.ListAsync(
                new FacultyListCriteria(
                    allowed, request.Search?.Trim(), request.IsActive,
                    request.SortBy, request.SortDirection, request.Page, request.PageSize),
                cancellationToken);

            return ServiceResult<PagedResult<FacultyDto>>.Ok(
                new PagedResult<FacultyDto>(
                    page.Items.Select(FacultyDto.From).ToArray(),
                    page.Page, page.PageSize, page.TotalCount));
        }
    }
}
