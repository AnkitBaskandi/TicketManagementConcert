using MediatR;
using Shared.Common;
using Shared.Contracts;
using System.Threading;
using System.Threading.Tasks;
using TicketManagement.Infrastructure;

namespace TicketManagement.Application;

public class ReserveTicketsCommand : IRequest<int>
{
    public Reservation Reservation { get; set; }
}

public class ReserveTicketsCommandHandler : IRequestHandler<ReserveTicketsCommand, int>
{
    private readonly ITicketRepository _ticketRepository;

    public ReserveTicketsCommandHandler(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<int> Handle(ReserveTicketsCommand request, CancellationToken cancellationToken)
    {
        var ticketTypeExists = await _ticketRepository.TicketTypeExistsAsync(request.Reservation.TicketTypeId);
        if (!ticketTypeExists)
            throw new Exception("Invalid TicketTypeId");

        return await _ticketRepository.ReserveTicketsAsync(request.Reservation);
    }
}