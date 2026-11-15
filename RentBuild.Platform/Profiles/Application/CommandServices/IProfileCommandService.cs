using RentBuild.Platform.Shared.Application.Model;
using RentBuild.Platform.Profiles.Domain.Model.Aggregates;
using RentBuild.Platform.Profiles.Domain.Model.Commands;
namespace RentBuild.Platform.Profiles.Application.CommandServices;

public interface IProfileCommandService
{
    Task<Result<CompanyProfile>> Handle(UpdateProfileCommand command); Task<Result<CompanyProfile>> Create(UpdateProfileCommand command);
}
