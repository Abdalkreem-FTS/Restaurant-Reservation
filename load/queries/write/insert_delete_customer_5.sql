DECLARE @email NVARCHAR(200) = CONCAT('load.', NEWID(), '@example.com');
INSERT INTO dbo.Customers (FirstName, LastName, Email, PhoneNumber)
VALUES ('Load', 'Probe', @email, '555-9999');
DELETE FROM dbo.Customers WHERE Email = @email;
