namespace FormatParser;

public interface IFileFormatInfo : IEquatable<IFileFormatInfo>
{
    string ToPrettyString();
}