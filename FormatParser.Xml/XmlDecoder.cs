using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using FormatParser.Text;

namespace FormatParser.Xml;

public class XmlDecoder : ITextBasedFormatDetector
{
    private static readonly Regex Pattern = new (@$"^<\?xml([^>]+)encoding=""(?<encoding>[^""]+)""",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
    
    public bool TryMatchFormat(string header, [NotNullWhen(true)] out string? clarifiedEncoding)
    {
        var match = Pattern.Match(header);

        if (match.Success)
        {
            clarifiedEncoding = match.Groups["encoding"].Value;
            return true;
        }
        
        clarifiedEncoding = null;
        return false;
    }

    public string MimeType => "text/xml";
}