using INustore.UseCase.Abstractions.Repositories;
using INustore.UseCase.Abstractions.Security;
using INustore.UseCase.Abstractions.UnitOfWork;
using INustore.UseCase.Exceptions;
using INustore.UseCase.UseCases.V1.AuthService;
using INuStore.Contract.Service.V1.Auth;
using INuStore.Domain.Entities.Identity;
using Moq;
using System.Security.Claims;

namespace INuStore.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IJwtTokenService> _jwtTokenService = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(unit => unit.UserRepository).Returns(_userRepository.Object);

        _service = new AuthService(_jwtTokenService.Object, unitOfWork.Object, _passwordHasher.Object);
    }

    [Fact]
    public async Task LoginAsync_When_User_Does_Not_Exist_Should_Throw_InvalidCredentialsException()
    {
        var request = new Query.Login("missing-user", "password");
        _userRepository
            .Setup(repository => repository.GetUserByUserNameAsync(request.UserName, CancellationToken.None))
            .ReturnsAsync((User?)null);

        var exception = await Assert.ThrowsAsync<InvalidCredentialsException>(() => _service.LoginAsync(request));

        Assert.Equal("Invalid username or password.", exception.Message);
        _jwtTokenService.Verify(service => service.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>()), Times.Never);
        _jwtTokenService.Verify(service => service.GenerateRefreshToken(), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_When_Password_Is_Invalid_Should_Throw_InvalidCredentialsException()
    {
        var request = new Query.Login("test-user", "wrong-password");
        var user = new User { Id = 8, UserName = request.UserName, PasswordHash = "stored-hash" };
        _userRepository
            .Setup(repository => repository.GetUserByUserNameAsync(request.UserName, CancellationToken.None))
            .ReturnsAsync(user);
        _passwordHasher.Setup(hasher => hasher.Verify(request.Password, user.PasswordHash)).Returns(false);

        var exception = await Assert.ThrowsAsync<InvalidCredentialsException>(() => _service.LoginAsync(request));

        Assert.Equal("Invalid username or password.", exception.Message);
        _jwtTokenService.Verify(service => service.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>()), Times.Never);
        _jwtTokenService.Verify(service => service.GenerateRefreshToken(), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_When_Credentials_Are_Valid_Should_Return_Tokens()
    {
        var request = new Query.Login("test-user", "correct-password");
        var user = new User
        {
            Id = 8,
            UserName = request.UserName,
            Email = "test@example.com",
            PasswordHash = "stored-hash"
        };
        _userRepository
            .Setup(repository => repository.GetUserByUserNameAsync(request.UserName, CancellationToken.None))
            .ReturnsAsync(user);
        _passwordHasher.Setup(hasher => hasher.Verify(request.Password, user.PasswordHash)).Returns(true);
        _jwtTokenService
            .Setup(service => service.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>()))
            .Returns("access-token");
        _jwtTokenService.Setup(service => service.GenerateRefreshToken()).Returns("refresh-token");

        var result = await _service.LoginAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal("access-token", result.Value.AccessToken);
        Assert.Equal("refresh-token", result.Value.RefreshToken);
    }
}
