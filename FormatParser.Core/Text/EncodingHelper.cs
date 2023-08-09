using FormatParser.Helpers;
using FormatParser.Text.Decoders;

namespace FormatParser.Text;

public static class EncodingHelper
{
    private static readonly IReadOnlySet<string> EncodingsWithBom;
    public static readonly IReadOnlyDictionary<string, string> CanonicalEncodingNames;
    
    static EncodingHelper()
    {
        var textDecoders = ClassDiscoveryHelper.GetAllInstancesOf<ITextDecoder, TextFileParsingSettings>(new TextFileParsingSettings()).ToArray();

        var encodingsWithBom = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        var knownEncodings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        foreach (var textDecoder in textDecoders)
        {
            if (textDecoder.SupportBom)
                encodingsWithBom.Add(textDecoder.EncodingName);
            
            knownEncodings.Add(textDecoder.EncodingName, textDecoder.EncodingName);
        }

        EncodingsWithBom = encodingsWithBom;
        CanonicalEncodingNames = knownEncodings;
    }
    
    public static bool SupportBom(string name) => EncodingsWithBom.Contains(name);
}