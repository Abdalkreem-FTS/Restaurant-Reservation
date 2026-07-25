namespace RestaurantReservation.Demos;

public sealed class ViewDemo(Func<RestaurantReservationDbContext> newContext)
{
    public async Task RunAsync(SampleIds ids, CancellationToken cancellationToken)
    {
        await using var context = newContext();

        ConsoleWriter.Step("GetReservationDetails() - view vw_ReservationDetails");

        var reservationDetails = await new ReservationRepository(context).GetReservationDetailsAsync(cancellationToken);

        ConsoleWriter.Table(
            ["ReservationId", "Date", "Party", "Customer", "Customer email", "Restaurant", "Restaurant address"],
            reservationDetails.Select(detail => new[]
            {
                detail.ReservationId.ToString(),
                detail.ReservationDate.ToString("yyyy-MM-dd HH:mm"),
                detail.PartySize.ToString(),
                $"{detail.CustomerFirstName} {detail.CustomerLastName}",
                detail.CustomerEmail,
                detail.RestaurantName,
                detail.RestaurantAddress
            }));

        ConsoleWriter.Step("GetEmployeesWithRestaurant() - view vw_EmployeeDetails");

        var employeeDetails = await new EmployeeRepository(context).GetEmployeesWithRestaurantAsync(cancellationToken);

        ConsoleWriter.Table(
            ["EmployeeId", "Name", "Position", "Restaurant", "Address", "Phone", "Opening hours"],
            employeeDetails.Select(detail => new[]
            {
                detail.EmployeeId.ToString(),
                $"{detail.FirstName} {detail.LastName}",
                detail.Position,
                detail.RestaurantName,
                detail.RestaurantAddress,
                detail.RestaurantPhoneNumber,
                detail.RestaurantOpeningHours
            }));
    }
}
