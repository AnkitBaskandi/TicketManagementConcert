using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TicketManagement.Infrastructure;

namespace TicketManagement.Application;

public class PurchaseTicketsCommand : IRequest<bool>
{
    public int ReservationId { get; set; }
    public string PaymentReference { get; set; }
}

public class PurchaseTicketsCommandHandler : IRequestHandler<PurchaseTicketsCommand, bool>
{
    private readonly ITicketRepository _ticketRepository;

    public PurchaseTicketsCommandHandler(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<bool> Handle(PurchaseTicketsCommand request, CancellationToken cancellationToken)
    {
        return await _ticketRepository.PurchaseTicketsAsync(request.ReservationId, request.PaymentReference);
    }
}