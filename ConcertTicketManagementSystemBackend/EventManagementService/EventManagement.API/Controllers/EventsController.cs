using MediatR;
using Microsoft.AspNetCore.Mvc;
using EventManagement.Application;
using Shared.Contracts;
using Shared.Common;

namespace EventManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EventsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> UpsertEvent([FromBody] ConcertEvent concertEvent)
    {
        var command = new UpsertEventCommand { ConcertEvent = concertEvent };
        var eventId = await _mediator.Send(command);
        return Ok(new { EventId = eventId });
    }

    [HttpPost("ticket-types")]
    public async Task<IActionResult> UpsertTicketType([FromBody] TicketType ticketType)
    {
        var ticketTypeId = await _mediator.Send(new UpsertTicketTypeCommand { TicketType = ticketType });
        return Ok(new { TicketTypeId = ticketTypeId });
    }
}