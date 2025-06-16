using MediatR;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.Application;
using Shared.Contracts;
using Shared.Common;

namespace TicketManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TicketsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("reserve")]
    public async Task<IActionResult> ReserveTickets([FromBody] Reservation reservation)
    {
        var command = new ReserveTicketsCommand { Reservation = reservation };
        var reservationId = await _mediator.Send(command);
        return Ok(new { ReservationId = reservationId });
    }

    [HttpPost("{reservationId}/purchase")]
    public async Task<IActionResult> PurchaseTickets(int reservationId, [FromBody] PaymentRequest paymentRequest)
    {
        var paymentResponse = new PaymentResponse { Success = true, PaymentReference = Guid.NewGuid().ToString() };
        if (!paymentResponse.Success)
            return BadRequest("Payment failed.");

        var command = new PurchaseTicketsCommand { ReservationId = reservationId, PaymentReference = paymentResponse.PaymentReference };
        var success = await _mediator.Send(command);
        return success ? Ok() : BadRequest("Purchase failed.");
    }
}