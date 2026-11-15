using RentBuild.Platform.Subscriptions.Application.Internal.OutboundServices.Acl;
using RentBuild.Platform.Iam.Application.Acl;
namespace RentBuild.Platform.Subscriptions.Infrastructure.Acl.Iam;

public sealed class SubscriberIdentityAclAdapter(IIdentityContextFacade identity) : ISubscriberIdentity
{
    public async Task<bool> IsActiveRentalCompany(int id)
    {
        var account = await identity.Find(id);
        return account is { Status: "active", Role: "rental_company" };
    }
}
