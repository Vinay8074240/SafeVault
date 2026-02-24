using System.Text.RegularExpressions;

public static class InputSanitizer
{
    public static string Sanitize(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        input = Regex.Replace(input, "<.*?>", string.Empty); // strip HTML tags
        return System.Net.WebUtility.HtmlEncode(input);      // encode HTML
    }
}
