using FluentValidation;
using INustore.UseCase.Abstractions.UnitOfWork;
using INuStore.Contract.Service.V1.User;

namespace INustore.UseCase.UseCases.V1.UserService.Validators
{
    public class UpdateUserValidator : AbstractValidator<Command.UpdateUser>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId must be greater than 0.");
        }
    }


    public class CreateUserValidator : AbstractValidator<Command.CreateUser>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName is required.")
                .MinimumLength(3).WithMessage("UserName must be at least 3 characters long.")
                .MaximumLength(50).WithMessage("UserName must not exceed 50 characters.");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be a valid email address.");
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
}
