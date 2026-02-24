[TestClass]
public class SqlInjectionTests
{
    [TestMethod]
    public void Test_SQLInjection_Prevention()
    {
        var maliciousInput = "'; DROP TABLE Users; --";
        var sanitized = InputSanitizer.Sanitize(maliciousInput);
        Assert.IsFalse(sanitized.Contains("DROP TABLE"));
    }
}
