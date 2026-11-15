using RentBuild.Platform.Profiles.Domain.Model.Aggregates;
using RentBuild.Platform.Profiles.Domain.Model.Commands;
using RentBuild.Platform.Profiles.Domain.Model.ValueObjects;
using RentBuild.Platform.Profiles.Interfaces.Rest.Resources;
namespace RentBuild.Platform.Profiles.Interfaces.Rest.Transform;

public static class ProfileAssembler
{
    public static ProfileResource Resource(CompanyProfile p, int actor) => p.UserId == actor ? new(p.Id, p.UserId, p.FirstName, p.LastName, p.ContactEmail, p.PhoneNumber, p.CompanyName, p.Address) : new(p.Id, p.UserId, "", "", "", "", p.CompanyName, null);
    public static UpdateProfileCommand Command(int id, ProfileResource r) => new(id, r.UserId, r.FirstName, r.LastName, r.ContactEmail, r.PhoneNumber, r.CompanyName, r.Address ?? new CompanyAddress());
}