using System.Text.RegularExpressions;
using FormatParser.Domain;
using FormatParser.Text;

namespace FormatParser.Xml;

public class XmlDecoder : ITextBasedFormatDetector
{
    private static readonly Regex XmlHeaderPattern = new(@"^<\?xml([^>]+)/?>",RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
    private static readonly Regex EncodingPattern = new(@"encoding=""(?<encoding>[^""]+)""",RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
    
    public IFileFormatInfo? TryMatchFormat(string header, EncodingInfo encodingInfo)
    {
        var match = XmlHeaderPattern.Match(header);

        if (!match.Success)
            return null;

        var encodingAttributeMatch = EncodingPattern.Match(header[..match.Length]);
        
        return encodingAttributeMatch.Success
            ? new TextFileFormatInfo(MimeType, encodingInfo with { Name = encodingAttributeMatch.Groups["encoding"].Value }) // todo: remove bom if unsopported
            : new TextFileFormatInfo(MimeType, encodingInfo );
    }

    public static string MimeType => "text/xml";
}