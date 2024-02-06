namespace GriffSoft.SmartSearch.Database.Extensions;
public static class StringExtensions
{
    public static string Bracketise(this string value) => $"[{value}]";

    public static string Quotise(this string value) => $"'{value}'";
}
