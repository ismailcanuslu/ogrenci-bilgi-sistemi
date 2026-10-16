using BuildingBlocks.Application.Abstractions.Messaging;
using SocialContribution.Application.Abstractions;
using SocialContribution.Domain;

namespace SocialContribution.Application.Features;

public record SocialContributionProjectDto(
    Guid Id,
    string Title,
    string Description,
    ContributionCategory Category,
    ProjectStatus Status,
    Guid CoordinatorUserId,
    string CoordinatorName,
    string TargetAudience,
    string PartnerOrganization,
    DateTime StartDate,
    DateTime EndDate,
    int VolunteerQuota,
    int ApprovedVolunteerCount,
    string? RejectionReason,
    string? ImpactSummary,
    int BeneficiaryCount,
    DateTime CreatedAtUtc);

public record VolunteerApplicationDto(
    Guid Id,
    Guid ProjectId,
    Guid UserId,
    string StudentOrStaffNo,
    string FullName,
    string Email,
    string Note,
    VolunteerApplicationStatus Status,
    DateTime AppliedAtUtc,
    DateTime? EvaluatedAtUtc);

// Queries
public record GetSocialProjectsQuery(ContributionCategory? Category = null, ProjectStatus? Status = null) : IQuery<IReadOnlyCollection<SocialContributionProjectDto>>;
public record GetSocialProjectByIdQuery(Guid Id) : IQuery<SocialContributionProjectDto?>;
public record GetCoordinatorProjectsQuery(Guid CoordinatorUserId) : IQuery<IReadOnlyCollection<SocialContributionProjectDto>>;
public record GetUserVolunteeredProjectsQuery(Guid UserId) : IQuery<IReadOnlyCollection<SocialContributionProjectDto>>;
public record GetProjectVolunteersQuery(Guid ProjectId) : IQuery<IReadOnlyCollection<VolunteerApplicationDto>>;

// Commands
public record CreateSocialProjectCommand(
    string Title,
    string Description,
    ContributionCategory Category,
    Guid CoordinatorUserId,
    string CoordinatorName,
    string TargetAudience,
    string PartnerOrganization,
    DateTime StartDate,
    DateTime EndDate,
    int VolunteerQuota) : ICommand<Guid>;

public record ApproveSocialProjectCommand(Guid ProjectId) : ICommand<bool>;
public record RejectSocialProjectCommand(Guid ProjectId, string Reason) : ICommand<bool>;
public record StartSocialProjectCommand(Guid ProjectId) : ICommand<bool>;
public record CompleteSocialProjectCommand(Guid ProjectId, string ImpactSummary, int BeneficiaryCount) : ICommand<bool>;

public record ApplyForVolunteeringCommand(
    Guid ProjectId,
    Guid UserId,
    string StudentOrStaffNo,
    string FullName,
    string Email,
    string Note) : ICommand<Guid>;

public record EvaluateVolunteerCommand(
    Guid ProjectId,
    Guid VolunteerId,
    VolunteerApplicationStatus NewStatus) : ICommand<bool>;

public sealed class SocialContributionHandlers :
    IQueryHandler<GetSocialProjectsQuery, IReadOnlyCollection<SocialContributionProjectDto>>,
    IQueryHandler<GetSocialProjectByIdQuery, SocialContributionProjectDto?>,
    IQueryHandler<GetCoordinatorProjectsQuery, IReadOnlyCollection<SocialContributionProjectDto>>,
    IQueryHandler<GetUserVolunteeredProjectsQuery, IReadOnlyCollection<SocialContributionProjectDto>>,
    IQueryHandler<GetProjectVolunteersQuery, IReadOnlyCollection<VolunteerApplicationDto>>,
    ICommandHandler<CreateSocialProjectCommand, Guid>,
    ICommandHandler<ApproveSocialProjectCommand, bool>,
    ICommandHandler<RejectSocialProjectCommand, bool>,
    ICommandHandler<StartSocialProjectCommand, bool>,
    ICommandHandler<CompleteSocialProjectCommand, bool>,
    ICommandHandler<ApplyForVolunteeringCommand, Guid>,
    ICommandHandler<EvaluateVolunteerCommand, bool>
{
    private readonly ISocialContributionRepository _repository;

    public SocialContributionHandlers(ISocialContributionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<SocialContributionProjectDto>> Handle(GetSocialProjectsQuery query, CancellationToken cancellationToken)
    {
        var list = await _repository.GetAllAsync(query.Category, query.Status, cancellationToken);
        return list.Select(MapDto).ToList();
    }

    public async Task<SocialContributionProjectDto?> Handle(GetSocialProjectByIdQuery query, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(query.Id, cancellationToken);
        return item == null ? null : MapDto(item);
    }

    public async Task<IReadOnlyCollection<SocialContributionProjectDto>> Handle(GetCoordinatorProjectsQuery query, CancellationToken cancellationToken)
    {
        var list = await _repository.GetByCoordinatorAsync(query.CoordinatorUserId, cancellationToken);
        return list.Select(MapDto).ToList();
    }

    public async Task<IReadOnlyCollection<SocialContributionProjectDto>> Handle(GetUserVolunteeredProjectsQuery query, CancellationToken cancellationToken)
    {
        var list = await _repository.GetUserVolunteeredProjectsAsync(query.UserId, cancellationToken);
        return list.Select(MapDto).ToList();
    }

    public async Task<IReadOnlyCollection<VolunteerApplicationDto>> Handle(GetProjectVolunteersQuery query, CancellationToken cancellationToken)
    {
        var project = await _repository.GetByIdAsync(query.ProjectId, cancellationToken);
        if (project == null) return Array.Empty<VolunteerApplicationDto>();

        return project.Volunteers.Select(v => new VolunteerApplicationDto(
            v.Id,
            v.ProjectId,
            v.UserId,
            v.StudentOrStaffNo,
            v.FullName,
            v.Email,
            v.Note,
            v.Status,
            v.AppliedAtUtc,
            v.EvaluatedAtUtc)).ToList();
    }

    public async Task<Guid> Handle(CreateSocialProjectCommand command, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();
        var project = SocialContributionProject.Create(
            id,
            command.Title,
            command.Description,
            command.Category,
            command.CoordinatorUserId,
            command.CoordinatorName,
            command.TargetAudience,
            command.PartnerOrganization,
            command.StartDate,
            command.EndDate,
            command.VolunteerQuota);

        await _repository.AddAsync(project, cancellationToken);
        return id;
    }

    public async Task<bool> Handle(ApproveSocialProjectCommand command, CancellationToken cancellationToken)
    {
        var project = await _repository.GetByIdAsync(command.ProjectId, cancellationToken);
        if (project == null) return false;

        project.Approve();
        await _repository.UpdateAsync(project, cancellationToken);
        return true;
    }

    public async Task<bool> Handle(RejectSocialProjectCommand command, CancellationToken cancellationToken)
    {
        var project = await _repository.GetByIdAsync(command.ProjectId, cancellationToken);
        if (project == null) return false;

        project.Reject(command.Reason);
        await _repository.UpdateAsync(project, cancellationToken);
        return true;
    }

    public async Task<bool> Handle(StartSocialProjectCommand command, CancellationToken cancellationToken)
    {
        var project = await _repository.GetByIdAsync(command.ProjectId, cancellationToken);
        if (project == null) return false;

        project.StartProject();
        await _repository.UpdateAsync(project, cancellationToken);
        return true;
    }

    public async Task<bool> Handle(CompleteSocialProjectCommand command, CancellationToken cancellationToken)
    {
        var project = await _repository.GetByIdAsync(command.ProjectId, cancellationToken);
        if (project == null) return false;

        project.CompleteProject(command.ImpactSummary, command.BeneficiaryCount);
        await _repository.UpdateAsync(project, cancellationToken);
        return true;
    }

    public async Task<Guid> Handle(ApplyForVolunteeringCommand command, CancellationToken cancellationToken)
    {
        var project = await _repository.GetByIdAsync(command.ProjectId, cancellationToken);
        if (project == null) throw new InvalidOperationException("Proje bulunamadı.");

        var volId = Guid.NewGuid();
        project.AddVolunteerApplication(
            volId,
            command.UserId,
            command.StudentOrStaffNo,
            command.FullName,
            command.Email,
            command.Note);

        await _repository.UpdateAsync(project, cancellationToken);
        return volId;
    }

    public async Task<bool> Handle(EvaluateVolunteerCommand command, CancellationToken cancellationToken)
    {
        var project = await _repository.GetByIdAsync(command.ProjectId, cancellationToken);
        if (project == null) return false;

        project.UpdateVolunteerStatus(command.VolunteerId, command.NewStatus);
        await _repository.UpdateAsync(project, cancellationToken);
        return true;
    }

    private static SocialContributionProjectDto MapDto(SocialContributionProject p) =>
        new(
            p.Id,
            p.Title,
            p.Description,
            p.Category,
            p.Status,
            p.CoordinatorUserId,
            p.CoordinatorName,
            p.TargetAudience,
            p.PartnerOrganization,
            p.StartDate,
            p.EndDate,
            p.VolunteerQuota,
            p.ApprovedVolunteerCount,
            p.RejectionReason,
            p.ImpactSummary,
            p.BeneficiaryCount,
            p.CreatedAtUtc);
}
