[TestClass]
public class AuthenticationTests
{
    [TestMethod]
    public void Test_PasswordHashing()
    {
        var auth = new AuthService();
        var hash = auth.HashPassword("securePassword");
        Assert.IsTrue(auth.VerifyPassword("securePassword", hash));
        Assert.IsFalse(auth.VerifyPassword("wrongPassword", hash));
    }
}
