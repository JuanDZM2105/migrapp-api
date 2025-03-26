namespace migrapp_api.Helpers.Admin
{
    public static class UserAssignmentHelper
    {
        public static bool RequiresAssignments(string userType, bool hasAccessToAllUsers)
        {
            if (userType == "admin") return false;
            return !hasAccessToAllUsers;
        }
    }
}
