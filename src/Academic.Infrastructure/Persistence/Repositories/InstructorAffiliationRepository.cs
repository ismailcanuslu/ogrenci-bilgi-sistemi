using Academic.Application.Abstractions;
using Academic.Domain.Instructors;
using Microsoft.EntityFrameworkCore;

namespace Academic.Infrastructure.Persistence.Repositories;

public sealed class InstructorAffiliationRepository(AcademicDbContext dbContext) : IInstructorAffiliationRepository
{
    public Task<InstructorDepartmentAffiliation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.InstructorDepartmentAffiliations.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<InstructorDepartmentAffiliation>> GetByInstructorIdAsync(
        Guid instructorId, CancellationToken cancellationToken = default)
        => await dbContext.InstructorDepartmentAffiliations
            .Where(x => x.InstructorId == instructorId)
            .OrderByDescending(x => x.IsPrimary)
            .ThenBy(x => x.StartDateUtc)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(InstructorDepartmentAffiliation affiliation, CancellationToken cancellationToken = default)
        => await dbContext.InstructorDepartmentAffiliations.AddAsync(affiliation, cancellationToken);

    public void Remove(InstructorDepartmentAffiliation affiliation)
        => dbContext.InstructorDepartmentAffiliations.Remove(affiliation);
}
