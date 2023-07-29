namespace FormatParser.Helpers;

public static class IEnumerableExtensions
{
    public static IEnumerable<T> Concat<T>(this IEnumerable<T> enumerable, T element) =>
        enumerable.Concat(new[] { element });
}