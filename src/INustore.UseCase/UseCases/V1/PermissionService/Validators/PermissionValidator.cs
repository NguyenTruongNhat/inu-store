using FluentValidation;
using INuStore.Contract.Service.V1.Permission;

namespace INuStore.UseCase.UseCases.V1.PermissionService.Validators;

public sealed class CreatePermissionValidator : AbstractValidator<Command.CreatePermission>
{
    public CreatePermissionValidator()
    {
        RuleFor(request => request.Name).NotEmpty()
            .Must(name => name is null || name.Trim().Length <= 256)
            .WithMessage("Name must not exceed 256 characters.");
    }
}

public sealed class UpdatePermissionValidator : AbstractValidator<Command.UpdatePermission>
{
    public UpdatePermissionValidator()
    {
        RuleFor(request => request.PermissionId).GreaterThan(0);
        RuleFor(request => request.Name).NotEmpty()
            .Must(name => name is null || name.Trim().Length <= 256)
            .WithMessage("Name must not exceed 256 characters.");
    }
}

public sealed class DeletePermissionValidator : AbstractValidator<Command.DeletePermission>
{
    public DeletePermissionValidator()
    {
        RuleFor(request => request.PermissionId).GreaterThan(0);
    }
}

public sealed class GetPermissionByIdValidator : AbstractValidator<Query.GetPermissionById>
{
    public GetPermissionByIdValidator()
    {
        RuleFor(request => request.PermissionId).GreaterThan(0);
    }
}
