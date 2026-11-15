using RentBuild.Platform.Profiles.Domain.Model.ValueObjects;
namespace RentBuild.Platform.Profiles.Interfaces.Rest.Resources;

public sealed record ProfileResource(int Id, int UserId, string FirstName, string LastName, string ContactEmail, string PhoneNumber, string CompanyName, CompanyAddress? Address);