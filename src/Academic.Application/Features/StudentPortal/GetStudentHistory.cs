using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using MediatR;
using Academic.Application.Abstractions;

namespace Academic.Application.Features.StudentPortal;

public sealed record GetStudentHistoryQuery : IRequest<ServiceResult<IReadOnlyCollection<StudentHistoryItemDto>>>;

public sealed record StudentHistoryItemDto(
    Guid Id,
    string EventTitle,
    string EventDescription,
    DateTime EventDateUtc,
    string? ActionBy);

public sealed class GetStudentHistoryHandler(
    IStudentPortalRepository portalRepository,
    ICurrentUser currentUser) : IRequestHandler<GetStudentHistoryQuery, ServiceResult<IReadOnlyCollection<StudentHistoryItemDto>>>
{
    public async Task<ServiceResult<IReadOnlyCollection<StudentHistoryItemDto>>> Handle(
        GetStudentHistoryQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<IReadOnlyCollection<StudentHistoryItemDto>>.Unauthorized(
                new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var student = await portalRepository.GetStudentByUserIdAsync(userId, cancellationToken);
        if (student is null)
        {
            return ServiceResult<IReadOnlyCollection<StudentHistoryItemDto>>.NotFound(
                new Error("Student.NotFound", ErrorType.NotFound));
        }

        var records = await portalRepository.GetHistoryRecordsAsync(student.Id, cancellationToken);

        var dtos = records.Select(r => new StudentHistoryItemDto(
            r.Id,
            r.EventTitle,
            r.EventDescription,
            r.EventDateUtc,
            r.ActionBy
        )).ToList();

        // Eğer henüz özel tarihçe kaydı yoksa öğrencinin ilk kayıt tarihçesini göster
        if (dtos.Count == 0)
        {
            dtos.Add(new StudentHistoryItemDto(
                Guid.NewGuid(),
                "İlk Kayıt İşlemi",
                $"{student.EnrollmentYear} Eğitim Öğretim Yılı Asil Kayıt",
                student.EnrollmentDateUtc,
                "Öğrenci İşleri Daire Başkanlığı"));
        }

        return ServiceResult<IReadOnlyCollection<StudentHistoryItemDto>>.Ok(dtos);
    }
}
