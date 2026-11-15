using RentBuild.Platform.Profiles.Domain.Model.ValueObjects;
namespace RentBuild.Platform.Profiles.Domain.Model.Commands;

public sealed record UpdateProfileCommand(int Id, int UserId, string FirstName, string LastName, string ContactEmail, string PhoneNumber, string CompanyName, CompanyAddress Address);