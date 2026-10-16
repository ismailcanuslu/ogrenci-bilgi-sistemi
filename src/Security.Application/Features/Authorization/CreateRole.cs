using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;
using Security.Application.Abstractions.Persistence;
using Security.Domain.Authorization;
using PermissionCatalog = BuildingBlocks.Application.Abstractions.Security.Permissions;

namespace Security.Application.Features.Authorization;

public static class CreateRole
{
    public const string AlreadyExistsCode = "Role.AlreadyExists";

    public const string UnknownPermissionCode = "Role.UnknownPermission";

    public sealed record Command(
        string Name,
        string? Description,
        IReadOnlyCollection<string> Permissions)
        : ICommand<ServiceResult<Response>>;

    public sealed record Response(
        Guid Id,
        string Name,
        string? Description,
        IReadOnlyCollection<string> Permissions);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(Role.NameMaxLength);

            RuleFor(x => x.Description)
                .MaximumLength(500);

            RuleFor(x => x.Permissions)
                .NotNull();

            RuleForEach(x => x.Permissions)
                .NotEmpty()
                .MaximumLength(RolePermission.PermissionMaxLength);
        }
    }

    public sealed class Handler(
        IPermissionGuard permissionGuard,
        IRoleRepository roleRepository,
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

            var requested = request.Permissions
                .Select(p => p.Trim().ToLowerInvariant())
                .Distinct()
                .ToList();

            var unknown = requested
                .Where(p => !PermissionCatalog.All.Contains(p))
                .ToList();

            if (unknown.Count > 0)
            {
                return ServiceResult<Response>.BadRequest(
                    new Error(UnknownPermissionCode, ErrorType.Validation, nameof(Command.Permissions)));
            }

            if (await roleRepository.GetByNameAsync(request.Name.Trim(), cancellationToken) is not null)
            {
                return ServiceResult<Response>.Conflict(
                    new Error(AlreadyExistsCode, ErrorType.Conflict, nameof(Command.Name)));
            }

            var role = Role.Create(
                request.Name,
                request.Description,
                requested,
                DateTime.UtcNow);

            await roleRepository.AddAsync(role, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await auditService.RecordAsync(
                "Role.Created",
                "Security",
                "Role",
                role.Id.ToString(),
                newValues: new { role.Name, Permissions = requested },
                cancellationToken: cancellationToken);

            return ServiceResult<Response>.Created(
                new Response(
                    role.Id,
                    role.Name,
                    role.Description,
                    role.Permissions.Select(p => p.Permission).OrderBy(p => p).ToArray()));
        }
    }
}
