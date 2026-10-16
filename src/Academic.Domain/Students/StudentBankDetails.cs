using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.Students;

public sealed class StudentBankDetails : Entity<Guid>
{
    public const int BankNameMaxLength = 100;
    public const int IbanMaxLength = 34;
    public const int AccountHolderMaxLength = 150;

    public Guid StudentId { get; private set; }

    public string BankName { get; private set; } = null!;

    public string Iban { get; private set; } = null!;

    public string AccountHolderName { get; private set; } = null!;

    public DateTime UpdatedAtUtc { get; private set; }

    private StudentBankDetails()
    {
    }

    public StudentBankDetails(
        Guid id,
        Guid studentId,
        string bankName,
        string iban,
        string accountHolderName,
        DateTime updatedAtUtc)
        : base(id)
    {
        if (studentId == Guid.Empty)
        {
            throw new DomainException("StudentBankDetails.StudentRequired");
        }

        Update(bankName, iban, accountHolderName, updatedAtUtc);
        StudentId = studentId;
    }

    public void Update(string bankName, string iban, string accountHolderName, DateTime updatedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(bankName))
        {
            throw new DomainException("StudentBankDetails.BankNameRequired");
        }

        if (string.IsNullOrWhiteSpace(iban))
        {
            throw new DomainException("StudentBankDetails.IbanRequired");
        }

        if (string.IsNullOrWhiteSpace(accountHolderName))
        {
            throw new DomainException("StudentBankDetails.AccountHolderRequired");
        }

        BankName = bankName.Trim();
        Iban = iban.Trim().ToUpperInvariant().Replace(" ", string.Empty);
        AccountHolderName = accountHolderName.Trim();
        UpdatedAtUtc = updatedAtUtc;
    }
}
