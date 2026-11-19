using FluentValidation;

namespace INuStore.UseCase.Validation;

internal static class AccountValidationRules
{
    public static void UserName<T>(IRuleBuilderInitial<T, string?> rule)
    {
        rule.Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("UserName is required.")
            .MinimumLength(3).WithMessage("UserName must be at least 3 characters long.")
            .MaximumLength(50).WithMessage("UserName must not exceed 50 characters.")
            .Must(userName => userName!.All(character => char.IsAsciiLetterOrDigit(character)
                || character is '.' or '_' or '-'))
            .WithMessage("UserName may contain only ASCII letters, digits, '.', '_' and '-'.")
            .OverridePropertyName("UserName");
    }

    public static void Email<T>(IRuleBuilderInitial<T, string?> rule)
    {
        rule.Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email is required.")
            .MaximumLength(254).WithMessage("Email must not exceed 254 characters.")
            .EmailAddress().WithMessage("Email must be a valid email address.")
            .OverridePropertyName("Email");
    }

    public static void Password<T>(IRuleBuilderInitial<T, string> rule)
    {
        rule.Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .MaximumLength(72).WithMessage("Password must not exceed 72 characters.")
            .Matches(@"\A[a-zA-Z0-9]+\z")
                .WithMessage("Password may contain only letters (a-z, A-Z) and digits (0-9).");
    }
}
