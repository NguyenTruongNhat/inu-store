using INuStore.Infrastructure;

using INustore.UseCase.Abstractions.UnitOfWork;

namespace INuStore.API.Middlewares;

public sealed class TransactionMiddleware : IMiddleware
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public TransactionMiddleware(ApplicationDbContext dbContext, IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
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
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
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
