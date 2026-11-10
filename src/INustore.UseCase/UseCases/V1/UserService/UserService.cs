using AutoMapper;
using INustore.UseCase.Abstractions.Security;
using INustore.UseCase.Abstractions.Services;
using INustore.UseCase.Abstractions.UnitOfWork;
using INuStore.Contract.Abstractions.Share;
using INuStore.Contract.Service.V1.User;
using INuStore.Domain.Entities.Identity;
using INuStore.Domain.Exceptions;

namespace INustore.UseCase.UseCases.V1.UserService;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<Response.GetAllUsersResponse>> GetAllUsers(Query.GetAllUsers request, CancellationToken cancellationToken = default)
    {
        var users = _unitOfWork.UserRepository.FindAll();
        var userResponses = _mapper.Map<List<Response.UserResponse>>(users);
        return Result.Success(new Response.GetAllUsersResponse(userResponses));
    }

    public async Task<Result<Response.GetUserByIdResponse>> GetUserById(Query.GetUserById request, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.UserRepository.FindByIdAsync(request.UserId, cancellationToken)
            ?? throw new UserException.UserNotFoundException(request.UserId);
        var userResponse = _mapper.Map<Response.UserResponse>(user);
        return Result.Success(new Response.GetUserByIdResponse(userResponse));
    }

    public async Task<Result> CreateUser(Command.CreateUser request, CancellationToken cancellationToken = default)
    {
        var user = _mapper.Map<User>(request);

        user.PasswordHash = _passwordHasher.Hash(request.Password);
        _unitOfWork.UserRepository.Add(user);

        return Result.Success();
    }

    public async Task<Result> DeleteUser(Command.DeleteUser request, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.UserRepository.FindByIdAsync(request.UserId);
        if (user != null)
        {
            _unitOfWork.UserRepository.Remove(user);
        }

        return Result.Success();
    }

    public async Task<Result> UpdateUser(Command.UpdateUser request, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.UserRepository.FindByIdAsync(request.UserId, cancellationToken);
        if (user != null)
        {
            user.UserName = request.UserName;
            user.Email = request.Email;
            _unitOfWork.UserRepository.Update(user);
        }

        return Result.Success();
    }

    public async Task<Result<Response.GetUserRolesResponse>> GetUserRoles(int userId, CancellationToken cancellationToken = default)
    {
        var user = await GetUserWithRoleAsync(userId, cancellationToken);
        var roles = user.UserRoles.OrderBy(ur => ur.Role.Name).Select(
            userRole => new Response.UserRoleResponse(
                userRole.Role.Id,
                userRole.Role.Name,
                userRole.Role.Description))
            .ToList();
        return Result.Success(new Response.GetUserRolesResponse(user.Id, roles));
    }

    public async Task<Result> AssignRoleToUser(Command.AssignRoleToUser request, CancellationToken cancellationToken = default)
    {
        var user = await GetUserWithRoleAsync(request.UserId, cancellationToken);

        var role = await _unitOfWork.RoleRepository.FindByIdAsync(request.RoleId, cancellationToken)
                            ?? throw new RoleException.RoleNotFoundException(request.RoleId);

        var alreadyAssigned = user.UserRoles.Any(userRole => userRole.RoleId == role.Id);

        if (alreadyAssigned)
        {
            return Result.Success();
        }

        var assignment = new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        };

        _unitOfWork.UserRepository.AddUserRole(assignment);

        return Result.Success();
    }

    public async Task<Result> RemoveRoleFromUser(Command.RemoveRoleFromUser request, CancellationToken cancellationToken = default)
    {
        var user = await GetUserWithRoleAsync(request.UserId, cancellationToken);
        var role = await _unitOfWork.RoleRepository.FindByIdAsync(request.RoleId, cancellationToken)
                    ?? throw new RoleException.RoleNotFoundException(request.RoleId);
        var assignedRole = user.UserRoles.FirstOrDefault(ur => ur.RoleId == request.RoleId);
        if (assignedRole == null)
        {
            return Result.Success(); // Role is not assigned to the user, nothing to remove
        }
        _unitOfWork.UserRepository.RemoveUserRole(assignedRole);
        return Result.Success();
    }

    /// <inheritdoc />
    public async Task<Result> ReplaceUserRoles(int userId, Command.ReplaceUserRoles request, CancellationToken cancellationToken = default)
    {
        //#avoiding-dbcontext-threading-issues
        // threading issues can occur when using the same DbContext instance across multiple threads.
        // threre are two await callls. if you use WhenAll, it will run both await calls in parallel, which can lead to threading issues with the DbContext.
        // if there are two call AsNoTracking, you can use WhenAll, but if you are using tracking queries, it's safer to await them sequentially to avoid potential threading issues.

        // check requested roles exist
        var requestedRoleExistingInSystem = await _unitOfWork.RoleRepository.GetByIdsAsync(request.RoleIds, cancellationToken);
        // get user with roles
        var user = await GetUserWithRoleAsync(userId, cancellationToken);
        var userCurrentRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToList();

        var rolesToAdd = requestedRoleExistingInSystem.Where(r => !userCurrentRoleIds.Contains(r.Id)).ToList();
        var rolesToRemove = user.UserRoles.Where(ur => !request.RoleIds.Contains(ur.RoleId)).ToList();

        foreach (var role in rolesToAdd)
        {
            var assignment = new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            };
            _unitOfWork.UserRepository.AddUserRole(assignment);
        }
        foreach (var roleToRemove in rolesToRemove)
        {
            _unitOfWork.UserRepository.RemoveUserRole(roleToRemove);
        }
        return Result.Success();
    }

    private async Task<User> GetUserWithRoleAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.UserRepository.GetUserWithRolesAsync(userId, cancellationToken) ?? throw new UserException.UserNotFoundException(userId);
    }
}

