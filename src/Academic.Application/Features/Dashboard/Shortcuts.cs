using Academic.Application.Abstractions;
using Academic.Domain.Dashboard;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;
using MediatR;

namespace Academic.Application.Features.Dashboard;

public sealed record UserShortcutDto(
    Guid Id,
    string Title,
    string TargetUrl,
    string? Icon,
    string? Color,
    int Order);

public sealed record GetUserShortcutsQuery : IRequest<ServiceResult<IReadOnlyCollection<UserShortcutDto>>>;

public sealed class GetUserShortcutsHandler(
    IDashboardRepository dashboardRepository,
    ICurrentUser currentUser) : IRequestHandler<GetUserShortcutsQuery, ServiceResult<IReadOnlyCollection<UserShortcutDto>>>
{
    public async Task<ServiceResult<IReadOnlyCollection<UserShortcutDto>>> Handle(
        GetUserShortcutsQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<IReadOnlyCollection<UserShortcutDto>>.Unauthorized(
                new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var shortcuts = await dashboardRepository.GetUserShortcutsAsync(userId, cancellationToken);

        var dtos = shortcuts.Select(s => new UserShortcutDto(
            s.Id,
            s.Title,
            s.TargetUrl,
            s.Icon,
            s.Color,
            s.Order
        )).ToList();

        // Varsayılan kısayollar yoksa hızlı linkleri göster
        if (dtos.Count == 0)
        {
            dtos.Add(new UserShortcutDto(Guid.NewGuid(), "Ders Seçimi", "/academic/course-selection", "repeat", "primary", 1));
            dtos.Add(new UserShortcutDto(Guid.NewGuid(), "Haftalık Program", "/student-portal/weekly-schedule", "calendar", "info", 2));
            dtos.Add(new UserShortcutDto(Guid.NewGuid(), "Transkript", "/student-portal/transcript", "file-text", "success", 3));
            dtos.Add(new UserShortcutDto(Guid.NewGuid(), "Canlı Dersler", "/student-portal/live-sessions", "video", "warning", 4));
        }

        return ServiceResult<IReadOnlyCollection<UserShortcutDto>>.Ok(dtos);
    }
}

public sealed record AddUserShortcutCommand(
    string Title,
    string TargetUrl,
    string? Icon,
    string? Color) : IRequest<ServiceResult<Guid>>;

public sealed class AddUserShortcutValidator : AbstractValidator<AddUserShortcutCommand>
{
    public AddUserShortcutValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(UserPersonalShortcut.TitleMaxLength);
        RuleFor(x => x.TargetUrl).NotEmpty().MaximumLength(UserPersonalShortcut.UrlMaxLength);
    }
}

public sealed class AddUserShortcutHandler(
    IDashboardRepository dashboardRepository,
    IAcademicUnitOfWork unitOfWork,
    ICurrentUser currentUser) : IRequestHandler<AddUserShortcutCommand, ServiceResult<Guid>>
{
    public async Task<ServiceResult<Guid>> Handle(
        AddUserShortcutCommand request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<Guid>.Unauthorized(new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var existing = await dashboardRepository.GetUserShortcutsAsync(userId, cancellationToken);
        var nextOrder = existing.Count > 0 ? existing.Max(s => s.Order) + 1 : 1;

        var shortcut = new UserPersonalShortcut(
            Guid.NewGuid(),
            userId,
            request.Title,
            request.TargetUrl,
            request.Icon,
            request.Color,
            nextOrder);

        await dashboardRepository.AddShortcutAsync(shortcut, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ServiceResult<Guid>.Created(shortcut.Id);
    }
}

public sealed record RemoveUserShortcutCommand(Guid ShortcutId) : IRequest<ServiceResult<Unit>>;

public sealed class RemoveUserShortcutHandler(
    IDashboardRepository dashboardRepository,
    IAcademicUnitOfWork unitOfWork,
    ICurrentUser currentUser) : IRequestHandler<RemoveUserShortcutCommand, ServiceResult<Unit>>
{
    public async Task<ServiceResult<Unit>> Handle(
        RemoveUserShortcutCommand request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<Unit>.Unauthorized(new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var shortcut = await dashboardRepository.GetShortcutByIdAsync(request.ShortcutId, cancellationToken);
        if (shortcut is null || shortcut.UserId != userId)
        {
            return ServiceResult<Unit>.NotFound(new Error("UserPersonalShortcut.NotFound", ErrorType.NotFound));
        }

        await dashboardRepository.RemoveShortcutAsync(shortcut, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ServiceResult<Unit>.Ok(Unit.Value);
    }
}
