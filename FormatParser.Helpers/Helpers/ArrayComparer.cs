namespace FormatParser.Helpers;

public class ArrayComparer<T> : IEqualityComparer<T[]> where T : IEquatable<T>
{
    private static readonly IEqualityComparer<T> ElementComparer = EqualityComparer<T>.Default;
    public static readonly ArrayComparer<T> Instance = new ();
    
    public bool Equals(T[]? x, T[]? y)
    {
        if (x == null || y == null)
            return ReferenceEquals(x, y);

        return x.SequenceEqual(y);
    }

    public int GetHashCode(T[] array) => array.Aggregate(array.Length, (current, element) => (current * 443) ^ ElementComparer.GetHashCode(element));
}