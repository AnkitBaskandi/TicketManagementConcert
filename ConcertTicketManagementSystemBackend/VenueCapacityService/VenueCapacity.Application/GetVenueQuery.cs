using MediatR;
using Shared.Common;
using Shared.Contracts;
using System.Threading;
using System.Threading.Tasks;
using VenueCapacity.Infrastructure;

namespace VenueCapacity.Application;

public class GetVenueQuery : IRequest<Venue>
{
    public int VenueId { get; set; }
}

public class GetVenueQueryHandler : IRequestHandler<GetVenueQuery, Venue>
{
    private readonly IVenueRepository _venueRepository;

    public GetVenueQueryHandler(IVenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }

    public async Task<Venue> Handle(GetVenueQuery request, CancellationToken cancellationToken)
    {
        return await _venueRepository.GetVenueAsync(request.VenueId);
    }
}