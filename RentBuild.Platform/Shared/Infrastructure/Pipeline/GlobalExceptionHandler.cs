using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using RentBuild.Platform.Shared.Domain.Model.Errors;
using RentBuild.Platform.Shared.Interfaces.Rest;
namespace RentBuild.Platform.Shared.Infrastructure.Pipeline;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        DomainRuleException? rule = exception as DomainRuleException;
        if (exception is DbUpdateException)
            rule = new("PERSISTENCE_CONFLICT", "A concurrent change or duplicate value prevents this operation", ErrorKind.Conflict);
        if (rule is not null)
        {
            var result = ResultActionAssembler.Error(rule);
            context.Response.StatusCode = result.StatusCode!.Value;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(result.Value, ct);
            return true;
        }
        logger.LogError(exception, "Unexpected request failure");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new
        {
            status = 500,
            title = "Server error",
            detail = "The operation could not be completed"
        }, ct);
        return true;
    }
}
