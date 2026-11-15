using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RentBuild.Platform.Inventory.Application.CommandServices;
using RentBuild.Platform.Inventory.Application.QueryServices;
using RentBuild.Platform.Inventory.Domain.Model.Commands;
using RentBuild.Platform.Inventory.Interfaces.Rest.Resources;
using RentBuild.Platform.Inventory.Interfaces.Rest.Transform;
using RentBuild.Platform.Shared.Interfaces.Rest;
using RentBuild.Platform.Shared.Domain.Model.Errors;
using RentBuild.Platform.Shared.Domain.Model.ValueObjects;
namespace RentBuild.Platform.Inventory.Interfaces.Rest;

[ApiController, Authorize, Route("api/v1/equipment")]
public sealed class EquipmentController(IEquipmentCommandService commands, IEquipmentQueryService queries) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> List([FromQuery] int? userId) => Ok((await queries.List(userId)).Select(EquipmentAssembler.Resource));
    [HttpGet("{id:int}")] public async Task<IActionResult> Find(int id) => Ok(EquipmentAssembler.Resource(await queries.Find(id)));
    [HttpPost]
    public async Task<IActionResult> Create(EquipmentResource r)
    {
        Rule.Require(r.Id == 0 && (r.Status is null or "AVAILABLE") && (r.AvailabilityBlocks is null || r.AvailabilityBlocks.Count == 0), "New equipment cannot provide operational state or reservations");
        return (await commands.Handle(new CreateEquipmentCommand(r.UserId, EquipmentAssembler.Fields(r)))).ToAction(EquipmentAssembler.Resource, 201);
    }
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Update(int id, EquipmentResource r)
    {
        Rule.Require(r.Status is null && r.AvailabilityBlocks is null, "Operational state is not editable here");
        return (await commands.Handle(new UpdateEquipmentCommand(id, EquipmentAssembler.Fields(r)))).ToAction(EquipmentAssembler.Resource);
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await commands.Handle(new DeleteEquipmentCommand(id));
        return result.IsSuccess ? NoContent() : ResultActionAssembler.Error(result.Error!);
    }
    [HttpGet("{id:int}/availability")] public async Task<IActionResult> Availability(int id, [FromQuery] string startDate, [FromQuery] string endDate) => Ok(new { equipmentId = id, startDate, endDate, available = await queries.Availability(id, BusinessCalendar.Parse(startDate), BusinessCalendar.Parse(endDate)) });
    [HttpPost("{id:int}/availability-blocks")] public async Task<IActionResult> Reserve(int id, ReserveResource r) => (await commands.Handle(new ReserveEquipmentCommand(id, BusinessCalendar.Parse(r.StartDate[..Math.Min(10, r.StartDate.Length)]), BusinessCalendar.Parse(r.EndDate[..Math.Min(10, r.EndDate.Length)])))).ToAction(EquipmentAssembler.Resource, 201);
    [HttpPatch("{id:int}/operational-status")] public async Task<IActionResult> Status(int id, OperationalStatusResource r) => (await commands.Handle(new RestrictEquipmentCommand(id, r.Status))).ToAction(EquipmentAssembler.Resource);
}
[ApiController, Authorize, Route("api/v1/equipment-categories")]
public sealed class EquipmentCategoriesController(IEquipmentQueryService queries) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> List() => Ok((await queries.Categories()).Select(c => new { c.Id, c.Name }));
}
