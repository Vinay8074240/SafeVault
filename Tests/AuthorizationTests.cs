[TestClass]
public class AuthorizationTests
{
    [TestMethod]
    public void Test_AdminAccess()
    {
        var user = new User { Username = "Alice", Role = "admin" };
        var authz = new AuthorizationService();
        Assert.IsTrue(authz.CanAccessAdminDashboard(user));
    }

    [TestMethod]
    public void Test_UserAccessDenied()
    {
        var user = new User { Username = "Bob", Role = "user" };
        var authz = new AuthorizationService();
        Assert.IsFalse(authz.CanAccessAdminDashboard(user));
    }
}
