using FluentValidation;
using INuStore.Contract.Abstractions.Share;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace INuStore.API.Filters;

public sealed class ValidationFilter<TModel> : IAsyncActionFilter where TModel : class
{
    private readonly IEnumerable<IValidator<TModel>> _validators;

    public ValidationFilter(IEnumerable<IValidator<TModel>> validators)
    {
        _validators = validators;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var model = context.ActionArguments.Values.OfType<TModel>().FirstOrDefault();
        if (model is null)
        {
            await next();
            return;
        }

        var errors = new List<Error>();
        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(model, context.HttpContext.RequestAborted);
            errors.AddRange(result.Errors
                .Where(failure => failure is not null)
                .Select(failure => new Error(failure.PropertyName, failure.ErrorMessage)));
        }

        if (errors.Count > 0)
        {
            context.Result = new BadRequestObjectResult(ValidationResult.WithErrors(errors.Distinct().ToArray()));
            return;
        }

        await next();
    }
}
