namespace RentBuild.Platform.Subscriptions.Application.Acl;

public interface ISubscriptionsContextFacade
{
    Task<bool> HasActive(int userId, int minimumPlan = 1);
}
