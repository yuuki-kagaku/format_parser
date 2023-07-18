using System.Text;

namespace FormatParser.Helpers;

public static class ArraySegmentExtensions
{
    public static ArraySegment<T> GetSubSegment<T>(this ArraySegment<T> arraySegment, int count)
    {
        if (count > arraySegment.Count)
            throw new Exception("Can not create subsegment, larger then original segment.");

        return new ArraySegment<T>(arraySegment.Array!, arraySegment.Offset, count);
    }

    public static string AsString(this ArraySegment<char> arraySegment) =>
        new StringBuilder().Append(arraySegment.ToMemory()).ToString();

    public static Memory<T> ToMemory<T>(this ArraySegment<T> arraySegment) => new (arraySegment.Array, arraySegment.Offset, arraySegment.Count);
}