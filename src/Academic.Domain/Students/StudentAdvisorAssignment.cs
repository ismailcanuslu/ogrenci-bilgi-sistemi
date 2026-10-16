using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.Students;

public sealed class StudentAdvisorAssignment : Entity<Guid>
{
    public Guid StudentId { get; private set; }

    public Guid InstructorId { get; private set; }

    public DateTime AssignedDateUtc { get; private set; }

    public bool IsActive { get; private set; }

    private StudentAdvisorAssignment()
    {
    }

    public StudentAdvisorAssignment(
        Guid id,
        Guid studentId,
        Guid instructorId,
        DateTime assignedDateUtc,
        bool isActive = true)
        : base(id)
    {
        if (studentId == Guid.Empty)
        {
            throw new DomainException("StudentAdvisorAssignment.StudentRequired");
        }

        if (instructorId == Guid.Empty)
        {
            throw new DomainException("StudentAdvisorAssignment.InstructorRequired");
        }

        StudentId = studentId;
        InstructorId = instructorId;
        AssignedDateUtc = assignedDateUtc;
        IsActive = isActive;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
