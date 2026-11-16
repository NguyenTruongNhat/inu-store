using FluentValidation;
using INuStore.Contract.Service.V1.User;

namespace INuStore.UseCase.UseCases.V1.UserService.Validators;

public sealed class UserIdRouteValidator : AbstractValidator<Request.UserIdRoute>
{
    public UserIdRouteValidator()
    {
        RuleFor(request => request.UserId)
            .GreaterThan(0).WithMessage("UserId must be greater than 0.");
    }
}

public sealed class ReplaceUserRolesValidator : AbstractValidator<Command.ReplaceUserRoles>
{
    public ReplaceUserRolesValidator()
    {
        RuleFor(request => request.RoleIds)
            .NotEmpty().WithMessage("At least one role is required.");
        RuleForEach(request => request.RoleIds)
            .GreaterThan(0).WithMessage("RoleId must be greater than 0.");
    }
}

public sealed class AssignRoleToUserValidator : AbstractValidator<Command.AssignRoleToUser>
{
    public AssignRoleToUserValidator()
    {
        RuleFor(request => request.UserId)
            .GreaterThan(0).WithMessage("UserId must be greater than 0.");
        RuleFor(request => request.RoleId)
            .GreaterThan(0).WithMessage("RoleId must be greater than 0.");
    }
}

public sealed class RemoveRoleFromUserValidator : AbstractValidator<Command.RemoveRoleFromUser>
{
    public RemoveRoleFromUserValidator()
    {
        RuleFor(request => request.UserId)
            .GreaterThan(0).WithMessage("UserId must be greater than 0.");
        RuleFor(request => request.RoleId)
            .GreaterThan(0).WithMessage("RoleId must be greater than 0.");
    }
}
