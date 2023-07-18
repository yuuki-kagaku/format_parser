namespace FormatParser.Domain;

public interface IFileFormatInfo : IEquatable<IFileFormatInfo>
{
    string ToPrettyString();
}