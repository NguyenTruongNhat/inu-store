using INustore.UseCase.Exceptions;
using INuStore.API.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json;

namespace INuStore.Tests;

public class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_When_Credentials_Are_Invalid_Should_Return_401_With_Error_Response()
    {
        var context = new DefaultHttpContext();
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        var middleware = new ExceptionHandlingMiddleware(NullLogger<ExceptionHandlingMiddleware>.Instance);

        await middleware.InvokeAsync(context, _ => Task.FromException(new InvalidCredentialsException()));

        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        responseBody.Position = 0;
        using var response = await JsonDocument.ParseAsync(responseBody);
        var root = response.RootElement;

        Assert.Equal("Authentication Failure", root.GetProperty("title").GetString());
        Assert.Equal(StatusCodes.Status401Unauthorized, root.GetProperty("status").GetInt32());
        Assert.Equal("Invalid username or password.", root.GetProperty("detail").GetString());
        Assert.Equal(JsonValueKind.Null, root.GetProperty("errors").ValueKind);
    }
}
