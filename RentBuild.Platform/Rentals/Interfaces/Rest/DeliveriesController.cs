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

[ApiController, Authorize, Route("api/v1/deliveries")]
public sealed class DeliveriesController(IRentalCommandService commands, IRentalQueryService queries) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> List() => Ok((await queries.Deliveries()).Select(RentalAssembler.Resource));
    [HttpPost]
    public async Task<IActionResult> Create(DeliveryResource r)
    {
        Rule.Require(r.Id == 0, "New operations must have no id");
        return (await commands.Handle(new RegisterDeliveryCommand(r.RentalId, r.DeliveredAt, r.Notes))).ToAction(RentalAssembler.Resource, 201);
    }
}
