using System.Text.RegularExpressions;
using FormatParser.Domain;
using FormatParser.Text;

namespace FormatParser.Xml;

public class XmlDecoder : ITextBasedFormatDetector
{
    private static readonly Regex Pattern = new (@$"^<\?xml([^>]+)encoding=""(?<encoding>[^""]+)""",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
    
    public IFileFormatInfo? TryMatchFormat(string header, EncodingInfo encodingInfo)
    {
        var match = Pattern.Match(header);

        if (match.Success)
        {
            return new TextFileFormatInfo(MimeType, encodingInfo with {Name = match.Groups["encoding"].Value});
        }
        
        return null;
    }

    public static string MimeType => "text/xml";
}