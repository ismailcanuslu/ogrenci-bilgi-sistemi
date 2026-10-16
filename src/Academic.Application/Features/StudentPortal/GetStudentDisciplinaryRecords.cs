using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using MediatR;
using Academic.Application.Abstractions;

namespace Academic.Application.Features.StudentPortal;

public sealed record GetStudentDisciplinaryRecordsQuery : IRequest<ServiceResult<IReadOnlyCollection<StudentDisciplinaryRecordDto>>>;

public sealed record StudentDisciplinaryRecordDto(
    Guid Id,
    string PenaltyType,
    string Reason,
    string? DecisionNumber,
    DateOnly DecisionDate,
    int? DurationDays,
    bool IsActive);

public sealed class GetStudentDisciplinaryRecordsHandler(
    IStudentPortalRepository portalRepository,
    ICurrentUser currentUser) : IRequestHandler<GetStudentDisciplinaryRecordsQuery, ServiceResult<IReadOnlyCollection<StudentDisciplinaryRecordDto>>>
{
    public async Task<ServiceResult<IReadOnlyCollection<StudentDisciplinaryRecordDto>>> Handle(
        GetStudentDisciplinaryRecordsQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<IReadOnlyCollection<StudentDisciplinaryRecordDto>>.Unauthorized(
                new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var student = await portalRepository.GetStudentByUserIdAsync(userId, cancellationToken);
        if (student is null)
        {
            return ServiceResult<IReadOnlyCollection<StudentDisciplinaryRecordDto>>.NotFound(
                new Error("Student.NotFound", ErrorType.NotFound));
        }

        var records = await portalRepository.GetDisciplinaryRecordsAsync(student.Id, cancellationToken);

        var dtos = records.Select(r => new StudentDisciplinaryRecordDto(
            r.Id,
            r.PenaltyType.ToString(),
            r.Reason,
            r.DecisionNumber,
            r.DecisionDate,
            r.DurationDays,
            r.IsActive
        )).ToList();

        return ServiceResult<IReadOnlyCollection<StudentDisciplinaryRecordDto>>.Ok(dtos);
    }
}
