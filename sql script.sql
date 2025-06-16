/* Concert Ticket Management System - Database Setup Script */
/* Creates database, schemas, tables, and stored procedures for ConcertDb */

-- Create Database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'ConcertDb')
BEGIN
    CREATE DATABASE ConcertDb;
END
GO

USE ConcertDb;
GO

-- Create Schemas
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'Events')
    EXEC('CREATE SCHEMA Events');
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'Tickets')
    EXEC('CREATE SCHEMA Tickets');
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'Venue')
    EXEC('CREATE SCHEMA Venue');
GO

-- Drop Tables if they exist (in reverse order due to foreign key dependencies)
IF OBJECT_ID('Tickets.Reservations', 'U') IS NOT NULL
    DROP TABLE Tickets.Reservations;
IF OBJECT_ID('Tickets.TicketTypes', 'U') IS NOT NULL
    DROP TABLE Tickets.TicketTypes;
IF OBJECT_ID('Events.ConcertEvents', 'U') IS NOT NULL
    DROP TABLE Events.ConcertEvents;
IF OBJECT_ID('Venue.Venues', 'U') IS NOT NULL
    DROP TABLE Venue.Venues;
GO

-- Create Tables
-- Venue.Venues
CREATE TABLE Venue.Venues
(
    VenueId INT NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Capacity INT NOT NULL CHECK (Capacity > 0),
    CONSTRAINT PK_Venues PRIMARY KEY (VenueId)
);
GO

-- Events.ConcertEvents
CREATE TABLE Events.ConcertEvents
(
    EventId INT NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Date DATETIME2 NOT NULL,
    VenueId INT NOT NULL,
    Description NVARCHAR(500),
    CONSTRAINT PK_ConcertEvents PRIMARY KEY (EventId),
    CONSTRAINT FK_ConcertEvents_Venues FOREIGN KEY (VenueId) REFERENCES Venue.Venues(VenueId)
);
GO

-- Tickets.TicketTypes
CREATE TABLE Tickets.TicketTypes
(
    TicketTypeId INT NOT NULL,
    EventId INT NOT NULL,
    TypeName NVARCHAR(50) NOT NULL,
    Price DECIMAL(10, 2) NOT NULL CHECK (Price >= 0),
    AvailableQuantity INT NOT NULL CHECK (AvailableQuantity >= 0),
    CONSTRAINT PK_TicketTypes PRIMARY KEY (TicketTypeId),
    CONSTRAINT FK_TicketTypes_ConcertEvents FOREIGN KEY (EventId) REFERENCES Events.ConcertEvents(EventId)
);
GO

-- Tickets.Reservations
CREATE TABLE Tickets.Reservations
(
    ReservationId INT NOT NULL,
    TicketTypeId INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    UserId NVARCHAR(50) NOT NULL,
    ReservationTime DATETIME2 NOT NULL,
    ExpiryTime DATETIME2 NOT NULL,
    Status NVARCHAR(20) NOT NULL CHECK (Status IN ('Reserved', 'Purchased', 'Cancelled')),
    PaymentReference NVARCHAR(100),
    CONSTRAINT PK_Reservations PRIMARY KEY (ReservationId),
    CONSTRAINT FK_Reservations_TicketTypes FOREIGN KEY (TicketTypeId) REFERENCES Tickets.TicketTypes(TicketTypeId)
);
GO

-- Drop Stored Procedures if they exist
IF OBJECT_ID('Events.sp_UpsertConcertEvent', 'P') IS NOT NULL
    DROP PROCEDURE Events.sp_UpsertConcertEvent;
IF OBJECT_ID('Tickets.sp_UpsertTicketType', 'P') IS NOT NULL
    DROP PROCEDURE Tickets.sp_UpsertTicketType;
IF OBJECT_ID('Venue.sp_UpsertVenue', 'P') IS NOT NULL
    DROP PROCEDURE Venue.sp_UpsertVenue;
IF OBJECT_ID('Tickets.sp_ReserveTickets', 'P') IS NOT NULL
    DROP PROCEDURE Tickets.sp_ReserveTickets;
IF OBJECT_ID('Tickets.sp_PurchaseTickets', 'P') IS NOT NULL
    DROP PROCEDURE Tickets.sp_PurchaseTickets;
GO

-- Create Stored Procedures
-- sp_UpsertVenue
CREATE PROCEDURE Venue.sp_UpsertVenue
    @VenueId INT,
    @Name NVARCHAR(100),
    @Capacity INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Venue.Venues WHERE VenueId = @VenueId)
    BEGIN
        UPDATE Venue.Venues
        SET Name = @Name,
            Capacity = @Capacity
        WHERE VenueId = @VenueId;
    END
    ELSE
    BEGIN
        INSERT INTO Venue.Venues (VenueId, Name, Capacity)
        VALUES (@VenueId, @Name, @Capacity);
    END

    SELECT @VenueId AS VenueId;
END
GO

-- sp_UpsertConcertEvent
CREATE PROCEDURE Events.sp_UpsertConcertEvent
    @EventId INT,
    @Name NVARCHAR(100),
    @Date DATETIME2,
    @VenueId INT,
    @Description NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Venue.Venues WHERE VenueId = @VenueId)
        THROW 50001, 'Invalid VenueId', 1;

    IF EXISTS (SELECT 1 FROM Events.ConcertEvents WHERE EventId = @EventId)
    BEGIN
        UPDATE Events.ConcertEvents
        SET Name = @Name,
            Date = @Date,
            VenueId = @VenueId,
            Description = @Description
        WHERE EventId = @EventId;
    END
    ELSE
    BEGIN
        INSERT INTO Events.ConcertEvents (EventId, Name, Date, VenueId, Description)
        VALUES (@EventId, @Name, @Date, @VenueId, @Description);
    END

    SELECT @EventId AS EventId;
END
GO

-- sp_UpsertTicketType
CREATE PROCEDURE Tickets.sp_UpsertTicketType
    @TicketTypeId INT,
    @EventId INT,
    @TypeName NVARCHAR(50),
    @Price DECIMAL(10, 2),
    @AvailableQuantity INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Events.ConcertEvents WHERE EventId = @EventId)
        THROW 50001, 'Invalid EventId', 1;

    IF EXISTS (SELECT 1 FROM Tickets.TicketTypes WHERE TicketTypeId = @TicketTypeId)
    BEGIN
        UPDATE Tickets.TicketTypes
        SET EventId = @EventId,
            TypeName = @TypeName,
            Price = @Price,
            AvailableQuantity = @AvailableQuantity
        WHERE TicketTypeId = @TicketTypeId;
    END
    ELSE
    BEGIN
        INSERT INTO Tickets.TicketTypes (TicketTypeId, EventId, TypeName, Price, AvailableQuantity)
        VALUES (@TicketTypeId, @EventId, @TypeName, @Price, @AvailableQuantity);
    END

    SELECT @TicketTypeId AS TicketTypeId;
END
GO

-- sp_ReserveTickets
CREATE PROCEDURE Tickets.sp_ReserveTickets
    @TicketTypeId INT,
    @Quantity INT,
    @UserId NVARCHAR(50),
    @ReservationTime DATETIME2,
    @ExpiryTime DATETIME2,
    @Status NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Tickets.TicketTypes WHERE TicketTypeId = @TicketTypeId)
        THROW 50001, 'Invalid TicketTypeId', 1;

    IF @Quantity > (SELECT AvailableQuantity FROM Tickets.TicketTypes WHERE TicketTypeId = @TicketTypeId)
        THROW 50002, 'Insufficient ticket availability', 1;

    BEGIN TRANSACTION;

    -- Update available quantity
    UPDATE Tickets.TicketTypes
    SET AvailableQuantity = AvailableQuantity - @Quantity
    WHERE TicketTypeId = @TicketTypeId;

    -- Insert reservation
    DECLARE @ReservationId INT;
    INSERT INTO Tickets.Reservations (ReservationId, TicketTypeId, Quantity, UserId, ReservationTime, ExpiryTime, Status)
    VALUES (NEXT VALUE FOR Tickets.ReservationSequence, @TicketTypeId, @Quantity, @UserId, @ReservationTime, @ExpiryTime, @Status);
    
    SET @ReservationId = SCOPE_IDENTITY();

    COMMIT TRANSACTION;

    SELECT @ReservationId AS ReservationId;
END
GO

-- sp_PurchaseTickets
CREATE PROCEDURE Tickets.sp_PurchaseTickets
    @ReservationId INT,
    @PaymentReference NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Tickets.Reservations WHERE ReservationId = @ReservationId AND Status = 'Reserved')
        THROW 50001, 'Invalid or non-reserved ReservationId', 1;

    UPDATE Tickets.Reservations
    SET Status = 'Purchased',
        PaymentReference = @PaymentReference
    WHERE ReservationId = @ReservationId;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- Create Sequence for ReservationId (if not exists)
IF NOT EXISTS (SELECT * FROM sys.sequences WHERE name = N'ReservationSequence')
    CREATE SEQUENCE Tickets.ReservationSequence AS INT START WITH 1 INCREMENT BY 1;
GO

-- Optional: Insert Sample Data for Testing
INSERT INTO Venue.Venues (VenueId, Name, Capacity)
VALUES (1, 'Main Arena', 5000),
       (2, 'City Hall', 2000);
GO

INSERT INTO Events.ConcertEvents (EventId, Name, Date, VenueId, Description)
VALUES (1, 'Rock Fest', '2025-12-25 19:00:00', 1, 'Annual rock concert'),
       (2, 'Jazz Night', '2025-11-15 20:00:00', 2, 'Smooth jazz evening');
GO

INSERT INTO Tickets.TicketTypes (TicketTypeId, EventId, TypeName, Price, AvailableQuantity)
VALUES (1, 1, 'VIP', 100.00, 50),
       (2, 1, 'General', 50.00, 200),
       (3, 2, 'Standard', 30.00, 100);
GO