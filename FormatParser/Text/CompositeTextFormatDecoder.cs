using System.Diagnostics.CodeAnalysis;
using FormatParser.Text.Encoding;

namespace FormatParser.Text;

public class CompositeTextFormatDecoder
{
    private readonly TextFileParsingSettings settings;
    private readonly Encoding.ITextDecoder[] decoders;
    private readonly Dictionary<Encoding.ITextDecoder, IEnumerable<ITextAnalyzer>> encodingAnalyzersDictionary;
    private readonly Dictionary<Encoding.ITextDecoder, CodepointValidator> invalidCharactersCheckers;

    public CompositeTextFormatDecoder(
        IUtfDecoder[] utfDecoders,
        Encoding.ITextDecoder[] nonUtfDecoders, 
        ITextAnalyzer[] encodingAnalyzers,
        TextFileParsingSettings settings)
    {
        decoders = utfDecoders.Concat(nonUtfDecoders).ToArray();
        
        var encodingAnalyzersByLanguage = encodingAnalyzers
            .SelectMany(analyzer => analyzer.SupportedLanguages.Select(lang => (Language: lang, Analyzer: analyzer)))
            .ToDictionary(x => x.Language, x => x.Analyzer);

        encodingAnalyzersDictionary = decoders.ToDictionary(x => x, x => GetEncodingAnalyzers(x, encodingAnalyzersByLanguage));
        invalidCharactersCheckers = decoders.ToDictionary(x => x, x => new CodepointValidator(x.GetInvalidCharacters));
        this.settings = settings;
    }

    public bool TryDecode(ArraySegment<byte> bytes, [NotNullWhen(true)] out string? resultEncoding, [NotNullWhen(true)] out string? resultTextSample)
    {
        var bestMatchProbability = DetectionProbability.No;
        resultEncoding = null;
        resultTextSample = null;
        var charsBuffer = new char[settings.SampleSize];
        
        foreach (var decoder in decoders)
        {
            try
            {
                var decodeResult = decoder.TryDecodeText(bytes, charsBuffer);
                if (decodeResult != null)
                {
                    var invalidCharactersChecker = invalidCharactersCheckers[decoder];
                    if (!invalidCharactersChecker.AllCharactersIsValid(decodeResult.Chars))
                        continue;
                    
                    var textSample = new TextSample(decodeResult.Chars);

                    var defaultDetectionProbability = decoder.DefaultDetectionProbability;
                    if (defaultDetectionProbability > bestMatchProbability)
                        (bestMatchProbability, resultEncoding, resultTextSample) = (defaultDetectionProbability, decodeResult.Encoding, textSample.Text);
                    
                    foreach (var encodingAnalyzer in encodingAnalyzersDictionary[decoder])
                    {
                        var detectionProbability = encodingAnalyzer.AnalyzeProbability(textSample, decodeResult.Encoding, out var clarifiedEncoding);

                        if (detectionProbability > bestMatchProbability)
                        {
                            (bestMatchProbability, resultEncoding, resultTextSample) = (detectionProbability, clarifiedEncoding ?? decodeResult.Encoding, textSample.Text);
                            
                            if (bestMatchProbability >= DetectionProbability.High)
                                return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }
        }
        
        return bestMatchProbability > DetectionProbability.No;
    }
    
    
    private static IEnumerable<ITextAnalyzer> GetEncodingAnalyzers(Encoding.ITextDecoder decoder, Dictionary<string, ITextAnalyzer> encodingAnalyzersByLanguage)
    {
        yield return new AsciiCharactersTextAnalyzer();
        yield return new UTF16Heuristics();

        if (decoder.RequiredEncodingAnalyzer == null)
            yield break;
        
        if (!encodingAnalyzersByLanguage.TryGetValue(decoder.RequiredEncodingAnalyzer, out var analyzer))
            throw new Exception($"Could not find analyzer for {decoder.RequiredEncodingAnalyzer}");

        yield return analyzer;
    }
}