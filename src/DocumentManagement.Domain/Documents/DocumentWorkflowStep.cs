using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace DocumentManagement.Domain.Documents;

public sealed class DocumentWorkflowStep : Entity<Guid>
{
    public const int NoteMaxLength = 500;

    public Guid DocumentId { get; private set; }

    public int StepOrder { get; private set; }

    public WorkflowStepType StepType { get; private set; }

    public Guid AssigneeUserId { get; private set; }

    public WorkflowStepStatus Status { get; private set; }

    public string? Note { get; private set; }

    public DateTime? ActionDateUtc { get; private set; }

    private DocumentWorkflowStep()
    {
    }

    public DocumentWorkflowStep(
        Guid id,
        Guid documentId,
        int stepOrder,
        WorkflowStepType stepType,
        Guid assigneeUserId)
        : base(id)
    {
        if (documentId == Guid.Empty)
        {
            throw new DomainException("DocumentWorkflowStep.DocumentRequired");
        }

        if (assigneeUserId == Guid.Empty)
        {
            throw new DomainException("DocumentWorkflowStep.AssigneeRequired");
        }

        if (stepOrder < 1)
        {
            throw new DomainException("DocumentWorkflowStep.InvalidOrder");
        }

        DocumentId = documentId;
        StepOrder = stepOrder;
        StepType = stepType;
        AssigneeUserId = assigneeUserId;
        Status = WorkflowStepStatus.Pending;
    }

    public void Approve(string? note, DateTime actionDateUtc)
    {
        if (Status != WorkflowStepStatus.Pending)
        {
            throw new DomainException("DocumentWorkflowStep.NotPending");
        }

        Status = WorkflowStepStatus.Approved;
        Note = note?.Trim();
        ActionDateUtc = actionDateUtc;
    }

    public void Reject(string? note, DateTime actionDateUtc)
    {
        if (Status != WorkflowStepStatus.Pending)
        {
            throw new DomainException("DocumentWorkflowStep.NotPending");
        }

        Status = WorkflowStepStatus.Rejected;
        Note = note?.Trim();
        ActionDateUtc = actionDateUtc;
    }
}
