using AutoMapper;
using INuStore.Contract.Service.V1.User;
using INuStore.Domain.Entities.Identity;
using INuStore.UseCase.Abstractions.Repositories;
using INuStore.UseCase.Abstractions.Security;
using INuStore.UseCase.Abstractions.UnitOfWork;
using Moq;
using UserServiceUnderTest = INuStore.UseCase.UseCases.V1.UserService.UserService;

namespace INuStore.UnitTets.UseCase.UserService;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly UserServiceUnderTest _service;

    public UserServiceTests()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(unit => unit.UserRepository).Returns(_userRepository.Object);

        _service = new UserServiceUnderTest(unitOfWork.Object, _mapper.Object, _passwordHasher.Object);
    }

    [Fact]
    public async Task CreateUser_WhenRequestIsValid_ShouldAddUserWithHashedPassword()
    {
        // Arrange
        var request = new Command.CreateUser("test-user", "test@example.com", "example-password");
        var mappedUser = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            PasswordHash = request.Password
        };
        const string passwordHash = "hashed-password";
        User? addedUser = null;
        string? passwordHashWhenAdded = null;

        _mapper.Setup(mapper => mapper.Map<User>(request)).Returns(mappedUser);
        _passwordHasher.Setup(hasher => hasher.Hash(request.Password)).Returns(passwordHash);
        _userRepository
            .Setup(repository => repository.Add(It.IsAny<User>()))
            .Callback<User>(user =>
            {
                addedUser = user;
                passwordHashWhenAdded = user.PasswordHash;
            });

        // Act
        var result = await _service.CreateUser(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Same(mappedUser, addedUser);
        Assert.Equal(passwordHash, passwordHashWhenAdded);
        _mapper.Verify(mapper => mapper.Map<User>(request), Times.Once);
        _passwordHasher.Verify(hasher => hasher.Hash(request.Password), Times.Once);
        _userRepository.Verify(repository => repository.Add(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task CreateUser_WhenPasswordHashingFails_ShouldThrowException()
    {
        // Arrange
        var request = new Command.CreateUser("test-user", "test@example.com", "example-password");
        var mappedUser = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            PasswordHash = request.Password
        };
        var hashingException = new InvalidOperationException("Password hashing failed.");

        _mapper.Setup(mapper => mapper.Map<User>(request)).Returns(mappedUser);
        _passwordHasher.Setup(hasher => hasher.Hash(request.Password)).Throws(hashingException);

        // Act
        var action = () => _service.CreateUser(request);

        // Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(action);
        Assert.Same(hashingException, exception);
        _userRepository.Verify(repository => repository.Add(It.IsAny<User>()), Times.Never);
    }
}
