using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;
using Security.Application.Abstractions.Persistence;
using PermissionCatalog = BuildingBlocks.Application.Abstractions.Security.Permissions;

namespace Security.Application.Features.Authorization;

public static class RevokeRoleAssignment
{
    public const string NotFoundCode = "RoleAssignment.NotFound";

    public const string AlreadyRevokedCode = "RoleAssignment.AlreadyRevoked";

    public sealed record Command(Guid AssignmentId, string? Reason)
        : ICommand<ServiceResult<Response>>;

    public sealed record Response(Guid Id, DateTime RevokedAtUtc);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.AssignmentId).NotEmpty();
            RuleFor(x => x.Reason).MaximumLength(500);
        }
    }

    public sealed class Handler(
        IPermissionGuard permissionGuard,
        ICurrentUser currentUser,
        IUserRoleAssignmentRepository assignmentRepository,
        ISecurityUnitOfWork unitOfWork,
        IAuditService auditService)
        : ICommandHandler<Command, ServiceResult<Response>>
    {
        public async Task<ServiceResult<Response>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var denied = await permissionGuard.CheckAsync(
                PermissionCatalog.Authorization.RoleManage,
                [ResourceScope.University],
                cancellationToken);

            if (denied is not null)
            {
                return AuthorizationFailure.To<Response>(denied);
            }

            var assignment = await assignmentRepository.GetByIdAsync(
                request.AssignmentId, cancellationToken);

            if (assignment is null)
            {
                return ServiceResult<Response>.NotFound(
                    new Error(NotFoundCode, ErrorType.NotFound));
            }

            if (!assignment.IsActive)
            {
                return ServiceResult<Response>.Conflict(
                    new Error(AlreadyRevokedCode, ErrorType.Conflict));
            }

            var nowUtc = DateTime.UtcNow;
            assignment.Revoke(nowUtc, currentUser.UserId, request.Reason);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            await auditService.RecordAsync(
                "RoleAssignment.Revoked",
                "Security",
                "UserRoleAssignment",
                assignment.Id.ToString(),
                oldValues: new { assignment.UserId, assignment.RoleId, assignment.ScopeType, assignment.ScopeId },
                newValues: new { RevokedAtUtc = nowUtc, Reason = request.Reason },
                cancellationToken: cancellationToken);

            return ServiceResult<Response>.Ok(new Response(assignment.Id, nowUtc));
        }
    }
}
