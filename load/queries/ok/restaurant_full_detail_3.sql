SELECT r.ReservationId, r.ReservationDate, r.PartySize,
       c.FirstName, c.LastName, o.OrderId, o.TotalAmount,
       m.Name AS MenuItemName, oi.Quantity, oi.UnitPrice
FROM dbo.Reservations AS r
INNER JOIN dbo.Customers AS c ON c.CustomerId = r.CustomerId
LEFT JOIN dbo.Orders AS o ON o.ReservationId = r.ReservationId
LEFT JOIN dbo.OrderItems AS oi ON oi.OrderId = o.OrderId
LEFT JOIN dbo.MenuItems AS m ON m.ItemId = oi.ItemId AND m.RestaurantId = oi.RestaurantId
WHERE r.RestaurantId = 3
ORDER BY r.ReservationId, o.OrderId, oi.OrderItemId;
