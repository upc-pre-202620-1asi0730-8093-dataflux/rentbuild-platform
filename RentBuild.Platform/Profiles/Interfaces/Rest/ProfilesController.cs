using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RentBuild.Platform.Profiles.Application.CommandServices;
using RentBuild.Platform.Profiles.Application.QueryServices;
using RentBuild.Platform.Profiles.Interfaces.Rest.Resources;
using RentBuild.Platform.Profiles.Interfaces.Rest.Transform;
using RentBuild.Platform.Shared.Application.OutboundServices;
using RentBuild.Platform.Shared.Interfaces.Rest;
namespace RentBuild.Platform.Profiles.Interfaces.Rest;

[ApiController, Authorize, Route("api/v1/profiles")]
public sealed class ProfilesController(IProfileCommandService commands, IProfileQueryService queries, ICurrentActor actor) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> List([FromQuery] int? userId) => Ok((await queries.List(userId)).Select(p => ProfileAssembler.Resource(p, actor.UserId)));
    [HttpGet("{id:int}")] public async Task<IActionResult> Find(int id) => Ok(ProfileAssembler.Resource(await queries.Find(id), actor.UserId));
    [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id, ProfileResource r) => (await commands.Handle(ProfileAssembler.Command(id, r))).ToAction(p => ProfileAssembler.Resource(p, actor.UserId));
    [HttpPost] public async Task<IActionResult> Create(ProfileResource r) => (await commands.Create(ProfileAssembler.Command(0, r))).ToAction(p => ProfileAssembler.Resource(p, actor.UserId), 201);
}