using BuildingBlocks.Domain.Primitives;

namespace SocialContribution.Domain;

public sealed class SocialContributionProject : AggregateRoot<Guid>
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public ContributionCategory Category { get; private set; }
    public ProjectStatus Status { get; private set; }
    public Guid CoordinatorUserId { get; private set; }
    public string CoordinatorName { get; private set; } = string.Empty;
    public string TargetAudience { get; private set; } = string.Empty; // Hedef Kitle (örn: İlkokul öğrencileri, yaşlılar vb.)
    public string PartnerOrganization { get; private set; } = string.Empty; // İş Birliği Yapılan Kurum/STK/Belediye
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int VolunteerQuota { get; private set; }
    public int ApprovedVolunteerCount { get; private set; }
    public string? RejectionReason { get; private set; }
    public string? ImpactSummary { get; private set; } // Tamamlandığında etki/çıktı raporu
    public int BeneficiaryCount { get; private set; } // Yararlanıcı / ulaşılan kişi sayısı
    public DateTime CreatedAtUtc { get; private set; }

    private readonly List<ProjectVolunteer> _volunteers = new();
    public IReadOnlyCollection<ProjectVolunteer> Volunteers => _volunteers.AsReadOnly();

    private SocialContributionProject() { }

    public static SocialContributionProject Create(
        Guid id,
        string title,
        string description,
        ContributionCategory category,
        Guid coordinatorUserId,
        string coordinatorName,
        string targetAudience,
        string partnerOrganization,
        DateTime startDate,
        DateTime endDate,
        int volunteerQuota)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Proje başlığı boş olamaz.", nameof(title));

        if (endDate < startDate)
            throw new ArgumentException("Bitiş tarihi başlangıç tarihinden önce olamaz.", nameof(endDate));

        return new SocialContributionProject
        {
            Id = id,
            Title = title.Trim(),
            Description = description.Trim(),
            Category = category,
            Status = ProjectStatus.PendingApproval,
            CoordinatorUserId = coordinatorUserId,
            CoordinatorName = coordinatorName.Trim(),
            TargetAudience = targetAudience.Trim(),
            PartnerOrganization = partnerOrganization.Trim(),
            StartDate = startDate,
            EndDate = endDate,
            VolunteerQuota = Math.Max(1, volunteerQuota),
            ApprovedVolunteerCount = 0,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void Approve()
    {
        if (Status != ProjectStatus.PendingApproval)
            throw new InvalidOperationException("Yalnızca onay bekleyen projeler onaylanabilir.");

        Status = ProjectStatus.Approved;
        RejectionReason = null;
    }

    public void Reject(string reason)
    {
        if (Status != ProjectStatus.PendingApproval)
            throw new InvalidOperationException("Yalnızca onay bekleyen projeler reddedilebilir.");

        Status = ProjectStatus.Rejected;
        RejectionReason = reason;
    }

    public void StartProject()
    {
        if (Status != ProjectStatus.Approved)
            throw new InvalidOperationException("Yalnızca onaylanmış projeler başlatılabilir.");

        Status = ProjectStatus.InProgress;
    }

    public void CompleteProject(string impactSummary, int beneficiaryCount)
    {
        if (Status != ProjectStatus.InProgress && Status != ProjectStatus.Approved)
            throw new InvalidOperationException("Yalnızca aktif veya onaylanmış projeler tamamlanabilir.");

        Status = ProjectStatus.Completed;
        ImpactSummary = impactSummary;
        BeneficiaryCount = beneficiaryCount;
    }

    public ProjectVolunteer AddVolunteerApplication(Guid volunteerId, Guid userId, string studentOrStaffNo, string fullName, string email, string note)
    {
        if (Status != ProjectStatus.Approved && Status != ProjectStatus.InProgress)
            throw new InvalidOperationException("Bu projeye şu anda gönüllü başvurusu alınamamaktadır.");

        if (_volunteers.Any(v => v.UserId == userId && v.Status != VolunteerApplicationStatus.Withdrawn))
            throw new InvalidOperationException("Bu projeye zaten aktif bir gönüllülük başvurunuz bulunmaktadır.");

        var volunteer = ProjectVolunteer.Create(volunteerId, Id, userId, studentOrStaffNo, fullName, email, note);
        _volunteers.Add(volunteer);
        return volunteer;
    }

    public void UpdateVolunteerStatus(Guid volunteerId, VolunteerApplicationStatus newStatus)
    {
        var volunteer = _volunteers.FirstOrDefault(v => v.Id == volunteerId);
        if (volunteer == null)
            throw new InvalidOperationException("Gönüllü başvurusu bulunamadı.");

        if (newStatus == VolunteerApplicationStatus.Approved && ApprovedVolunteerCount >= VolunteerQuota)
            throw new InvalidOperationException("Gönüllü kontenjanı dolmuştur.");

        var oldStatus = volunteer.Status;
        volunteer.SetStatus(newStatus);

        if (oldStatus != VolunteerApplicationStatus.Approved && newStatus == VolunteerApplicationStatus.Approved)
        {
            ApprovedVolunteerCount++;
        }
        else if (oldStatus == VolunteerApplicationStatus.Approved && newStatus != VolunteerApplicationStatus.Approved)
        {
            ApprovedVolunteerCount = Math.Max(0, ApprovedVolunteerCount - 1);
        }
    }
}
