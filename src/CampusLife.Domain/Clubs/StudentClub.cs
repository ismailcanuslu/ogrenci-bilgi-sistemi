using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace CampusLife.Domain.Clubs;

public sealed class StudentClub : AggregateRoot<Guid>
{
    public const int NameMaxLength = 150;
    public const int CodeMaxLength = 30;
    public const int CategoryMaxLength = 100;
    public const int DescriptionMaxLength = 1000;

    public string Name { get; private set; } = null!;

    public string Code { get; private set; } = null!;

    public string? Category { get; private set; }

    public string? Description { get; private set; }

    public Guid? AdvisorUserId { get; private set; }

    public Guid? PresidentStudentId { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    private StudentClub()
    {
    }

    public StudentClub(
        Guid id,
        string name,
        string code,
        string? category,
        string? description,
        Guid? advisorUserId,
        Guid? presidentStudentId,
        DateTime createdAtUtc)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("StudentClub.NameRequired");
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainException("StudentClub.CodeRequired");
        }

        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        Category = category?.Trim();
        Description = description?.Trim();
        AdvisorUserId = advisorUserId;
        PresidentStudentId = presidentStudentId;
        IsActive = true;
        CreatedAtUtc = createdAtUtc;
    }

    public static StudentClub Create(
        string name,
        string code,
        string? category,
        string? description,
        Guid? advisorUserId,
        Guid? presidentStudentId,
        DateTime createdAtUtc)
    {
        return new StudentClub(
            Guid.NewGuid(),
            name,
            code,
            category,
            description,
            advisorUserId,
            presidentStudentId,
            createdAtUtc);
    }

    public void UpdateLeadership(Guid? advisorUserId, Guid? presidentStudentId, DateTime updatedAtUtc)
    {
        AdvisorUserId = advisorUserId;
        PresidentStudentId = presidentStudentId;
        UpdatedAtUtc = updatedAtUtc;
    }

    public void SetActive(bool isActive, DateTime updatedAtUtc)
    {
        IsActive = isActive;
        UpdatedAtUtc = updatedAtUtc;
    }
}
