namespace RentBuild.Platform.Rentals.Application.Acl;

public sealed record RentalHistoryItem(int Id, DateTimeOffset OccurredAt, string Description, string Status);
public interface IRentalsContextFacade
{
    Task<bool> HasActiveRental(int equipmentId); Task<List<RentalHistoryItem>> History(int equipmentId);
}
