using FluentValidation;
using INuStore.Contract.Service.V1.Auth;
using INuStore.UseCase.Validation;

namespace INuStore.UseCase.UseCases.V1.AuthService.Validators;

public sealed class RegisterValidator : AbstractValidator<Command.Register>
{
    public RegisterValidator()
    {
        AccountValidationRules.UserName(RuleFor(request => request.UserName == null ? null : request.UserName.Trim()));
        AccountValidationRules.Email(RuleFor(request => request.Email == null ? null : request.Email.Trim()));
        AccountValidationRules.Password(RuleFor(request => request.Password));
    }
}

public sealed class LoginValidator : AbstractValidator<Query.Login>
{
    public LoginValidator()
    {
        RuleFor(request => request.UserName).NotEmpty();
        RuleFor(request => request.Password).NotEmpty();
    }
}

public sealed class RefreshTokenValidator : AbstractValidator<Query.Token>
{
    public RefreshTokenValidator()
    {
        RuleFor(request => request.AccessToken).NotEmpty();
        RuleFor(request => request.RefreshToken).NotEmpty();
    }
}

public sealed class RevokeTokenValidator : AbstractValidator<Command.Revoke>
{
    public RevokeTokenValidator()
    {
        RuleFor(request => request.AccessToken).NotEmpty();
    }
}
