using MediatR;
using Shared.Common;
using Shared.Contracts;
using System.Threading;
using System.Threading.Tasks;
using VenueCapacity.Infrastructure;

namespace VenueCapacity.Application;

public class UpsertVenueCommand : IRequest<int>
{
    public Venue Venue { get; set; }
}

public class UpsertVenueCommandHandler : IRequestHandler<UpsertVenueCommand, int>
{
    private readonly IVenueRepository _venueRepository;

    public UpsertVenueCommandHandler(IVenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }

    public async Task<int> Handle(UpsertVenueCommand request, CancellationToken cancellationToken)
    {
        return await _venueRepository.UpsertVenueAsync(request.Venue);
    }
}