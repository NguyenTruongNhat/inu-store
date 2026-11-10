using FluentValidation;
using INuStore.Contract.Service.V1.User;

namespace INustore.UseCase.UseCases.V1.UserService.Validators
{
    /// <summary>Validates the user identifier for replacing role assignments.</summary>
    public sealed class UserRolesRouteValidator : AbstractValidator<Request.UserRolesRoute>
    {
        /// <summary>Requires a positive user identifier.</summary>
        public UserRolesRouteValidator()
        {
            RuleFor(request => request.UserId)
                .GreaterThan(0).WithMessage("UserId must be greater than 0.");
        }
    }

    /// <summary>Validates the complete replacement list of role identifiers.</summary>
    public sealed class ReplaceUserRolesValidator : AbstractValidator<Command.ReplaceUserRoles>
    {
        /// <summary>Requires at least one role and positive identifiers.</summary>
        public ReplaceUserRolesValidator()
        {
            RuleFor(request => request.RoleIds)
                .NotEmpty().WithMessage("At least one role is required.");
            RuleForEach(request => request.RoleIds)
                .GreaterThan(0).WithMessage("RoleId must be greater than 0.");
        }
    }

    /// <summary>Validates the route identifiers for assigning a role.</summary>
    public sealed class AssignRoleToUserValidator : AbstractValidator<Command.AssignRoleToUser>
    {
        /// <summary>Requires positive user and role identifiers.</summary>
        public AssignRoleToUserValidator()
        {
            RuleFor(request => request.UserId)
                .GreaterThan(0).WithMessage("UserId must be greater than 0.");
            RuleFor(request => request.RoleId)
                .GreaterThan(0).WithMessage("RoleId must be greater than 0.");
        }
    }

    /// <summary>Validates the route identifiers for removing a role assignment.</summary>
    public sealed class RemoveRoleFromUserValidator : AbstractValidator<Command.RemoveRoleFromUser>
    {
        /// <summary>Requires positive user and role identifiers.</summary>
        public RemoveRoleFromUserValidator()
        {
            RuleFor(request => request.UserId)
                .GreaterThan(0).WithMessage("UserId must be greater than 0.");
            RuleFor(request => request.RoleId)
                .GreaterThan(0).WithMessage("RoleId must be greater than 0.");
        }
    }
}
