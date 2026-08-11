namespace RestaurantReservation.API.Authentication;

public static class Roles
{
    public const string Customer = nameof(Customer);
    public const string Employee = nameof(Employee);
    public const string Manager = nameof(Manager);

    public static IReadOnlyList<string> For(User user)
    {
        var roles = new List<string>(3);

        if (user.Customer is not null)
        {
            roles.Add(Customer);
        }

        if (user.Employee is not { } employee)
        {
            return roles;
        }

        roles.Add(Employee);
        roles.Add(employee.Position.ToString());

        return roles;
    }
}
