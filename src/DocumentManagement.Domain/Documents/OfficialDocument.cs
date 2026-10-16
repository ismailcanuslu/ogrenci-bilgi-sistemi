using System.Security.Cryptography;
using System.Text;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace DocumentManagement.Domain.Documents;

public sealed class OfficialDocument : AggregateRoot<Guid>
{
    public const int DocumentNumberMaxLength = 50;
    public const int SubjectMaxLength = 250;
    public const int ContentSummaryMaxLength = 4000;
    public const int VerificationCodeMaxLength = 50;
    public const int VerificationHashMaxLength = 100;
    public const int DepartmentCodeMaxLength = 30;

    private readonly List<DocumentWorkflowStep> _workflowSteps = [];

    public string? DocumentNumber { get; private set; }

    public string Subject { get; private set; } = null!;

    public string ContentSummary { get; private set; } = null!;

    public DocumentType Type { get; private set; }

    public DocumentStatus Status { get; private set; }

    public Guid CreatorUserId { get; private set; }

    public string? DepartmentCode { get; private set; }

    public string VerificationCode { get; private set; } = null!;

    public string VerificationHash { get; private set; } = null!;

    public string? AttachmentUrl { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? SignedAtUtc { get; private set; }

    public IReadOnlyCollection<DocumentWorkflowStep> WorkflowSteps => _workflowSteps.AsReadOnly();

    private OfficialDocument()
    {
    }

    public OfficialDocument(
        Guid id,
        string subject,
        string contentSummary,
        DocumentType type,
        Guid creatorUserId,
        string? departmentCode,
        string? attachmentUrl,
        DateTime createdAtUtc)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new DomainException("OfficialDocument.SubjectRequired");
        }

        if (string.IsNullOrWhiteSpace(contentSummary))
        {
            throw new DomainException("OfficialDocument.ContentRequired");
        }

        if (creatorUserId == Guid.Empty)
        {
            throw new DomainException("OfficialDocument.CreatorRequired");
        }

        Subject = subject.Trim();
        ContentSummary = contentSummary.Trim();
        Type = type;
        Status = DocumentStatus.Draft;
        CreatorUserId = creatorUserId;
        DepartmentCode = departmentCode?.Trim().ToUpperInvariant();
        AttachmentUrl = attachmentUrl?.Trim();
        CreatedAtUtc = createdAtUtc;

        VerificationCode = GenerateVerificationCode(createdAtUtc);
        VerificationHash = ComputeVerificationHash(id, VerificationCode, Subject, createdAtUtc);
    }

    public static OfficialDocument Create(
        string subject,
        string contentSummary,
        DocumentType type,
        Guid creatorUserId,
        string? departmentCode,
        string? attachmentUrl,
        DateTime createdAtUtc)
    {
        return new OfficialDocument(
            Guid.NewGuid(),
            subject,
            contentSummary,
            type,
            creatorUserId,
            departmentCode,
            attachmentUrl,
            createdAtUtc);
    }

    public void AddWorkflowStep(WorkflowStepType stepType, Guid assigneeUserId)
    {
        if (Status != DocumentStatus.Draft)
        {
            throw new DomainException("OfficialDocument.CannotModifyStepsWhenNotInDraft");
        }

        var nextOrder = _workflowSteps.Count + 1;
        var step = new DocumentWorkflowStep(Guid.NewGuid(), Id, nextOrder, stepType, assigneeUserId);
        _workflowSteps.Add(step);
    }

    public void SubmitToWorkflow()
    {
        if (Status != DocumentStatus.Draft)
        {
            throw new DomainException("OfficialDocument.NotDraft");
        }

        if (_workflowSteps.Count == 0)
        {
            throw new DomainException("OfficialDocument.WorkflowStepsRequired");
        }

        Status = DocumentStatus.InWorkflow;
    }

    public void ProcessStep(Guid stepId, Guid currentUserId, bool approve, string? note, DateTime actionDateUtc)
    {
        if (Status != DocumentStatus.InWorkflow)
        {
            throw new DomainException("OfficialDocument.NotInWorkflow");
        }

        var currentPendingStep = _workflowSteps
            .OrderBy(x => x.StepOrder)
            .FirstOrDefault(x => x.Status == WorkflowStepStatus.Pending);

        if (currentPendingStep is null || currentPendingStep.Id != stepId)
        {
            throw new DomainException("OfficialDocument.NotCurrentStep");
        }

        if (currentPendingStep.AssigneeUserId != currentUserId)
        {
            throw new DomainException("OfficialDocument.AssigneeMismatch");
        }

        if (approve)
        {
            currentPendingStep.Approve(note, actionDateUtc);

            // Tüm adımlar onaylandı mı kontrol et
            var hasRemainingPending = _workflowSteps.Any(x => x.Status == WorkflowStepStatus.Pending);
            if (!hasRemainingPending)
            {
                Status = DocumentStatus.Signed;
                SignedAtUtc = actionDateUtc;
                DocumentNumber = GenerateDocumentNumber(actionDateUtc, DepartmentCode);
            }
        }
        else
        {
            currentPendingStep.Reject(note, actionDateUtc);
            Status = DocumentStatus.Rejected;
        }
    }

    public void Revoke(DateTime revokedAtUtc)
    {
        if (Status != DocumentStatus.Signed)
        {
            throw new DomainException("OfficialDocument.CannotRevokeUnsigned");
        }

        Status = DocumentStatus.Revoked;
    }

    private static string GenerateVerificationCode(DateTime date)
    {
        return $"DOC-{date:yyyyMM}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
    }

    private static string GenerateDocumentNumber(DateTime date, string? departmentCode)
    {
        var dept = string.IsNullOrWhiteSpace(departmentCode) ? "GENEL" : departmentCode.Trim().ToUpperInvariant();
        return $"E-{dept}-{date:yyyy}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
    }

    private static string ComputeVerificationHash(Guid id, string code, string subject, DateTime date)
    {
        var raw = $"{id}:{code}:{subject}:{date:O}";
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
