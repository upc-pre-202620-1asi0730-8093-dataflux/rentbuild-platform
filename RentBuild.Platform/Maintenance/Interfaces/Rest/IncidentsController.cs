using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RentBuild.Platform.Maintenance.Application.CommandServices;
using RentBuild.Platform.Maintenance.Application.QueryServices;
using RentBuild.Platform.Maintenance.Domain.Model.Commands;
using RentBuild.Platform.Maintenance.Interfaces.Rest.Resources;
using RentBuild.Platform.Maintenance.Interfaces.Rest.Transform;
using RentBuild.Platform.Shared.Interfaces.Rest;
using RentBuild.Platform.Shared.Domain.Model.Errors;
using RentBuild.Platform.Shared.Application.OutboundServices;
namespace RentBuild.Platform.Maintenance.Interfaces.Rest;

[ApiController, Authorize, Route("api/v1/incidents")]
public sealed class IncidentsController(IMaintenanceCommandService commands, IMaintenanceQueryService queries, ICurrentActor actor) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int? equipmentId)
    {
        var list = await queries.Incidents(equipmentId);
        if (actor.Role == "construction_company")
            return Ok(list.Select(i => new { i.Id, i.EquipmentId, Description = "", i.ReportedAt, i.BlocksRental, i.Status, i.ResolvedAt }));
        return Ok(list.Select(MaintenanceAssembler.Resource));
    }
    [HttpGet("{id:int}")] public async Task<IActionResult> Find(int id) => Ok(MaintenanceAssembler.Resource(await queries.Incident(id)));
    [HttpPost]
    public async Task<IActionResult> Create(IncidentResource r)
    {
        Rule.Require(r.Id == 0 && r.Status == "OPEN", "New incident must be OPEN and have no id");
        return (await commands.Handle(new ReportIncidentCommand(r.EquipmentId, r.Description, r.BlocksRental))).ToAction(MaintenanceAssembler.Resource, 201);
    }
    [HttpPatch("{id:int}")] public async Task<IActionResult> Patch(int id, IncidentPatchResource r) => (await commands.Handle(new PatchIncidentCommand(id, r.Status, r.BlocksRental))).ToAction(MaintenanceAssembler.Resource);
    [HttpPost("equipment/{equipmentId:int}/reactivate")]
    public async Task<IActionResult> Reactivate(int equipmentId, ReactivationResource r)
    {
        var result = await commands.Handle(new ReactivateEquipmentCommand(equipmentId, r.InspectionConfirmed));
        return result.IsSuccess ? NoContent() : ResultActionAssembler.Error(result.Error!);
    }
}
