using Dapper;
using Microsoft.Data.SqlClient;
using Shared.Common;
using Shared.Contracts;
using System.Data;
using System.Threading.Tasks;

namespace VenueCapacity.Infrastructure;

public interface IVenueRepository
{
    Task<int> UpsertVenueAsync(Venue venue);
    Task<Venue> GetVenueAsync(int venueId);
}

public class VenueRepository : IVenueRepository
{
    private readonly IDbConnection _dbConnection;

    public VenueRepository(string connectionString)
    {
        _dbConnection = new SqlConnection(connectionString);
    }

    public async Task<int> UpsertVenueAsync(Venue venue)
    {
        var parameters = new
        {
            venue.VenueId,
            venue.Name,
            venue.Capacity
        };

        return await _dbConnection.ExecuteScalarAsync<int>(
            "Venue.sp_UpsertVenue",
            parameters,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<Venue> GetVenueAsync(int venueId)
    {
        return await _dbConnection.QuerySingleOrDefaultAsync<Venue>(
            "SELECT * FROM Venue.Venues WHERE VenueId = @VenueId",
            new { VenueId = venueId });
    }
}