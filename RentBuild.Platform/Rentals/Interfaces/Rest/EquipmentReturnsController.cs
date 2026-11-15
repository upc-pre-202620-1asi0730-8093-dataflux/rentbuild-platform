using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RentBuild.Platform.Rentals.Application.CommandServices;
using RentBuild.Platform.Rentals.Application.QueryServices;
using RentBuild.Platform.Rentals.Domain.Model.Commands;
using RentBuild.Platform.Rentals.Interfaces.Rest.Resources;
using RentBuild.Platform.Rentals.Interfaces.Rest.Transform;
using RentBuild.Platform.Shared.Interfaces.Rest;
using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Rentals.Interfaces.Rest;

[ApiController, Authorize, Route("api/v1/equipment-returns")]
public sealed class EquipmentReturnsController(IRentalCommandService commands, IRentalQueryService queries) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> List() => Ok((await queries.Returns()).Select(RentalAssembler.Resource));
    [HttpPost]
    public async Task<IActionResult> Create(ReturnResource r)
    {
        Rule.Require(r.Id == 0, "New operations must have no id");
        return (await commands.Handle(new RegisterReturnCommand(r.RentalId, r.ReturnedAt, r.Notes, r.MaintenanceRequired))).ToAction(RentalAssembler.Resource, 201);
    }
}
