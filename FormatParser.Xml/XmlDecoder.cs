using System.Text.RegularExpressions;
using FormatParser.Domain;
using FormatParser.Text;

namespace FormatParser.Xml;

public class XmlDecoder : ITextBasedFormatDetector
{
    private static readonly Regex XmlHeaderPattern = new(@"^<\?xml([^>]+)\?>",RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
    private static readonly Regex EncodingPattern = new(@"encoding=""(?<encoding>[^""]+)""",RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
    
    private static IReadOnlySet<string> EncodingsWithAllowedAutodetection = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
        { WellKnownEncodings.Utf8, WellKnownEncodings.Utf16, WellKnownEncodings.Utf32 }; 

    private static string MimeType => "text/xml";
    
    public TextFileFormatInfo? TryMatchFormat(string header, EncodingInfo
        encodingInfo)
    {
        var match = XmlHeaderPattern.Match(header);

        if (!match.Success)
            return null;

        var encodingAttributeMatch = EncodingPattern.Match(header[..match.Length]);

        return encodingAttributeMatch.Success
            ? new XmlFileFormatInfo(MimeType, encodingInfo with { Name = encodingAttributeMatch.Groups["encoding"].Value })
            : new XmlFileFormatInfo(MimeType, encodingInfo with { Name = GetName(encodingInfo.Name) });
    }

    private static string GetName(string autoDetectedName) =>
        EncodingsWithAllowedAutodetection.Contains(autoDetectedName) ? autoDetectedName : WellKnownEncodings.Utf8;
}