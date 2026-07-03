// Shared/Constants/SystemRoles.cs
namespace HRTaskManagement.Shared.Constants
{
    public static class SystemRoles
    {
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string Employee = "Employee";
        public const string HR = "HR";

        public static readonly string[] All = { Admin, Manager, Employee, HR };
    }
}