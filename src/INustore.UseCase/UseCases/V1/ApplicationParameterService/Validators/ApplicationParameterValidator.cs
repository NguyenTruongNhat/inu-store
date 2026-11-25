using FluentValidation;
using INuStore.Contract.Service.V1.ApplicationParameter;

namespace INuStore.UseCase.UseCases.V1.ApplicationParameterService.Validators;

public sealed class GetByKeyValidator : AbstractValidator<Query.GetByKey>
{
    public GetByKeyValidator()
    {
        RuleFor(request => request.Key)
            .NotEmpty()
            .Must(key => key is null || key.Trim().Length <= 256)
            .WithMessage("Key must not exceed 256 characters.");
    }
}

public sealed class CreateValidator : AbstractValidator<Command.Create>
{
    public CreateValidator()
    {
        RuleFor(request => request.Key)
            .NotEmpty()
            .Must(key => key is null || key.Trim().Length <= 256)
            .WithMessage("Key must not exceed 256 characters.");

        RuleFor(request => request.Value).NotNull();
    }
}

public sealed class UpdateValidator : AbstractValidator<Command.Update>
{
    public UpdateValidator()
    {
        RuleFor(request => request.Key)
            .NotEmpty()
            .Must(key => key is null || key.Trim().Length <= 256)
            .WithMessage("Key must not exceed 256 characters.");

        RuleFor(request => request.Value).NotNull();
    }
}
