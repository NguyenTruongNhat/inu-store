using FluentValidation;
using INuStore.UseCase.Abstractions.Notifications;
using INuStore.UseCase.Abstractions.Services;
using INuStore.UseCase.EmailTemplates;
using INuStore.UseCase.UseCases.V1.ApplicationParameterService;
using INuStore.UseCase.UseCases.V1.AuthService;
using INuStore.UseCase.UseCases.V1.FileService;
using INuStore.UseCase.UseCases.V1.NotificationService;
using INuStore.UseCase.UseCases.V1.PermissionService;
using INuStore.UseCase.UseCases.V1.RoleService;
using INuStore.UseCase.UseCases.V1.UserService;
using Microsoft.Extensions.DependencyInjection;

namespace INuStore.UseCase.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUseCase(this IServiceCollection services)
    {
        services.AddAppServices();
        services.AddAppValidators();
        services.AddAppMappings();

        return services;
    }

    public static IServiceCollection AddAppServices(this IServiceCollection services)
    => services.AddScoped<IUserService, UserService>()
               .AddScoped<IRoleService, RoleService>()
               .AddScoped<IPermissionService, PermissionService>()
               .AddScoped<IApplicationParameterService, ApplicationParameterService>()
               .AddTransient<IAuthService, AuthService>()
               .AddScoped<FirstLoginNotificationService>()
               .AddScoped<IEmailTemplateRenderer, EmailTemplateRenderer>()
               .AddScoped<FileService>()
               ;

    public static IServiceCollection AddAppMappings(this IServiceCollection services)
    => services.AddAutoMapper(UseCase.AssemblyReference.Assembly);

    public static IServiceCollection AddAppValidators(this IServiceCollection services)
    => services.AddValidatorsFromAssembly(UseCase.AssemblyReference.Assembly, includeInternalTypes: true);

    public static IServiceCollection AddNotificationWorkerUseCase(
        this IServiceCollection services)
        => services.AddScoped<PendingNotificationService>();
}
