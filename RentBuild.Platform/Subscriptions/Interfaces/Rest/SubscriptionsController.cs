using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RentBuild.Platform.Subscriptions.Application.QueryServices;
using RentBuild.Platform.Subscriptions.Application.CommandServices;
using RentBuild.Platform.Subscriptions.Domain.Model.Commands;
using RentBuild.Platform.Subscriptions.Interfaces.Rest.Resources;
using RentBuild.Platform.Subscriptions.Interfaces.Rest.Transform;
using RentBuild.Platform.Shared.Interfaces.Rest;
using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Subscriptions.Interfaces.Rest;

[ApiController, Authorize, Route("api/v1/user-subscriptions")]
public sealed class SubscriptionsController(ISubscriptionQueryService queries, ISubscriptionCommandService commands) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> List() => Ok((await queries.Subscriptions()).Select(SubscriptionAssembler.Resource));
    [HttpGet("{id:int}")] public async Task<IActionResult> Get(int id) => Ok(SubscriptionAssembler.Resource(await queries.Find(id)));
    [HttpPost]
    public async Task<IActionResult> Create(SubscriptionResource r)
    {
        Rule.Require(r.Id == 0 && r.Status == "ACTIVE", "New subscriptions must have no id and be ACTIVE");
        return (await commands.Handle(SubscriptionAssembler.Command(r))).ToAction(SubscriptionAssembler.Resource, 201);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Change(int id, SubscriptionResource r)
    {
        Rule.Require(r.Id == 0 || r.Id == id, "Subscription id mismatch");
        return (await commands.Handle(new ChangePlanCommand(id, r.PlanId))).ToAction(SubscriptionAssembler.Resource);
    }
    [HttpPatch("{id:int}/cancel")] public async Task<IActionResult> Cancel(int id) => (await commands.Handle(new CancelSubscriptionCommand(id))).ToAction(SubscriptionAssembler.Resource);
}
[ApiController, Authorize, Route("api/v1/subscription-plans")]
public sealed class SubscriptionPlansController(ISubscriptionQueryService queries) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> List() => Ok((await queries.Plans()).Select(SubscriptionAssembler.Resource));
}
