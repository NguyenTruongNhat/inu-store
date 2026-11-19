using System.Text;
using FluentValidation;
using INuStore.Contract.Service.V1.User;

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
        RuleFor(x => x.UserName == null ? null : x.UserName.Trim())
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("UserName is required.")
            .MinimumLength(3).WithMessage("UserName must be at least 3 characters long.")
            .MaximumLength(50).WithMessage("UserName must not exceed 50 characters.")
            .Must(userName => userName!.All(character => char.IsAsciiLetterOrDigit(character)
                || character is '.' or '_' or '-'))
            .WithMessage("UserName may contain only ASCII letters, digits, '.', '_' and '-'.")
            .OverridePropertyName(nameof(Command.CreateUser.UserName));

        RuleFor(x => x.Email == null ? null : x.Email.Trim())
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email is required.")
            .MaximumLength(254).WithMessage("Email must not exceed 254 characters.")
            .EmailAddress().WithMessage("Email must be a valid email address.")
            .OverridePropertyName(nameof(Command.CreateUser.Email));

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Must(password => Encoding.UTF8.GetByteCount(password) <= 72)
                .WithMessage("Password must not exceed 72 UTF-8 bytes.")
            .Must(password => password.Any(char.IsAsciiLetterUpper))
                .WithMessage("Password must contain an uppercase ASCII letter.")
            .Must(password => password.Any(char.IsAsciiLetterLower))
                .WithMessage("Password must contain a lowercase ASCII letter.")
            .Must(password => password.Any(char.IsAsciiDigit))
                .WithMessage("Password must contain an ASCII digit.")
            .Must(password => password.Any(character => char.IsPunctuation(character) || char.IsSymbol(character)))
                .WithMessage("Password must contain a punctuation mark or symbol.");
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
