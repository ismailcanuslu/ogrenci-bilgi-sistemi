namespace ImportExport.Domain;

public enum ImportStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    PartiallyCompleted = 4,
    Failed = 5
}
