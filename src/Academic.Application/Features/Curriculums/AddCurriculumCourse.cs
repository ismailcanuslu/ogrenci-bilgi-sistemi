using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Curriculums;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;

namespace Academic.Application.Features.Curriculums;

public static class AddCurriculumCourse
{
    public sealed record Command(
        Guid CurriculumId,
        Guid VersionId,
        string CourseCode,
        string CourseName,
        int Ects,
        int Credit,
        int Semester,
        bool IsElective,
        CourseCategory Category,
        string? Prerequisites)
        : ICommand<ServiceResult<CurriculumCourseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.CurriculumId).NotEmpty();
            RuleFor(x => x.VersionId).NotEmpty();
            RuleFor(x => x.CourseCode).NotEmpty().MaximumLength(CurriculumCourse.CodeMaxLength);
            RuleFor(x => x.CourseName).NotEmpty().MaximumLength(CurriculumCourse.NameMaxLength);
            RuleFor(x => x.Ects).InclusiveBetween(1, 60);
            RuleFor(x => x.Credit).InclusiveBetween(0, 30);
            RuleFor(x => x.Semester).InclusiveBetween(1, 12);
            RuleFor(x => x.Category).IsInEnum();
            RuleFor(x => x.Prerequisites).MaximumLength(200);
        }
    }

    public sealed class Handler(
        AcademicAccess access,
        ICurriculumRepository curriculumRepository,
        IProgramRepository programRepository,
        IDepartmentRepository departmentRepository,
        IAcademicUnitOfWork unitOfWork,
        IAuditService auditService)
        : ICommandHandler<Command, ServiceResult<CurriculumCourseDto>>
    {
        public async Task<ServiceResult<CurriculumCourseDto>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var curriculum = await curriculumRepository.GetByIdAsync(request.CurriculumId, cancellationToken);
            var program = curriculum is not null
                ? await programRepository.GetByIdAsync(curriculum.ProgramId, cancellationToken)
                : null;
            var department = program is not null
                ? await departmentRepository.GetByIdAsync(program.DepartmentId, cancellationToken)
                : null;

            var denied = await access.CheckCurriculumAsync(
                Permissions.Curriculum.Update,
                department?.Id,
                department?.FacultyId,
                curriculum is not null,
                cancellationToken);

            if (denied is not null)
            {
                return AuthorizationFailure.To<CurriculumCourseDto>(denied);
            }

            if (curriculum is null)
            {
                return ServiceResult<CurriculumCourseDto>.NotFound(AcademicErrors.Curriculum.NotFound());
            }

            var version = curriculum.Versions.FirstOrDefault(v => v.Id == request.VersionId);
            if (version is null)
            {
                return ServiceResult<CurriculumCourseDto>.NotFound(AcademicErrors.Curriculum.VersionNotFound);
            }

            if (version.Status != CurriculumVersionStatus.Draft)
            {
                return ServiceResult<CurriculumCourseDto>.Conflict(AcademicErrors.Curriculum.VersionNotDraft);
            }

            var course = version.AddCourse(
                request.CourseCode,
                request.CourseName,
                request.Ects,
                request.Credit,
                request.Semester,
                request.IsElective,
                request.Category,
                request.Prerequisites);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            await auditService.RecordAsync(
                "CurriculumCourse.Added", "Academic", "CurriculumCourse", course.Id.ToString(),
                newValues: new { course.CurriculumVersionId, course.CourseCode, course.CourseName, course.Ects, course.Semester },
                cancellationToken: cancellationToken);

            return ServiceResult<CurriculumCourseDto>.Created(CurriculumCourseDto.From(course));
        }
    }
}
