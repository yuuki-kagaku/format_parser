using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FormatParser.Text;

public class CompositeTextFormatDecoder
{
    private readonly TextParserSettings settings;
    private readonly Dictionary<string, IUtfDecoder> utfDecodersByEncoding;
    private readonly Dictionary<string, ILanguageAnalyzer> languageAnalyzers;
    private readonly ITextDecoder[] decoders;
    
    public CompositeTextFormatDecoder(
        IUtfDecoder[] utfDecoders,
        ITextDecoder[] nonUtfDecoders, 
        ILanguageAnalyzer[] languageAnalyzers,
        TextParserSettings settings)
    {
        decoders = utfDecoders.Concat(nonUtfDecoders).ToArray();
        
        this.languageAnalyzers = languageAnalyzers
            .SelectMany(analyzer => analyzer.SupportedLanguages.Select(lang => (Language: lang, Analyzer: analyzer)))
            .ToDictionary(x => x.Language, x => x.Analyzer);
        
        utfDecodersByEncoding = utfDecoders
            .SelectMany(d => d.CanReadEncodings.Select(e => (Decoder: d, Encoding: e)))
            .ToDictionary(x => x.Encoding, x => x.Decoder);
            
        this.settings = settings;
    }
    
    public bool TryDecode(InMemoryBinaryReader binaryReader, [NotNullWhen(true)] out string? encoding, [NotNullWhen(true)] out string? textSample)
    {
        if (TryFindUtfBom(binaryReader, out encoding, out var bom))
        {
            var decoder = utfDecodersByEncoding[encoding];
            binaryReader.Offset = bom.Length;

            var stringBuilder = new StringBuilder(settings.SampleSize);
            if (!decoder.TryDecode(binaryReader, stringBuilder, out _, out _) )
                throw new Exception("File is corrupt.");

            textSample = stringBuilder.ToString();
            return true;
        }
      
        return TryDecodeInternal(binaryReader, out encoding, out textSample);
    }
    
    private static bool TryFindUtfBom(InMemoryBinaryReader binaryReader, [NotNullWhen(true)] out string? encoding, [NotNullWhen(true)] out byte[]? bom)
    {
        binaryReader.Offset = 0;
        var firstBytes = binaryReader.ReadBytes(4);
        
        foreach (var (magicBytes, e) in UTFConstants.BOMs)
        {
            if (magicBytes.SequenceEqual(new ArraySegment<byte>(firstBytes, 0, magicBytes.Length)))
            {
                (encoding, bom) = (e, magicBytes);
                return true;
            }
        }

        (encoding, bom) = (default, null);
        return false;
    }

    private bool TryDecodeInternal(InMemoryBinaryReader binaryReader, [NotNullWhen(true)] out string? resultEncoding, [NotNullWhen(true)] out string? textSample)
    {
        var bestMatchProbability = DetectionProbability.No;
        resultEncoding = null;
        textSample = null;
        var stringBuilder = new StringBuilder(settings.SampleSize);
        
        foreach (var decoder in decoders)
        {
            try
            {
                binaryReader.Offset = 0;
                stringBuilder.Clear();

                if (decoder.TryDecode(binaryReader, stringBuilder, out var encoding, out var detectionProbability))
                {
                    var text = new Lazy<string>(() => stringBuilder.ToString(), LazyThreadSafetyMode.None);

                    if (detectionProbability > bestMatchProbability)
                    {
                        (bestMatchProbability, resultEncoding, textSample) = (detectionProbability, encoding, text.Value);

                        if (bestMatchProbability == DetectionProbability.High)
                            return true;
                    }

                    if (TryMatchWithLanguageDetector(decoder, text.Value, out detectionProbability) && detectionProbability > bestMatchProbability)
                    {
                        (bestMatchProbability, resultEncoding, textSample) = (detectionProbability, encoding, text.Value);
                            
                        if (bestMatchProbability == DetectionProbability.High)
                            return true;
                    }
                }
            }
            catch (Exception e)
            {
            }
        }

        return bestMatchProbability > DetectionProbability.No;
    }

    private bool TryMatchWithLanguageDetector(ITextDecoder decoder, string textSample, out DetectionProbability detectionProbability)
    {
        detectionProbability = DetectionProbability.No;

        if (decoder.RequiredLanguageAnalyzer == null)
            return false;
        
        var languageAnalyzer = FindLanguageAnalyzer(decoder);
        
        if (languageAnalyzer == null)
            return false;
        
        if (languageAnalyzer.IsCorrectText(textSample.ToLowerInvariant()))
        {
            detectionProbability = languageAnalyzer.DetectionProbabilityOnSuccessfulMatch;
            return true;
        }
        return false;
    }

    private ILanguageAnalyzer? FindLanguageAnalyzer(ITextDecoder decoder)
    {
        if (decoder.RequiredLanguageAnalyzer == null)
            return null;

        if (!languageAnalyzers.TryGetValue(decoder.RequiredLanguageAnalyzer, out var analyzer))
            return null;

        return analyzer;
    }
}