using FluentValidation;
using INuStore.Contract.Service.V1.User;

namespace INustore.UseCase.UseCases.V1.UserService.Validators
{
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
