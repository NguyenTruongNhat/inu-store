using FluentValidation;
using INustore.UseCase.Abstractions.Services;
using INustore.UseCase.UseCases.V1.UserService;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.Contracts;

namespace INustore.UseCase.DependencyInjection.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        => services.AddScoped<IUserService, UserService>();

        public static IServiceCollection AddAppMappings(this IServiceCollection services)
        => services.AddAutoMapper(UseCase.AssemblyReference.Assembly);

        public static IServiceCollection AddAppValidators(this IServiceCollection services)
        => services.AddValidatorsFromAssembly(UseCase.AssemblyReference.Assembly, includeInternalTypes: true);
    }
}
