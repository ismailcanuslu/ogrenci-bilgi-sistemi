namespace ImportExport.Domain;

public readonly record struct ImportJobId(Guid Value)
{
    public static ImportJobId New() => new(Guid.NewGuid());
    public static ImportJobId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
