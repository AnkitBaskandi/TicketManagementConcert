using Dapper;
using Microsoft.Data.SqlClient;
using Shared.Common;
using Shared.Contracts;
using System.Data;
using System.Threading.Tasks;

namespace TicketManagement.Infrastructure;

public interface ITicketRepository
{
    Task<int> ReserveTicketsAsync(Reservation reservation);
    Task<bool> PurchaseTicketsAsync(int reservationId, string paymentReference);
    Task<bool> TicketTypeExistsAsync(int ticketTypeId);
}

public class TicketRepository : ITicketRepository
{
    private readonly IDbConnection _dbConnection;

    public TicketRepository(string connectionString)
    {
        _dbConnection = new SqlConnection(connectionString);
    }

    public async Task<int> ReserveTicketsAsync(Reservation reservation)
    {
        var parameters = new
        {
            reservation.TicketTypeId,
            reservation.Quantity,
            reservation.UserId,
            ReservationTime = DateTime.UtcNow,
            ExpiryTime = DateTime.UtcNow.AddMinutes(15),
            Status = "Reserved"
        };

        return await _dbConnection.ExecuteScalarAsync<int>(
            "Tickets.sp_ReserveTickets",
            parameters,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> PurchaseTicketsAsync(int reservationId, string paymentReference)
    {
        var parameters = new { ReservationId = reservationId, PaymentReference = paymentReference };
        return await _dbConnection.ExecuteAsync(
            "Tickets.sp_PurchaseTickets",
            parameters,
            commandType: CommandType.StoredProcedure) > 0;
    }

    public async Task<bool> TicketTypeExistsAsync(int ticketTypeId)
    {
        return await _dbConnection.ExecuteScalarAsync<bool>(
            "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM Tickets.TicketTypes WHERE TicketTypeId = @TicketTypeId) THEN 1 ELSE 0 END AS BIT)",
            new { TicketTypeId = ticketTypeId });
    }
}