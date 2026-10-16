using Academic.Application.Abstractions;
using Academic.Domain.Dashboard;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;
using MediatR;

namespace Academic.Application.Features.Dashboard;

// 1. Sistemde Kullanılabilir Widget Kataloğu
public sealed record AvailableWidgetCatalogItem(
    string Type,
    string DisplayName,
    string Description,
    int DefaultColumnSpan,
    string Icon);

public sealed record GetAvailableWidgetsQuery : IRequest<ServiceResult<IReadOnlyCollection<AvailableWidgetCatalogItem>>>;

public sealed class GetAvailableWidgetsHandler : IRequestHandler<GetAvailableWidgetsQuery, ServiceResult<IReadOnlyCollection<AvailableWidgetCatalogItem>>>
{
    public Task<ServiceResult<IReadOnlyCollection<AvailableWidgetCatalogItem>>> Handle(
        GetAvailableWidgetsQuery request,
        CancellationToken cancellationToken)
    {
        var catalog = new List<AvailableWidgetCatalogItem>
        {
            new("CourseList", "Ders Listesi", "Dönemlik kayıtlı derslerinizi ve ders detaylarınızı listeler", 2, "book"),
            new("WeeklySchedule", "Haftalık Ders Programı", "Derslik ve fakülte bilgileriyle haftalık ders saatleri", 2, "calendar-days"),
            new("AcademicCalendar", "Akademik Takvim", "Sınav, kayıt ve tatil haftaları önemli tarihleri", 1, "calendar"),
            new("UpcomingExams", "Yaklaşan Sınavlar", "Vize, final ve online sınav tarih ve bağlantıları", 1, "clipboard-check"),
            new("Announcements", "Bilgilendirmeler ve Duyurular", "Fakülte ve rektörlükten son duyurular", 1, "bell"),
            new("Surveys", "Anketler", "Ders değerlendirme ve kurumsal memnuniyet anketleri", 1, "list-checks"),
            new("TuitionStatus", "Harç ve Borç Durumu", "Dönem harç borcu ve son ödeme tarihi bilgisi", 1, "credit-card"),
            new("AttendanceSummary", "Devamsızlık Durumu", "Ders bazında devamsızlık saatleri ve limitleri", 1, "user-x"),
            new("RecentGrades", "Son Açıklanan Notlar", "Sisteme yeni girilen vize ve ödev notları", 1, "award"),
            new("SupportTickets", "Destek Taleplerim", "Açık servis destek biletleri ve durumları", 1, "life-buoy"),
            new("FacilityReservations", "Tesis Rezervasyonlarım", "Spor salonu, halı saha ve PDR randevuları", 1, "clock")
        };

        return Task.FromResult(ServiceResult<IReadOnlyCollection<AvailableWidgetCatalogItem>>.Ok(catalog));
    }
}

// 2. Kullanıcının Dashboard Widget'larını Getirme
public sealed record UserWidgetDto(
    Guid Id,
    string Type,
    string? CustomTitle,
    int Order,
    int ColumnSpan,
    string? ConfigurationJson,
    bool IsVisible);

public sealed record GetUserWidgetsQuery : IRequest<ServiceResult<IReadOnlyCollection<UserWidgetDto>>>;

public sealed class GetUserWidgetsHandler(
    IDashboardRepository dashboardRepository,
    ICurrentUser currentUser) : IRequestHandler<GetUserWidgetsQuery, ServiceResult<IReadOnlyCollection<UserWidgetDto>>>
{
    public async Task<ServiceResult<IReadOnlyCollection<UserWidgetDto>>> Handle(
        GetUserWidgetsQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<IReadOnlyCollection<UserWidgetDto>>.Unauthorized(
                new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var widgets = await dashboardRepository.GetUserWidgetsAsync(userId, cancellationToken);

        var dtos = widgets.Select(w => new UserWidgetDto(
            w.Id,
            w.Type.ToString(),
            w.CustomTitle,
            w.Order,
            w.ColumnSpan,
            w.ConfigurationJson,
            w.IsVisible
        )).ToList();

        // Eğer kullanıcı henüz hiç widget eklememişse varsayılan CourseList widget'ını döndür
        if (dtos.Count == 0)
        {
            dtos.Add(new UserWidgetDto(
                Guid.NewGuid(),
                WidgetType.CourseList.ToString(),
                "Ders Listesi (Widget)",
                1,
                2,
                "{}",
                true));
        }

        return ServiceResult<IReadOnlyCollection<UserWidgetDto>>.Ok(dtos);
    }
}

// 3. Yeni Widget Ekleme ("+ Yeni Widget Ekle" Butonu)
public sealed record AddUserWidgetCommand(
    WidgetType Type,
    string? CustomTitle,
    int ColumnSpan = 1,
    string? ConfigurationJson = null) : IRequest<ServiceResult<Guid>>;

public sealed class AddUserWidgetValidator : AbstractValidator<AddUserWidgetCommand>
{
    public AddUserWidgetValidator()
    {
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.ColumnSpan).InclusiveBetween(1, 2);
    }
}

public sealed class AddUserWidgetHandler(
    IDashboardRepository dashboardRepository,
    IAcademicUnitOfWork unitOfWork,
    ICurrentUser currentUser) : IRequestHandler<AddUserWidgetCommand, ServiceResult<Guid>>
{
    public async Task<ServiceResult<Guid>> Handle(
        AddUserWidgetCommand request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<Guid>.Unauthorized(new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var existing = await dashboardRepository.GetUserWidgetsAsync(userId, cancellationToken);
        var nextOrder = existing.Count > 0 ? existing.Max(w => w.Order) + 1 : 1;

        var widget = new UserDashboardWidget(
            Guid.NewGuid(),
            userId,
            request.Type,
            request.CustomTitle,
            nextOrder,
            request.ColumnSpan,
            request.ConfigurationJson,
            DateTime.UtcNow);

        await dashboardRepository.AddWidgetAsync(widget, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ServiceResult<Guid>.Created(widget.Id);
    }
}

// 4. Widget Güncelleme (Sıralama ve Kolon Genişliği)
public sealed record UpdateUserWidgetCommand(
    Guid WidgetId,
    string? CustomTitle,
    int Order,
    int ColumnSpan,
    string? ConfigurationJson,
    bool IsVisible) : IRequest<ServiceResult<Unit>>;

public sealed class UpdateUserWidgetHandler(
    IDashboardRepository dashboardRepository,
    IAcademicUnitOfWork unitOfWork,
    ICurrentUser currentUser) : IRequestHandler<UpdateUserWidgetCommand, ServiceResult<Unit>>
{
    public async Task<ServiceResult<Unit>> Handle(
        UpdateUserWidgetCommand request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<Unit>.Unauthorized(new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var widget = await dashboardRepository.GetWidgetByIdAsync(request.WidgetId, cancellationToken);
        if (widget is null || widget.UserId != userId)
        {
            return ServiceResult<Unit>.NotFound(new Error("UserDashboardWidget.NotFound", ErrorType.NotFound));
        }

        widget.UpdateSettings(
            request.CustomTitle,
            request.Order,
            request.ColumnSpan,
            request.ConfigurationJson,
            request.IsVisible);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ServiceResult<Unit>.Ok(Unit.Value);
    }
}

// 5. Widget Kaldırma (1. görseldeki kırmızı kapatma çarpısı)
public sealed record RemoveUserWidgetCommand(Guid WidgetId) : IRequest<ServiceResult<Unit>>;

public sealed class RemoveUserWidgetHandler(
    IDashboardRepository dashboardRepository,
    IAcademicUnitOfWork unitOfWork,
    ICurrentUser currentUser) : IRequestHandler<RemoveUserWidgetCommand, ServiceResult<Unit>>
{
    public async Task<ServiceResult<Unit>> Handle(
        RemoveUserWidgetCommand request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<Unit>.Unauthorized(new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var widget = await dashboardRepository.GetWidgetByIdAsync(request.WidgetId, cancellationToken);
        if (widget is null || widget.UserId != userId)
        {
            return ServiceResult<Unit>.NotFound(new Error("UserDashboardWidget.NotFound", ErrorType.NotFound));
        }

        await dashboardRepository.RemoveWidgetAsync(widget, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ServiceResult<Unit>.Ok(Unit.Value);
    }
}
