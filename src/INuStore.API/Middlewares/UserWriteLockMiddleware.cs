using INuStore.API.Concurrency;

namespace INuStore.API.Middlewares;

public sealed class UserWriteLockMiddleware : IMiddleware, IDisposable
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.GetEndpoint()?.Metadata.GetMetadata<SerializeUserWritesAttribute>() is null)
        {
            await next(context);
            return;
        }

        await _semaphore.WaitAsync(context.RequestAborted);
        try
        {
            // This middleware wraps TransactionMiddleware, so release happens after save/commit/disposal.
            await next(context);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose()
    {
        _semaphore.Dispose();
    }
}
