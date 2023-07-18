using FormatParser.Domain;

namespace FormatParser.PE;

public record DosMzFileFormatInfo() : IFileFormatInfo
{
    public bool Equals(IFileFormatInfo? other) => other is DosMzFileFormatInfo dosMzFileFormatInfo && Equals(dosMzFileFormatInfo);
    
    public string ToPrettyString() => "DOS MZ executable";
}