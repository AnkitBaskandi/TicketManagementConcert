using Dapper;
using Microsoft.Data.SqlClient;
using Shared.Common;
using System.Data;

namespace EventManagement.Infrastructure;

public interface IEventRepository
{
    Task<int> UpsertConcertEventAsync(ConcertEvent concertEvent);
    Task<int> UpsertTicketTypeAsync(TicketType ticketType);
    Task<bool> EventExistsAsync(int eventId);
}

public class EventRepository : IEventRepository
{
    private readonly IDbConnection _dbConnection;

    public EventRepository(string connectionString)
    {
        _dbConnection = new SqlConnection(connectionString);
    }

    public async Task<int> UpsertConcertEventAsync(ConcertEvent concertEvent)
    {
        var parameters = new
        {
            concertEvent.EventId,
            concertEvent.Name,
            concertEvent.Date,
            concertEvent.VenueId,
            concertEvent.Description
        };

        return await _dbConnection.ExecuteScalarAsync<int>(
            "Events.sp_UpsertConcertEvent",
            parameters,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> UpsertTicketTypeAsync(TicketType ticketType)
    {
        var parameters = new
        {
            ticketType.TicketTypeId,
            ticketType.EventId,
            ticketType.TypeName,
            ticketType.Price,
            ticketType.AvailableQuantity
        };

        return await _dbConnection.ExecuteScalarAsync<int>(
            "Tickets.sp_UpsertTicketType",
            parameters,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> EventExistsAsync(int eventId)
    {
        return await _dbConnection.ExecuteScalarAsync<bool>(
            "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM Events.ConcertEvents WHERE EventId = @EventId) THEN 1 ELSE 0 END AS BIT)",
            new { EventId = eventId });
    }
}