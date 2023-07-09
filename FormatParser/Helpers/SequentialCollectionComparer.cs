namespace FormatParser.Helpers;

public class SequentialCollectionComparer<T> : IEqualityComparer<ICollection<T>> where T : IEquatable<T>
{
    private static readonly IEqualityComparer<T> elementComparer = EqualityComparer<T>.Default;
    public static readonly SequentialCollectionComparer<T> Instance = new ();

    public bool Equals(ICollection<T>? x, ICollection<T>? y)
    {
        if (x == null || y == null)
            return ReferenceEquals(x, y);

        return x.SequenceEqual(y);
    }

    public int GetHashCode(ICollection<T>  array) => array.Aggregate(array.Count, (current, element) => (current * 443) ^ elementComparer.GetHashCode(element));
}