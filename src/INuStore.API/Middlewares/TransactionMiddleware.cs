using INuStore.Infrastructure;

namespace INuStore.API.Middlewares
{
    public sealed class TransactionMiddleware : IMiddleware
    {
        private readonly ApplicationDbContext _dbContext;

        public TransactionMiddleware(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task InvokeAsync(
            HttpContext context,
            RequestDelegate next)
        {
            // Chỉ áp dụng transaction cho request ghi dữ liệu.
            var method = context.Request.Method;

            if (!HttpMethods.IsPost(method)
                && !HttpMethods.IsPut(method)
                && !HttpMethods.IsPatch(method)
                && !HttpMethods.IsDelete(method))
            {
                await next(context);
                return;
            }

            var cancellationToken = context.RequestAborted;
            var originalBody = context.Response.Body;

            // Giữ response lại để commit xong mới gửi về client.
            await using var responseBuffer = new MemoryStream();
            context.Response.Body = responseBuffer;

            try
            {
                await using (var transaction =
                    await _dbContext.Database.BeginTransactionAsync(cancellationToken))
                {
                    await next(context);

                    if (context.Response.StatusCode >= 400)
                    {
                        await transaction.RollbackAsync(CancellationToken.None);
                    }
                    else
                    {
                        await _dbContext.SaveChangesAsync(cancellationToken);
                        await transaction.CommitAsync(cancellationToken);
                    }
                }

                responseBuffer.Position = 0;

                await responseBuffer.CopyToAsync(
                    originalBody,
                    cancellationToken);
            }
            finally
            {
                context.Response.Body = originalBody;
            }
        }
    }
}
