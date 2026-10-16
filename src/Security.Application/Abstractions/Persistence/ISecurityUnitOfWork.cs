using BuildingBlocks.Application.Abstractions.Persistence;

namespace Security.Application.Abstractions.Persistence;

/// <summary>
/// Security modülüne özgü unit of work (bkz. IAcademicUnitOfWork açıklaması).
/// </summary>
public interface ISecurityUnitOfWork : IUnitOfWork;
