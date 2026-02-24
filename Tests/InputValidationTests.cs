[TestClass]
public class InputValidationTests
{
    [TestMethod]
    public void Test_XSS_Prevention()
    {
        var maliciousInput = "<script>alert('XSS');</script>";
        var sanitized = InputSanitizer.Sanitize(maliciousInput);
        Assert.IsFalse(sanitized.Contains("<script>"));
    }
}
