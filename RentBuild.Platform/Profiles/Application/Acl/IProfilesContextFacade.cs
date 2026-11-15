namespace RentBuild.Platform.Profiles.Application.Acl;

public interface IProfilesContextFacade
{
    Task Register(int userId, string first, string last, string email, string company);
}