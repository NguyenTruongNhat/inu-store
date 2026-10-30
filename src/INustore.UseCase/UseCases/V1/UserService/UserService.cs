using INustore.UseCase.Abstractions.Services;
using INustore.UseCase.Abstractions.UnitOfWork;
using AutoMapper;
using INuStore.Contract.Abstractions.Share;
using INuStore.Contract.Service.V1.User;
using INuStore.Domain.Entities.Identity;
using INuStore.Domain.Exceptions;

namespace INustore.UseCase.UseCases.V1.UserService
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
            _unitOfWork.UserRepository.Add(user);

            var user2 = new User { UserName = "nhatnguyen2", Email = "nhatnguyen2@example.com", PasswordHash = "hashedpassword2" };
            _unitOfWork.UserRepository.Add(user2);

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
    }
}
