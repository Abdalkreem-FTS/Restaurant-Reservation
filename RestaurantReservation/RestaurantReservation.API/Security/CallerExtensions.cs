using System.Security.Claims;

namespace RestaurantReservation.API.Security;

public static class CallerExtensions
{
    extension(ClaimsPrincipal caller)
    {
        private bool IsStaff() => caller.IsInRole(Roles.Employee);
        private bool IsAdmin() => caller.IsInRole(Roles.Admin);
        private bool IsManager() => caller.IsInRole(Roles.Manager);

        private int? CustomerId() =>
            int.TryParse(caller.FindFirstValue(JwtTokenGenerator.CustomerIdClaimType), out var customerId) ? customerId : null;

        private int? EmployeeId() =>
            int.TryParse(caller.FindFirstValue(JwtTokenGenerator.EmployeeIdClaimType), out var employeeId) ? employeeId : null;

        public bool MayActFor(int customerId) =>
            caller.IsStaff() || caller.CustomerId() == customerId;

        public bool MayReadFiguresFor(int employeeId) =>
            caller.IsAdmin() || caller.IsManager() || caller.EmployeeId() == employeeId;
    }
}
