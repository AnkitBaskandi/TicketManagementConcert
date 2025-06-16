using MediatR;
using Shared.Common;
using Shared.Contracts;
using System.Threading;
using System.Threading.Tasks;
using EventManagement.Infrastructure;

namespace EventManagement.Application;

public class UpsertEventCommand : IRequest<int>
{
    public ConcertEvent ConcertEvent { get; set; }
}

public class UpsertEventCommandHandler : IRequestHandler<UpsertEventCommand, int>
{
    private readonly IEventRepository _eventRepository;
    private readonly IServiceHandler<IVenueCapacityService> _venueServiceHandler;

    public UpsertEventCommandHandler(IEventRepository eventRepository, IServiceHandler<IVenueCapacityService> venueServiceHandler)
    {
        _eventRepository = eventRepository;
        _venueServiceHandler = venueServiceHandler;
    }

    public async Task<int> Handle(UpsertEventCommand request, CancellationToken cancellationToken)
    {
        var venue = await _venueServiceHandler.GetAsync<Venue>($"/api/venues/{request.ConcertEvent.VenueId}");
        if (venue == null)
            throw new Exception("Invalid VenueId");

        return await _eventRepository.UpsertConcertEventAsync(request.ConcertEvent);
    }
}