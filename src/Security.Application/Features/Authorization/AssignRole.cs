using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using BuildingBlocks.Domain.Authorization;
using FluentValidation;
using Security.Application.Abstractions.Persistence;
using Security.Domain.Authorization;
using PermissionCatalog = BuildingBlocks.Application.Abstractions.Security.Permissions;

namespace Security.Application.Features.Authorization;

public static class AssignRole
{
    public const string UserNotFoundCode = "User.NotFound";

    public const string RoleNotFoundCode = "Role.NotFound";

    public const string AlreadyAssignedCode = "RoleAssignment.AlreadyAssigned";

    public sealed record Command(
        Guid UserId,
        Guid RoleId,
        ScopeType ScopeType,
        Guid? ScopeId)
        : ICommand<ServiceResult<Response>>;

    public sealed record Response(
        Guid Id,
        Guid UserId,
        Guid RoleId,
        ScopeType ScopeType,
        Guid? ScopeId,
        DateTime AssignedAtUtc);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.RoleId).NotEmpty();
            RuleFor(x => x.ScopeType).IsInEnum();
        }
    }

    public sealed class Handler(
        IPermissionGuard permissionGuard,
        ICurrentUser currentUser,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
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

            var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user is null)
            {
                return ServiceResult<Response>.NotFound(
                    new Error(UserNotFoundCode, ErrorType.NotFound, nameof(Command.UserId)));
            }

            var role = await roleRepository.GetByIdAsync(request.RoleId, cancellationToken);

            if (role is null)
            {
                return ServiceResult<Response>.NotFound(
                    new Error(RoleNotFoundCode, ErrorType.NotFound, nameof(Command.RoleId)));
            }

            if (await assignmentRepository.ExistsActiveAsync(
                    request.UserId, request.RoleId, request.ScopeType, request.ScopeId, cancellationToken))
            {
                return ServiceResult<Response>.Conflict(
                    new Error(AlreadyAssignedCode, ErrorType.Conflict));
            }

            var assignment = UserRoleAssignment.Create(
                request.UserId,
                request.RoleId,
                request.ScopeType,
                request.ScopeId,
                DateTime.UtcNow,
                currentUser.UserId);

            await assignmentRepository.AddAsync(assignment, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await auditService.RecordAsync(
                "RoleAssignment.Created",
                "Security",
                "UserRoleAssignment",
                assignment.Id.ToString(),
                newValues: new
                {
                    assignment.UserId,
                    assignment.RoleId,
                    RoleName = role.Name,
                    assignment.ScopeType,
                    assignment.ScopeId
                },
                cancellationToken: cancellationToken);

            return ServiceResult<Response>.Created(
                new Response(
                    assignment.Id,
                    assignment.UserId,
                    assignment.RoleId,
                    assignment.ScopeType,
                    assignment.ScopeId,
                    assignment.AssignedAtUtc));
        }
    }
}
