using INustore.UseCase.Abstractions.Services;
using INustore.UseCase.Abstractions.UnitOfWork;
using INuStore.Contract.Abstractions.Share;
using INuStore.Contract.Service.V1.User;
using INuStore.Domain.Entities.Identity;

namespace INustore.UseCase.UseCases.V1.UserService
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Response.GetAllUsersResponse>> GetAllUsers(Query.GetAllUsers request, CancellationToken cancellationToken = default)
        {
            var users = _unitOfWork.UserRepository.FindAll();
            var userResponses = users.Select(user => new Response.UserResponse(user.Id, user.UserName, user.Email)).ToList();
            return Result.Success(new Response.GetAllUsersResponse(userResponses, userResponses.Count()));
        }

        public async Task<Result<Response.GetUserByIdResponse>> GetUserById(Query.GetUserById request, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.UserRepository.FindByIdAsync(request.UserId);
            var userResponse = new Response.UserResponse(user.Id, user.UserName, user.Email);
            return Result.Success(new Response.GetUserByIdResponse(userResponse));
        }

        public async Task<Result> CreateUser(Command.CreateUser request, CancellationToken cancellationToken = default)
        {
            var user = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                PasswordHash = request.Password
            };
            _unitOfWork.UserRepository.Add(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
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
            var user = await _unitOfWork.UserRepository.FindByIdAsync(request.UserId);
            if (user != null)
            {
                user.UserName = request.UserName;
                user.Email = request.Email;
                _unitOfWork.UserRepository.Update(user);
            }

            return Result.Success();
        }
    }
}
