using FormatParser.Domain;

namespace FormatParser.PE;

public record PeFileFormatInfo(Bitness Bitness, Architecture Architecture, bool IsManaged) : IFileFormatInfo
{
    public bool Equals(IFileFormatInfo? other) => other is PeFileFormatInfo peFileFormatInfo && Equals(peFileFormatInfo);
    
    public string ToPrettyString() => $"PE: {Architecture.ToStringWithoutBitness()}/{Bitness.ToPrettyString()} managed: {IsManaged}";
}