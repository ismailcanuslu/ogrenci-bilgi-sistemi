using BuildingBlocks.Application.Abstractions.Persistence;

namespace Academic.Application.Abstractions;

/// <summary>
/// Academic modülüne özgü unit of work. Ortak <see cref="IUnitOfWork"/> birden fazla modül
/// tarafından kaydedildiğinde "son kayıt kazanır" davranışı yanlış DbContext'e yazmaya yol açar.
/// </summary>
public interface IAcademicUnitOfWork : IUnitOfWork;
