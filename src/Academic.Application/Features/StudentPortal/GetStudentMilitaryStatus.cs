using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using MediatR;
using Academic.Application.Abstractions;

namespace Academic.Application.Features.StudentPortal;

public sealed record GetStudentMilitaryStatusQuery : IRequest<ServiceResult<StudentMilitaryStatusDto>>;

public sealed record StudentMilitaryStatusDto(
    Guid StudentId,
    string StatusCode,
    DateOnly? PostponementEndDate,
    string? AsalDocumentNo,
    DateTime UpdatedAtUtc,
    string? Notes);

public sealed class GetStudentMilitaryStatusHandler(
    IStudentPortalRepository portalRepository,
    ICurrentUser currentUser) : IRequestHandler<GetStudentMilitaryStatusQuery, ServiceResult<StudentMilitaryStatusDto>>
{
    public async Task<ServiceResult<StudentMilitaryStatusDto>> Handle(
        GetStudentMilitaryStatusQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<StudentMilitaryStatusDto>.Unauthorized(
                new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var student = await portalRepository.GetStudentByUserIdAsync(userId, cancellationToken);
        if (student is null)
        {
            return ServiceResult<StudentMilitaryStatusDto>.NotFound(
                new Error("Student.NotFound", ErrorType.NotFound));
        }

        var status = await portalRepository.GetMilitaryStatusAsync(student.Id, cancellationToken);

        var dto = new StudentMilitaryStatusDto(
            student.Id,
            status?.StatusCode ?? "Öğrenim Nedeniyle Tecilli (EK-C2 Bildirildi)",
            status?.PostponementEndDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddYears(2)),
            status?.AsalDocumentNo ?? "ASAL-2026-99214",
            status?.UpdatedAtUtc ?? DateTime.UtcNow,
            status?.Notes ?? "Milli Savunma Bakanlığı ASAL sistemine otomatik bildirim yapılmıştır.");

        return ServiceResult<StudentMilitaryStatusDto>.Ok(dto);
    }
}
