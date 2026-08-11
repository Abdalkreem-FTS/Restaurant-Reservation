using System.Security.Claims;

namespace RestaurantReservation.API.Authentication;

public static class CallerExtensions
{
    extension(ClaimsPrincipal caller)
    {
        public bool IsStaff() => caller.IsInRole(Roles.Employee);
        public bool IsManager() => caller.IsInRole(Roles.Manager);

        public int? CustomerId() =>
            int.TryParse(caller.FindFirstValue(JwtTokenGenerator.CustomerIdClaimType), out var customerId) ? customerId : null;

        public int? EmployeeId() =>
            int.TryParse(caller.FindFirstValue(JwtTokenGenerator.EmployeeIdClaimType), out var employeeId) ? employeeId : null;

        public bool MayActFor(int customerId) =>
            caller.IsStaff() || caller.CustomerId() == customerId;

        public bool MayReadFiguresFor(int employeeId) => caller.IsManager() || caller.EmployeeId() == employeeId;
    }
}
