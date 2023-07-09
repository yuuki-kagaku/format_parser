namespace FormatParser.Helpers;

public static class ArraySegmentExtensions
{
    public static ArraySegment<T> GetSubSegment<T>(this ArraySegment<T> arraySegment, int count)
    {
        if (count > arraySegment.Count)
            throw new Exception("Can not create subsegment, larger then original segment.");

        return new ArraySegment<T>(arraySegment.Array!, arraySegment.Offset, count);
    }
}