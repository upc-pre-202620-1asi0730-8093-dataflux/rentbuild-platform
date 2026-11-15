using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RentBuild.Platform.Rentals.Application.CommandServices;
using RentBuild.Platform.Rentals.Application.QueryServices;
using RentBuild.Platform.Rentals.Domain.Model.Commands;
using RentBuild.Platform.Rentals.Domain.Model.ValueObjects;
using RentBuild.Platform.Rentals.Interfaces.Rest.Resources;
using RentBuild.Platform.Rentals.Interfaces.Rest.Transform;
using RentBuild.Platform.Shared.Interfaces.Rest;
using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Rentals.Interfaces.Rest;

[ApiController, Authorize, Route("api/v1/rental-requests")]
public sealed class RentalRequestsController(IRentalCommandService commands, IRentalQueryService queries) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> List() => Ok((await queries.Requests()).Select(RentalAssembler.Resource));
    [HttpGet("{id:int}")] public async Task<IActionResult> Find(int id) => Ok(RentalAssembler.Resource(await queries.Request(id)));
    [HttpPost]
    public async Task<IActionResult> Submit(RentalRequestResource r)
    {
        Rule.Require(r.Id == 0 && r.Status == "PENDING", "New request must be pending and have no id");
        return (await commands.Handle(RentalAssembler.Command(r))).ToAction(RentalAssembler.Resource, 201);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Decide(int id, RentalRequestResource r)
    {
        Rule.Require((r.Id == 0 || r.Id == id) && Enum.TryParse<RentalRequestStatus>(r.Status, false, out var target) && Enum.IsDefined(target), "Invalid request decision");
        return (await commands.Handle(new DecideRentalRequestCommand(id, Enum.Parse<RentalRequestStatus>(r.Status)))).ToAction(RentalAssembler.Resource);
    }
    [HttpPatch("{id:int}/period")] public async Task<IActionResult> Period(int id, PeriodResource r) => (await commands.Handle(new RescheduleRentalRequestCommand(id, r.StartDate, r.EndDate))).ToAction(RentalAssembler.Resource);
}
