using Microsoft.AspNetCore.Mvc;

namespace INuStore.API.Filters;

public sealed class ValidateAttribute<TModel> : TypeFilterAttribute<ValidationFilter<TModel>> where TModel : class
{
}
