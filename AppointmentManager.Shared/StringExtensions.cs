namespace AppointmentManager.Shared;

public static class StringExtensions
{
    public static string SubstringFromChar(this string arg, char c, int startIndexFromChar = 1)
    {
        if (!arg.Contains(c)) return arg;

        var i = arg.IndexOf(c);
        var result = arg[(i + startIndexFromChar)..];

        return result;
    }
}