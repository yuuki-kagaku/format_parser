namespace FormatParser.Domain;

public record EncodingInfo(string Name, Endianness Endianness, bool ContainsBom)
{
    private static readonly StringComparer StringComparer = StringComparer.InvariantCultureIgnoreCase;

    public virtual bool Equals(EncodingInfo? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return StringComparer.Equals(Name, other.Name) && Endianness == other.Endianness && ContainsBom == other.ContainsBom;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StringComparer.GetHashCode(Name), (int)Endianness, ContainsBom);
    }
}