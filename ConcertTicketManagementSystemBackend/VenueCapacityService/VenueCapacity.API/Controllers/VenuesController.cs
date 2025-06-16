using MediatR;
using Microsoft.AspNetCore.Mvc;
using VenueCapacity.Application;
using Shared.Contracts;
using Shared.Common;

namespace VenueCapacity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VenuesController : ControllerBase
{
    private readonly IMediator _mediator;

    public VenuesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> UpsertVenue([FromBody] Venue venue)
    {
        var command = new UpsertVenueCommand { Venue = venue };
        var venueId = await _mediator.Send(command);
        return Ok(new { VenueId = venueId });
    }

    [HttpGet("{venueId}")]
    public async Task<IActionResult> GetVenue(int venueId)
    {
        var venue = await _mediator.Send(new GetVenueQuery { VenueId = venueId });
        return venue != null ? Ok(venue) : NotFound();
    }
}