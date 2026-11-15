namespace RentBuild.Platform.Shared.Application.OutboundServices;

public interface ICurrentActor
{
    int UserId
    {
        get;
    }
    string Role
    {
        get;
    }
    void RequireRole(string role);
}
