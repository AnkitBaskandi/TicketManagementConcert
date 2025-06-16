using EventManagement.Infrastructure;
using MediatR;
using Shared.Common;
using Shared.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace EventManagement.Application;

public class UpsertTicketTypeCommand : IRequest<int>
{
    public TicketType TicketType { get; set; }
}

public class UpsertTicketTypeCommandHandler : IRequestHandler<UpsertTicketTypeCommand, int>
{
    private readonly IEventRepository _eventRepository;

    public UpsertTicketTypeCommandHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<int> Handle(UpsertTicketTypeCommand request, CancellationToken cancellationToken)
    {
        var eventExists = await _eventRepository.EventExistsAsync(request.TicketType.EventId);
        if (!eventExists)
            throw new Exception("Invalid EventId");

        return await _eventRepository.UpsertTicketTypeAsync(request.TicketType);
    }
}