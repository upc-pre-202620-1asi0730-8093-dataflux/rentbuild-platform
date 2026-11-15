using RentBuild.Platform.Profiles.Application.CommandServices;
using RentBuild.Platform.Profiles.Domain.Model.Aggregates;
using RentBuild.Platform.Profiles.Domain.Model.Commands;
using RentBuild.Platform.Profiles.Domain.Repositories;
using RentBuild.Platform.Shared.Application.Model;
using RentBuild.Platform.Shared.Application.Internal;
using RentBuild.Platform.Shared.Application.OutboundServices;
using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Profiles.Application.Internal.CommandServices;

public sealed class ProfileCommandService(ICompanyProfileRepository profiles, ICurrentActor actor, CommandExecutor executor) : IProfileCommandService
{
    public Task<Result<CompanyProfile>> Handle(UpdateProfileCommand c) => executor.ExecuteAsync(async () => { var p = Rule.Found(await profiles.FindByIdAsync(c.Id), "Profile"); Rule.Owner(actor.UserId, p.UserId); Rule.Owner(c.UserId, p.UserId); p.Update(c.FirstName, c.LastName, c.ContactEmail, c.PhoneNumber, c.CompanyName, c.Address); return p; });
    public Task<Result<CompanyProfile>> Create(UpdateProfileCommand c) => executor.ExecuteAsync(async () => { Rule.Owner(actor.UserId, c.UserId); Rule.Require(!(await profiles.ListAsync()).Any(p => p.UserId == c.UserId), "A profile already exists", ErrorKind.Conflict); var profile = new CompanyProfile(c.UserId, c.FirstName, c.LastName, c.ContactEmail, c.CompanyName); profile.Update(c.FirstName, c.LastName, c.ContactEmail, c.PhoneNumber, c.CompanyName, c.Address); await profiles.AddAsync(profile); return profile; });
}