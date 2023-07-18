using System.Diagnostics.CodeAnalysis;

namespace FormatParser.Text;

public interface ITextBasedFormatDetector
{
    public bool TryMatchFormat(string text, [NotNullWhen(true)] out string? clarifiedEncoding);
    
    public string MimeType { get; }
}