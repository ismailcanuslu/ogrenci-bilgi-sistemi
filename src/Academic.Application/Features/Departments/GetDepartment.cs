using Academic.Application.Abstractions;
using Academic.Application.Common;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;

namespace Academic.Application.Features.Departments;

public static class GetDepartment
{
    public sealed record Query(Guid Id) : IQuery<ServiceResult<DepartmentDto>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator() => RuleFor(x => x.Id).NotEmpty();
    }

    public sealed class Handler(
        AcademicAccess access,
        IDepartmentRepository departmentRepository)
        : IQueryHandler<Query, ServiceResult<DepartmentDto>>
    {
        public async Task<ServiceResult<DepartmentDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var department = await departmentRepository.GetByIdAsync(request.Id, cancellationToken);

            var denied = await access.CheckDepartmentAsync(Permissions.Department.Read, department, cancellationToken);

            if (denied is not null)
            {
                return AuthorizationFailure.To<DepartmentDto>(denied);
            }

            return department is null
                ? ServiceResult<DepartmentDto>.NotFound(AcademicErrors.Department.NotFound())
                : ServiceResult<DepartmentDto>.Ok(DepartmentDto.From(department));
        }
    }
}
