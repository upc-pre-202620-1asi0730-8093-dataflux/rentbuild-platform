using RentBuild.Platform.Iam.Application.CommandServices;
using RentBuild.Platform.Iam.Domain.Model.Commands;
using RentBuild.Platform.Iam.Interfaces.Rest.Resources;
namespace RentBuild.Platform.Iam.Interfaces.Rest.Transform;

public static class AccountAssembler
{
    public static SignUpCommand Command(SignUpResource r) => new(r.FirstName, r.LastName, r.Email, r.Password, r.CompanyName, r.Role);
    public static SignInCommand Command(SignInResource r) => new(r.Email, r.Password);
    public static UserResource Resource(AuthSession session) => new(session.Id, session.Email, session.Role, session.Status, session.Token);
}