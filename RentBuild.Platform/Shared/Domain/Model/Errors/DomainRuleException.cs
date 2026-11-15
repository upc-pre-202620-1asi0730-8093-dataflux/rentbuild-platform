namespace RentBuild.Platform.Shared.Domain.Model.Errors;

public enum ErrorKind
{
    Validation, Conflict, NotFound, Forbidden, Unauthorized, Unavailable
}
public sealed class DomainRuleException(string code, string message, ErrorKind kind = ErrorKind.Validation) : Exception(message)
{
    public string Code { get; } = code; public ErrorKind Kind { get; } = kind;
}
public static class Rule
{
    public static void Require(bool valid, string message, ErrorKind kind = ErrorKind.Validation, string code = "BUSINESS_RULE")
    {
        if (!valid)
            throw new DomainRuleException(code, message, kind);
    }
    public static T Found<T>(T? entity, string name) where T : class => entity ?? throw new DomainRuleException("NOT_FOUND", name + " not found", ErrorKind.NotFound);
    public static string Text(string? value, string name, int max = 200)
    {
        Require(!string.IsNullOrWhiteSpace(value) && value.Trim().Length <= max, name + " is required or too long");
        return value!.Trim();
    }
    public static void Owner(int actor, int owner) => Require(actor == owner, "Resource belongs to another company", ErrorKind.Forbidden, "OWNERSHIP");
}