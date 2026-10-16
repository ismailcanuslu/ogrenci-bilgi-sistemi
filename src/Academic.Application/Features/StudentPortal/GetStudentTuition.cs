using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using MediatR;
using Academic.Application.Abstractions;

namespace Academic.Application.Features.StudentPortal;

public sealed record GetStudentTuitionQuery : IRequest<ServiceResult<StudentTuitionSummaryDto>>;

public sealed record StudentTuitionItemDto(
    Guid Id,
    string TermName,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal RemainingDebt,
    bool IsPaid,
    bool IsExempt,
    DateTime DueDateUtc,
    string? Notes);

public sealed record StudentTuitionSummaryDto(
    decimal TotalDebt,
    IReadOnlyCollection<StudentTuitionItemDto> Tuitions);

public sealed class GetStudentTuitionHandler(
    IStudentPortalRepository portalRepository,
    ICurrentUser currentUser) : IRequestHandler<GetStudentTuitionQuery, ServiceResult<StudentTuitionSummaryDto>>
{
    public async Task<ServiceResult<StudentTuitionSummaryDto>> Handle(
        GetStudentTuitionQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<StudentTuitionSummaryDto>.Unauthorized(
                new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var student = await portalRepository.GetStudentByUserIdAsync(userId, cancellationToken);
        if (student is null)
        {
            return ServiceResult<StudentTuitionSummaryDto>.NotFound(
                new Error("Student.NotFound", ErrorType.NotFound));
        }

        var tuitions = await portalRepository.GetTuitionsAsync(student.Id, cancellationToken);

        var items = tuitions.Select(t => new StudentTuitionItemDto(
            t.Id,
            t.TermName,
            t.TotalAmount,
            t.PaidAmount,
            t.RemainingDebt,
            t.IsPaid,
            t.IsExempt,
            t.DueDateUtc,
            t.Notes
        )).ToList();

        var totalDebt = items.Sum(i => i.RemainingDebt);

        return ServiceResult<StudentTuitionSummaryDto>.Ok(new StudentTuitionSummaryDto(totalDebt, items));
    }
}
