using AutoMapper;
using INuStore.Contract.Service.V1.Permission;
using INuStore.Domain.Entities.Identity;
using INuStore.Domain.Exceptions;
using INuStore.UseCase.Abstractions.Repositories;
using INuStore.UseCase.Abstractions.UnitOfWork;
using Moq;
using PermissionServiceUnderTest = INuStore.UseCase.UseCases.V1.PermissionService.PermissionService;

namespace INuStore.UnitTets.UseCase.PermissionService;

public class PermissionServiceTests
{
    private readonly Mock<IPermissionRepository> _permissionRepository = new();
    private readonly PermissionServiceUnderTest _service;

    public PermissionServiceTests()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();
        unitOfWork.Setup(unit => unit.PermissionRepository).Returns(_permissionRepository.Object);

        _service = new PermissionServiceUnderTest(unitOfWork.Object, mapper.Object);
    }

    [Fact]
    public async Task CreatePermission_WhenNameIsUnique_ShouldAddPermission()
    {
        // Arrange
        var request = new Command.CreatePermission("users.read", "Read users");
        _permissionRepository
            .Setup(repository => repository.PermissionNameExistsAsync(request.Name, null, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var result = await _service.CreatePermission(request);

        // Assert
        Assert.True(result.IsSuccess);
        _permissionRepository.Verify(repository => repository.Add(It.Is<Permission>(permission =>
            permission.Name == request.Name && permission.Description == request.Description)), Times.Once);
    }

    [Fact]
    public async Task CreatePermission_WhenNameExists_ShouldThrowException()
    {
        // Arrange
        var request = new Command.CreatePermission("users.read", "Read users");
        _permissionRepository
            .Setup(repository => repository.PermissionNameExistsAsync(request.Name, null, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var action = () => _service.CreatePermission(request);

        // Assert
        await Assert.ThrowsAsync<PermissionException.DuplicatePermissionNameException>(action);
        _permissionRepository.Verify(repository => repository.Add(It.IsAny<Permission>()), Times.Never);
    }
}
