namespace BuildingBlocks.Application.Abstractions.Security;

public interface ICurrentUser
{
    /// <summary>Kimliği doğrulanmış kullanıcının kimliği; anonim ise null.</summary>
    Guid? UserId { get; }
}
