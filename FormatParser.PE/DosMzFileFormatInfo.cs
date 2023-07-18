using FormatParser.Domain;

namespace FormatParser.PE;

public record DosMzFileFormatInfo() : IFileFormatInfo
{
    public bool Equals(IFileFormatInfo? other) => other is DosMzFileFormatInfo dosMzFileFormatInfo && this.Equals(dosMzFileFormatInfo);
    
    public string ToPrettyString()
    {
        return $"DOS MZ executable";
    }
}