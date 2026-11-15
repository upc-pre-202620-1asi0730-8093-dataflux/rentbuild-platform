using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Shared.Application.Model;

public sealed record Result<T>(T? Value, DomainRuleException? Error)
{
    public bool IsSuccess => Error is null;
    public static Result<T> Success(T value) => new(value, null);
    public static Result<T> Failure(DomainRuleException error) => new(default, error);
}
