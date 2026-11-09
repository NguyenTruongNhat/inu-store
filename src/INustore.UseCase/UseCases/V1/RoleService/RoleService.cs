using AutoMapper;
using INustore.UseCase.Abstractions.Services;
using INustore.UseCase.Abstractions.UnitOfWork;
using INuStore.Contract.Abstractions.Share;
using INuStore.Contract.Service.V1.Role;
using INuStore.Domain.Entities.Identity;
using INuStore.Domain.Enums;
using INuStore.Domain.Exceptions;

namespace INustore.UseCase.UseCases.V1.RoleService;

/// <summary>Applies role rules without taking ownership of saving or committing changes.</summary>
public sealed class RoleService(IUnitOfWork unitOfWork, IMapper mapper) : IRoleService
{
    /// <inheritdoc />
    public async Task<Result<Response.GetAllRolesResponse>> GetAllRoles(CancellationToken cancellationToken = default)
    {
        var roles = await unitOfWork.RoleRepository.GetAllAsync(cancellationToken);
        return Result.Success(new Response.GetAllRolesResponse(mapper.Map<List<Response.RoleResponse>>(roles)));
    }

    /// <inheritdoc />
    public async Task<Result<Response.GetRoleByIdResponse>> GetRoleById(Query.GetRoleById request, CancellationToken cancellationToken = default)
    {
        var role = await FindRoleAsync(request.RoleId, cancellationToken);
        return Result.Success(new Response.GetRoleByIdResponse(mapper.Map<Response.RoleResponse>(role)));
    }

    /// <inheritdoc />
    public async Task<Result> CreateRole(Command.CreateRole request, CancellationToken cancellationToken = default)
    {
        var name = request.Name.Trim();
        await EnsureUniqueNameAsync(name, null, cancellationToken);
        // Reserve built-in names even if seeding has not yet created them.
        if (IsBuiltIn(name))
        {
            throw new RoleException.BuiltInRoleException();
        }

        unitOfWork.RoleRepository.Add(mapper.Map<Role>(request));
        return Result.Success();
    }

    /// <inheritdoc />
    public async Task<Result> UpdateRole(Command.UpdateRole request, CancellationToken cancellationToken = default)
    {
        var role = await FindRoleAsync(request.RoleId, cancellationToken);
        var name = request.Name.Trim();
        if ((IsBuiltIn(role.Name) && !string.Equals(role.Name, name, StringComparison.Ordinal))
            || (!IsBuiltIn(role.Name) && IsBuiltIn(name)))
        {
            throw new RoleException.BuiltInRoleException();
        }

        await EnsureUniqueNameAsync(name, role.Id, cancellationToken);
        role.Name = name;
        role.Description = request.Description ?? string.Empty;
        unitOfWork.RoleRepository.Update(role);
        return Result.Success();
    }

    /// <inheritdoc />
    public async Task<Result> DeleteRole(Command.DeleteRole request, CancellationToken cancellationToken = default)
    {
        var role = await FindRoleAsync(request.RoleId, cancellationToken);
        if (IsBuiltIn(role.Name))
        {
            throw new RoleException.BuiltInRoleException();
        }

        if (await unitOfWork.RoleRepository.HasAssignmentsAsync(role.Id, cancellationToken))
        {
            throw new RoleException.RoleAssignedException();
        }

        unitOfWork.RoleRepository.Remove(role);
        return Result.Success();
    }

    private async Task<Role> FindRoleAsync(int roleId, CancellationToken cancellationToken)
        => await unitOfWork.RoleRepository.FindByIdAsync(roleId, cancellationToken)
            ?? throw new RoleException.RoleNotFoundException(roleId);

    private async Task EnsureUniqueNameAsync(string name, int? excludedRoleId, CancellationToken cancellationToken)
    {
        if (await unitOfWork.RoleRepository.NameExistsAsync(name, excludedRoleId, cancellationToken))
        {
            throw new RoleException.DuplicateRoleNameException();
        }
    }

    private static bool IsBuiltIn(string name)
        => RoleType.List.Any(role => string.Equals(role.Name, name, StringComparison.OrdinalIgnoreCase));
}
