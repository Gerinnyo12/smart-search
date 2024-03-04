namespace GriffSoft.SmartSearch.Logic.Extensions;
internal static class StringExtensions
{
    public static string SurroundWith(this string text, string surroundText) =>
        $"{surroundText}{text}{surroundText}";
}
