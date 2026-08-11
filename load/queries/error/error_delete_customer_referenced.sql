DELETE FROM dbo.Customers
WHERE CustomerId = (SELECT TOP (1) CustomerId FROM dbo.Reservations ORDER BY ReservationId);
