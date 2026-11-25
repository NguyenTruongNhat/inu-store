using AutoMapper;
using INuStore.Contract.Abstractions.Share;
using INuStore.Contract.Service.V1.ApplicationParameter;
using INuStore.Domain.Entities;
using INuStore.Domain.Exceptions;
using INuStore.UseCase.Abstractions.Repositories;
using INuStore.UseCase.Abstractions.Services;

namespace INuStore.UseCase.UseCases.V1.ApplicationParameterService;

public sealed class ApplicationParameterService(IApplicationParameterRepository repository, IMapper mapper) : IApplicationParameterService
{
    public async Task<Result<Response.GetByKeyResponse>> GetByKeyAsync(Query.GetByKey request, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Key);
        var key = request.Key.Trim();
        var parameter = await repository.GetByKeyAsync(key, cancellationToken)
            ?? throw new ApplicationParameterException.ApplicationParameterNotFoundException(key);

        var response = mapper.Map<Response.ApplicationParameterResponse>(parameter);
        return Result.Success(new Response.GetByKeyResponse(response));
    }

    public async Task<Result> CreateAsync(Command.Create request, CancellationToken cancellationToken = default)
    {
        var key = request.Key.Trim();
        var existingParameter = await repository.GetByKeyAsync(key, cancellationToken);
        if (existingParameter is not null)
        {
            throw new ApplicationParameterException.DuplicateApplicationParameterKeyException(key);
        }

        repository.Add(new ApplicationParameter { Key = key, Value = request.Value });
        return Result.Success();
    }

    public async Task<Result> UpdateAsync(Command.Update request, CancellationToken cancellationToken = default)
    {
        var key = request.Key.Trim();
        var parameter = await repository.GetByKeyAsync(key, cancellationToken)
            ?? throw new ApplicationParameterException.ApplicationParameterNotFoundException(key);

        parameter.Value = request.Value;
        repository.Update(parameter);
        return Result.Success();
    }
}
