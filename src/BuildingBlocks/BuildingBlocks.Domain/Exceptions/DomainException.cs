namespace BuildingBlocks.Domain.Exceptions;

public class DomainException : Exception
{
    public string Code { get; }

    public DomainException(string code)
        : base(code)
    {
        Code = code;
    }

    public DomainException(string code, Exception innerException)
        : base(code, innerException)
    {
        Code = code;
    }
}