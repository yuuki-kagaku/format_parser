using System.Text.RegularExpressions;
using FormatParser.TextBasedFormats;

namespace FormatParser.Xml;

public class XmlDecoder : ITextBasedFormatDetector
{
    private static readonly Regex pattern = new (@$"^<\?xml([^>]+)encoding=""(?<encoding>[^""]+)""",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
    
    public bool TryMatchFormat(string header, out string? encoding)
    {
        var match = pattern.Match(header);

        if (match.Success)
        {
            encoding = match.Groups["encoding"].Value;
            return true;
        }
        
        encoding = null;
        return false;
    }

    public string MimeType => "text/xml";
}