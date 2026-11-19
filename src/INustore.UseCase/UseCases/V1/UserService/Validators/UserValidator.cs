using FluentValidation;
using INuStore.Contract.Service.V1.User;
using INuStore.UseCase.Validation;

namespace INuStore.UseCase.UseCases.V1.UserService.Validators;

public class UpdateUserValidator : AbstractValidator<Command.UpdateUser>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("UserId must be greater than 0.");

        RuleFor(x => x.Email == null ? null : x.Email.Trim())
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email is required.")
            .MaximumLength(254).WithMessage("Email must not exceed 254 characters.")
            .EmailAddress().WithMessage("Email must be a valid email address.")
            .OverridePropertyName(nameof(Command.UpdateUser.Email));
    }
}


public class CreateUserValidator : AbstractValidator<Command.CreateUser>
{
    public CreateUserValidator()
    {
        AccountValidationRules.UserName(RuleFor(request => request.UserName == null ? null : request.UserName.Trim()));
        AccountValidationRules.Email(RuleFor(request => request.Email == null ? null : request.Email.Trim()));
        AccountValidationRules.Password(RuleFor(request => request.Password));
    }
}



public class GetUserByIdValidator : AbstractValidator<Query.GetUserById>
{
    public GetUserByIdValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(7).WithMessage("UserId must be greater than 7.");
    }
}
