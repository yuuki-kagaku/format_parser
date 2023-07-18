using System.Diagnostics.CodeAnalysis;
using System.Text;
using FormatParser.Text.Decoders;
using FormatParser.Text.EncodingAnalyzers;
using EncodingInfo = FormatParser.Domain.EncodingInfo;

namespace FormatParser.Text;

public class CompositeTextFormatDecoder
{
    private readonly TextFileParsingSettings settings;
    private readonly ITextDecoder[] decoders;
    private readonly Dictionary<ITextDecoder, IEnumerable<ITextAnalyzer>> encodingAnalyzersDictionary;
    private readonly Dictionary<ITextDecoder, CodepointValidator> invalidCharactersCheckers;

    public CompositeTextFormatDecoder(
        ITextDecoder[] decoders, 
        ITextAnalyzer[] encodingAnalyzers,
        TextFileParsingSettings settings)
    {
        this.decoders = decoders;
        var encodingAnalyzersByLanguage = encodingAnalyzers
            .SelectMany(analyzer => analyzer.SupportedLanguages.Select(lang => (Language: lang, Analyzer: analyzer)))
            .ToDictionary(x => x.Language, x => x.Analyzer);

        encodingAnalyzersDictionary = decoders.ToDictionary(x => x, x => GetEncodingAnalyzers(x, encodingAnalyzersByLanguage));
        invalidCharactersCheckers = decoders.ToDictionary(x => x, x => new CodepointValidator(x.GetInvalidCharacters));
        this.settings = settings;
    }

    public bool TryDecode(ArraySegment<byte> bytes, [NotNullWhen(true)] out EncodingInfo? resultEncoding, [NotNullWhen(true)] out string? resultTextSample)
    {
        var bestMatchProbability = DetectionProbability.No;
        resultEncoding = null;
        resultTextSample = null;
        
        foreach (var decoder in decoders)
        {
            try
            {
                var decodeResult = decoder.TryDecodeText(bytes);
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
            catch (DecoderFallbackException)
            {
            }
        }
        
        return bestMatchProbability > DetectionProbability.No;
    }
    
    
    private IEnumerable<ITextAnalyzer> GetEncodingAnalyzers(ITextDecoder decoder, Dictionary<string, ITextAnalyzer> encodingAnalyzersByLanguage)
    {
        if (decoder.RequiredEncodingAnalyzers == null)
            yield break;

        foreach (var encodingAnalyzerId in decoder.RequiredEncodingAnalyzers)
        {
            if (!encodingAnalyzersByLanguage.TryGetValue(encodingAnalyzerId, out var analyzer))
                throw new Exception($"Could not find analyzer for {decoder.RequiredEncodingAnalyzers}");

            yield return analyzer;
        }
    }
}