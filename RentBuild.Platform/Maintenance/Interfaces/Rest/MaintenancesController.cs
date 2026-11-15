using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RentBuild.Platform.Maintenance.Application.CommandServices;
using RentBuild.Platform.Maintenance.Application.QueryServices;
using RentBuild.Platform.Maintenance.Domain.Model.Commands;
using RentBuild.Platform.Maintenance.Interfaces.Rest.Resources;
using RentBuild.Platform.Maintenance.Interfaces.Rest.Transform;
using RentBuild.Platform.Shared.Interfaces.Rest;
using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Maintenance.Interfaces.Rest;

[ApiController, Authorize, Route("api/v1/maintenances")]
public sealed class MaintenancesController(IMaintenanceCommandService commands, IMaintenanceQueryService queries) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> List([FromQuery] int? equipmentId) => Ok((await queries.Records(equipmentId)).Select(MaintenanceAssembler.Resource));
    [HttpGet("{id:int}")] public async Task<IActionResult> Find(int id) => Ok(MaintenanceAssembler.Resource(await queries.Record(id)));
    [HttpPost]
    public async Task<IActionResult> Create(MaintenanceResource r)
    {
        Rule.Require(r.Id == 0, "New maintenance must have no id");
        return (await commands.Handle(MaintenanceAssembler.Command(r))).ToAction(MaintenanceAssembler.Resource, 201);
    }
    [HttpPatch("{id:int}")] public async Task<IActionResult> Advance(int id, MaintenancePatchResource r) => (await commands.Handle(new AdvanceMaintenanceCommand(id, r.Status, r.Technician, r.Acknowledge, r.Notes))).ToAction(MaintenanceAssembler.Resource);
    [HttpGet("{id:int}/activities")] public async Task<IActionResult> Activities(int id) => Ok((await queries.Activities(id)).Select(a => new { a.Id, a.MaintenanceId, a.OccurredAt, a.Status, a.Technician, a.Notes }));
    [HttpGet("equipment/{equipmentId:int}/history")] public async Task<IActionResult> History(int equipmentId) => Ok(await queries.History(equipmentId));
}
