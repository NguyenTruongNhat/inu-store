using FluentValidation;
using INuStore.Contract.Service.V1.Role;

namespace INuStore.UseCase.UseCases.V1.RoleService.Validators;

public sealed class CreateRoleValidator : AbstractValidator<Command.CreateRole>
{
    public CreateRoleValidator()
    {
        RuleFor(request => request.Name).NotEmpty()
            .Must(name => name is null || name.Trim().Length <= 256)
            .WithMessage("Name must not exceed 256 characters.");
    }
}

public sealed class UpdateRoleValidator : AbstractValidator<Command.UpdateRole>
{
    public UpdateRoleValidator()
    {
        RuleFor(request => request.RoleId).GreaterThan(0);
        RuleFor(request => request.Name).NotEmpty()
            .Must(name => name is null || name.Trim().Length <= 256)
            .WithMessage("Name must not exceed 256 characters.");
    }
}

public sealed class DeleteRoleValidator : AbstractValidator<Command.DeleteRole>
{
    public DeleteRoleValidator()
    {
        RuleFor(request => request.RoleId).GreaterThan(0);
    }
}

public sealed class GetRoleByIdValidator : AbstractValidator<Query.GetRoleById>
{
    public GetRoleByIdValidator()
    {
        RuleFor(request => request.RoleId).GreaterThan(0);
    }
}

public sealed class RoleIdRouteValidator : AbstractValidator<Request.RoleIdRoute>
{
    public RoleIdRouteValidator()
    {
        RuleFor(request => request.RoleId).GreaterThan(0);
    }
}

public sealed class RolePermissionRouteValidator : AbstractValidator<Request.RolePermissionRoute>
{
    public RolePermissionRouteValidator()
    {
        RuleFor(request => request.RoleId).GreaterThan(0);
        RuleFor(request => request.PermissionId).GreaterThan(0);
    }
}

public sealed class AssignPermissionsValidator : AbstractValidator<Command.AssignPermissions>
{
    public AssignPermissionsValidator()
    {
        RuleFor(request => request.PermissionIds)
            .NotEmpty().WithMessage("At least one permission is required.");
        RuleForEach(request => request.PermissionIds)
            .GreaterThan(0).WithMessage("PermissionId must be greater than 0.");
    }
}
