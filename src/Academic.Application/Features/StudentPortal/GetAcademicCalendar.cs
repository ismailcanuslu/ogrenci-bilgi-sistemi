using BuildingBlocks.Application.Results;
using MediatR;
using Academic.Application.Abstractions;

namespace Academic.Application.Features.StudentPortal;

public sealed record GetAcademicCalendarQuery(string? AcademicYear = null) : IRequest<ServiceResult<IReadOnlyCollection<AcademicCalendarEventDto>>>;

public sealed record AcademicCalendarEventDto(
    Guid Id,
    string Title,
    string? Description,
    DateOnly StartDate,
    DateOnly EndDate,
    string AcademicYear,
    string Semester);

public sealed class GetAcademicCalendarHandler(
    IStudentPortalRepository portalRepository) : IRequestHandler<GetAcademicCalendarQuery, ServiceResult<IReadOnlyCollection<AcademicCalendarEventDto>>>
{
    public async Task<ServiceResult<IReadOnlyCollection<AcademicCalendarEventDto>>> Handle(
        GetAcademicCalendarQuery request,
        CancellationToken cancellationToken)
    {
        var events = await portalRepository.GetAcademicCalendarEventsAsync(request.AcademicYear, cancellationToken);

        var dtos = events.Select(e => new AcademicCalendarEventDto(
            e.Id,
            e.Title,
            e.Description,
            e.StartDate,
            e.EndDate,
            e.AcademicYear,
            e.Semester
        )).ToList();

        // Eğer veritabanında henüz etkinlik yoksa varsayılan takvim maddelerini döndür
        if (dtos.Count == 0)
        {
            var year = request.AcademicYear ?? "2026-2027";
            dtos.Add(new AcademicCalendarEventDto(Guid.NewGuid(), "Güz Dönemi Ders Kayıtları ve Danışman Onayları", "Ders seçimi ve kayıt yenileme haftası", new DateOnly(2026, 9, 15), new DateOnly(2026, 9, 22), year, "Güz"));
            dtos.Add(new AcademicCalendarEventDto(Guid.NewGuid(), "Güz Dönemi Ders Başlangıcı", "Derslerin başlaması", new DateOnly(2026, 9, 23), new DateOnly(2026, 9, 23), year, "Güz"));
            dtos.Add(new AcademicCalendarEventDto(Guid.NewGuid(), "Ders Ekle - Bırak (Add/Drop) Haftası", "Mazeretli ders kayıt ve değişiklikleri", new DateOnly(2026, 10, 1), new DateOnly(2026, 10, 5), year, "Güz"));
            dtos.Add(new AcademicCalendarEventDto(Guid.NewGuid(), "Ara Sınavlar (Vize Haftası)", "Ara sınavların yapılması", new DateOnly(2026, 11, 10), new DateOnly(2026, 11, 21), year, "Güz"));
            dtos.Add(new AcademicCalendarEventDto(Guid.NewGuid(), "Yarıyıl Sonu Sınavları (Final Haftası)", "Dönem sonu final sınavları", new DateOnly(2027, 1, 5), new DateOnly(2027, 1, 18), year, "Güz"));
            dtos.Add(new AcademicCalendarEventDto(Guid.NewGuid(), "Bütünleme Sınavları", "Yarıyıl sonu bütünleme sınavları", new DateOnly(2027, 1, 25), new DateOnly(2027, 2, 2), year, "Güz"));
        }

        return ServiceResult<IReadOnlyCollection<AcademicCalendarEventDto>>.Ok(dtos);
    }
}
