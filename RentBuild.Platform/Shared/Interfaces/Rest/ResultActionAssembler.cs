using Microsoft.AspNetCore.Mvc;
using RentBuild.Platform.Shared.Application.Model;
using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Shared.Interfaces.Rest;

public static class ResultActionAssembler
{
    public static IActionResult ToAction<T>(this Result<T> result, Func<T, object> transform, int success = 200) => result.IsSuccess ? new ObjectResult(transform(result.Value!)) { StatusCode = success } : Error(result.Error!);
    public static ObjectResult Error(DomainRuleException error)
    {
        int status = error.Kind switch
        {
            ErrorKind.NotFound => 404,
            ErrorKind.Forbidden => 403,
            ErrorKind.Unauthorized => 401,
            ErrorKind.Conflict => 409,
            ErrorKind.Unavailable => 501,
            _ => 400
        };
        var problem = new ProblemDetails { Status = status, Title = error.Kind.ToString(), Detail = error.Message, Type = "https://httpstatuses.com/" + status };
        problem.Extensions["code"] = error.Code;
        return new ObjectResult(problem) { StatusCode = status, ContentTypes = { "application/problem+json" } };
    }
}
