using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;
using MediatR;
using Academic.Application.Abstractions;
using Academic.Domain.Students;

namespace Academic.Application.Features.StudentPortal;

public sealed record GetStudentBankDetailsQuery : IRequest<ServiceResult<StudentBankDetailsDto>>;

public sealed record UpdateStudentBankDetailsCommand(
    string BankName,
    string Iban,
    string AccountHolderName) : IRequest<ServiceResult<StudentBankDetailsDto>>;

public sealed record StudentBankDetailsDto(
    Guid StudentId,
    string BankName,
    string Iban,
    string AccountHolderName,
    DateTime UpdatedAtUtc);

public sealed class UpdateStudentBankDetailsValidator : AbstractValidator<UpdateStudentBankDetailsCommand>
{
    public UpdateStudentBankDetailsValidator()
    {
        RuleFor(x => x.BankName).NotEmpty().MaximumLength(StudentBankDetails.BankNameMaxLength);
        RuleFor(x => x.Iban).NotEmpty().MaximumLength(StudentBankDetails.IbanMaxLength);
        RuleFor(x => x.AccountHolderName).NotEmpty().MaximumLength(StudentBankDetails.AccountHolderMaxLength);
    }
}

public sealed class ManageStudentBankDetailsHandler(
    IStudentPortalRepository portalRepository,
    IAcademicUnitOfWork unitOfWork,
    ICurrentUser currentUser) :
    IRequestHandler<GetStudentBankDetailsQuery, ServiceResult<StudentBankDetailsDto>>,
    IRequestHandler<UpdateStudentBankDetailsCommand, ServiceResult<StudentBankDetailsDto>>
{
    public async Task<ServiceResult<StudentBankDetailsDto>> Handle(
        GetStudentBankDetailsQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<StudentBankDetailsDto>.Unauthorized(
                new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var student = await portalRepository.GetStudentByUserIdAsync(userId, cancellationToken);
        if (student is null)
        {
            return ServiceResult<StudentBankDetailsDto>.NotFound(
                new Error("Student.NotFound", ErrorType.NotFound));
        }

        var bank = await portalRepository.GetBankDetailsAsync(student.Id, cancellationToken);
        if (bank is null)
        {
            return ServiceResult<StudentBankDetailsDto>.Ok(new StudentBankDetailsDto(
                student.Id,
                string.Empty,
                string.Empty,
                $"{student.FirstName} {student.LastName}",
                DateTime.UtcNow));
        }

        return ServiceResult<StudentBankDetailsDto>.Ok(new StudentBankDetailsDto(
            bank.StudentId,
            bank.BankName,
            bank.Iban,
            bank.AccountHolderName,
            bank.UpdatedAtUtc));
    }

    public async Task<ServiceResult<StudentBankDetailsDto>> Handle(
        UpdateStudentBankDetailsCommand request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<StudentBankDetailsDto>.Unauthorized(
                new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var student = await portalRepository.GetStudentByUserIdAsync(userId, cancellationToken);
        if (student is null)
        {
            return ServiceResult<StudentBankDetailsDto>.NotFound(
                new Error("Student.NotFound", ErrorType.NotFound));
        }

        var bankDetails = new StudentBankDetails(
            Guid.NewGuid(),
            student.Id,
            request.BankName,
            request.Iban,
            request.AccountHolderName,
            DateTime.UtcNow);

        await portalRepository.SaveBankDetailsAsync(bankDetails, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ServiceResult<StudentBankDetailsDto>.Ok(new StudentBankDetailsDto(
            student.Id,
            bankDetails.BankName,
            bankDetails.Iban,
            bankDetails.AccountHolderName,
            bankDetails.UpdatedAtUtc));
    }
}
