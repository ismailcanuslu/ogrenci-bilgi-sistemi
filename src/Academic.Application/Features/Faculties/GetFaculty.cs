using Academic.Application.Abstractions;
using Academic.Application.Common;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;

namespace Academic.Application.Features.Faculties;

public static class GetFaculty
{
    public sealed record Query(Guid Id) : IQuery<ServiceResult<FacultyDto>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator() => RuleFor(x => x.Id).NotEmpty();
    }

    public sealed class Handler(
        AcademicAccess access,
        IFacultyRepository facultyRepository)
        : IQueryHandler<Query, ServiceResult<FacultyDto>>
    {
        public async Task<ServiceResult<FacultyDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var faculty = await facultyRepository.GetByIdAsync(request.Id, cancellationToken);

            var denied = await access.CheckFacultyAsync(Permissions.Faculty.Read, request.Id, faculty is not null, cancellationToken);

            if (denied is not null)
            {
                return AuthorizationFailure.To<FacultyDto>(denied);
            }

            return faculty is null
                ? ServiceResult<FacultyDto>.NotFound(AcademicErrors.Faculty.NotFound())
                : ServiceResult<FacultyDto>.Ok(FacultyDto.From(faculty));
        }
    }
}
