using System.Security.Claims;
using RentBuild.Platform.Shared.Application.OutboundServices;
using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Shared.Infrastructure.Security;

public sealed class CurrentActor(IHttpContextAccessor http) : ICurrentActor
{
    public int UserId => int.TryParse(http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) && id > 0 ? id : throw new DomainRuleException("UNAUTHENTICATED", "Sign in required", ErrorKind.Unauthorized);
    public string Role => http.HttpContext?.User.FindFirstValue(ClaimTypes.Role) ?? "";
    public void RequireRole(string role)
    {
        _ = UserId;
        Rule.Require(Role == role, "Role is not authorized", ErrorKind.Forbidden, "ROLE_FORBIDDEN");
    }
}
