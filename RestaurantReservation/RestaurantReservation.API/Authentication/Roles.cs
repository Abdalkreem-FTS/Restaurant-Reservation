using RestaurantReservation.Db.Enums;

namespace RestaurantReservation.API.Authentication;

public static class Roles
{
    public const string Customer = nameof(Customer);
    public const string Employee = nameof(Employee);

    public const string Admin = nameof(Admin);
    public const string Manager = nameof(Manager);
    public const string VipOrdersWaiter = nameof(VipOrdersWaiter);
    public const string StandardWaiter = nameof(StandardWaiter);
    public const string AssistantWaiter = nameof(AssistantWaiter);

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
        roles.Add(For(employee.Position));

        return roles;
    }

    private static string For(EmployeePosition position) => position switch
    {
        EmployeePosition.Admin => Admin,
        EmployeePosition.Manager => Manager,
        EmployeePosition.VipOrdersWaiter => VipOrdersWaiter,
        EmployeePosition.StandardWaiter => StandardWaiter,
        EmployeePosition.AssistantWaiter => AssistantWaiter,
        _ => throw new ArgumentOutOfRangeException(nameof(position), position, "No role is mapped to this position."),
    };
}
