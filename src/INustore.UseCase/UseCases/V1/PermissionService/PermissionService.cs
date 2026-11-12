using AutoMapper;
using INuStore.UseCase.Abstractions.Repositories;
using INuStore.UseCase.Abstractions.Services;
using INuStore.UseCase.Abstractions.UnitOfWork;
using INuStore.Contract.Abstractions.Share;
using INuStore.Contract.Service.V1.Permission;
using INuStore.Domain.Entities.Identity;
using INuStore.Domain.Exceptions;

namespace INuStore.UseCase.UseCases.V1.PermissionService;

public sealed class PermissionService(IUnitOfWork unitOfWork, IMapper mapper) : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository = unitOfWork.PermissionRepository;

    public async Task<Result<Response.GetAllPermissionsResponse>> GetAllPermissions(CancellationToken cancellationToken = default)
    {
        var permissions = await _permissionRepository.GetAllAsync(cancellationToken);
        var permissionResponses = mapper.Map<List<Response.PermissionResponse>>(permissions);
        return Result.Success(new Response.GetAllPermissionsResponse(permissionResponses));
    }

    public async Task<Result<Response.GetPermissionByIdResponse>> GetPermissionById(Query.GetPermissionById request, CancellationToken cancellationToken = default)
    {
        var permission = await GetRequiredPermissionAsync(request.PermissionId, cancellationToken);
        var permissionResponse = mapper.Map<Response.PermissionResponse>(permission);
        return Result.Success(new Response.GetPermissionByIdResponse(permissionResponse));
    }

    public async Task<Result> CreatePermission(Command.CreatePermission request, CancellationToken cancellationToken = default)
    {
        var permissionName = request.Name.Trim();
        await EnsurePermissionNameIsUniqueAsync(permissionName, excludedPermissionId: null, cancellationToken);

        var permission = new Permission
        {
            Name = permissionName,
            Description = request.Description ?? string.Empty
        };

        _permissionRepository.Add(permission);
        return Result.Success();
    }

    public async Task<Result> UpdatePermission(Command.UpdatePermission request, CancellationToken cancellationToken = default)
    {
        var permission = await GetRequiredPermissionAsync(request.PermissionId, cancellationToken);
        var permissionName = request.Name.Trim();
        await EnsurePermissionNameIsUniqueAsync(permissionName, permission.Id, cancellationToken);

        permission.Name = permissionName;
        permission.Description = request.Description ?? string.Empty;
        _permissionRepository.Update(permission);
        return Result.Success();
    }

    public async Task<Result> DeletePermission(Command.DeletePermission request, CancellationToken cancellationToken = default)
    {
        var permission = await GetRequiredPermissionAsync(request.PermissionId, cancellationToken);
        var isAssignedToRole = await _permissionRepository.IsAssignedToAnyRoleAsync(permission.Id, cancellationToken);
        if (isAssignedToRole)
        {
            throw new PermissionException.PermissionAssignedException();
        }

        _permissionRepository.Remove(permission);
        return Result.Success();
    }

    private async Task<Permission> GetRequiredPermissionAsync(int permissionId, CancellationToken cancellationToken)
    {
        var permission = await _permissionRepository.FindByIdAsync(permissionId, cancellationToken);
        return permission ?? throw new PermissionException.PermissionNotFoundException(permissionId);
    }

    private async Task EnsurePermissionNameIsUniqueAsync(string permissionName, int? excludedPermissionId, CancellationToken cancellationToken)
    {
        var nameAlreadyExists = await _permissionRepository.PermissionNameExistsAsync(permissionName, excludedPermissionId, cancellationToken);
        if (nameAlreadyExists)
        {
            throw new PermissionException.DuplicatePermissionNameException();
        }
    }
}
