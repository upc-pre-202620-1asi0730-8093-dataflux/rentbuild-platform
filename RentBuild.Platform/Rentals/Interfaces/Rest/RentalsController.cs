using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RentBuild.Platform.Rentals.Application.QueryServices;
using RentBuild.Platform.Rentals.Interfaces.Rest.Resources;
using RentBuild.Platform.Rentals.Interfaces.Rest.Transform;
using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Rentals.Interfaces.Rest;

[ApiController, Authorize, Route("api/v1/rentals")]
public sealed class RentalsController(IRentalQueryService queries) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> List() => Ok((await queries.Rentals()).Select(RentalAssembler.Resource));
    [HttpGet("{id:int}")] public async Task<IActionResult> Find(int id) => Ok(RentalAssembler.Resource(await queries.Rental(id)));
    [HttpPost] public IActionResult Create() => throw new DomainRuleException("DIRECT_RENTAL_FORBIDDEN", "Approve a rental request; the server creates its rental atomically", ErrorKind.Conflict);
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, RentalResource r)
    {
        var current = RentalAssembler.Resource(await queries.Rental(id));
        Rule.Require(current == r, "Use deliveries or equipment-returns for rental transitions", ErrorKind.Conflict);
        return Ok(current);
    }
}
