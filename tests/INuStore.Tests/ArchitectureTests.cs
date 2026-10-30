using INuStore.Contract.Abstractions.Share;
using INuStore.Domain.Entities.Identity;
using NetArchTest.Rules;

namespace INuStore.Tests;

public class ArchitectureTests
{
    private const string DomainNamespace = "INuStore.Domain";
    private const string ContractNamespace = "INuStore.Contract";
    private const string UseCaseNamespace = "INustore.UseCase";
    private const string InfrastructureNamespace = "INuStore.Infrastructure";
    private const string ApiNamespace = "INuStore.API";

    [Fact]
    public void Domain_Should_Not_Depend_On_Other_Layers()
    {
        var result = Types
            .InAssembly(typeof(User).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                ContractNamespace,
                UseCaseNamespace,
                InfrastructureNamespace,
                ApiNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful,
            $"Types with forbidden dependencies: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Contract_Should_Not_Depend_On_Other_Layers()
    {
        var result = Types
            .InAssembly(typeof(Result).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                DomainNamespace,
                UseCaseNamespace,
                InfrastructureNamespace,
                ApiNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful,
            $"Types with forbidden dependencies: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void UseCase_Should_Not_Depend_On_Infrastructure_Or_Api()
    {
        var result = Types
            .InAssembly(INustore.UseCase.AssemblyReference.Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(InfrastructureNamespace, ApiNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful,
            $"Types with forbidden dependencies: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Infrastructure_Should_Not_Depend_On_Api()
    {
        var result = Types
            .InAssembly(Infrastructure.AssemblyReference.Assembly)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful,
            $"Types with forbidden dependencies: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }
}
