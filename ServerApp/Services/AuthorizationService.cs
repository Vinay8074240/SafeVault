public class AuthorizationService
{
    public bool CanAccessAdminDashboard(User user)
    {
        return user.Role == "admin";
    }
}
