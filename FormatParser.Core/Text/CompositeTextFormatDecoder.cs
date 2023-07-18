using System.Diagnostics.CodeAnalysis;
using System.Text;
using FormatParser.Helpers;
using FormatParser.Text.Decoders;
using EncodingInfo = FormatParser.Domain.EncodingInfo;

namespace FormatParser.Text;

public class CompositeTextFormatDecoder
{
    private readonly ITextDecoder[] decoders;
    private readonly Dictionary<ITextDecoder, IEnumerable<ITextAnalyzer>> encodingAnalyzersDictionary;
    private readonly Dictionary<ITextDecoder, CharacterValidator> invalidCharactersCheckers;

    public CompositeTextFormatDecoder(ITextDecoder[] decoders, ITextAnalyzer[] textAnalyzers)
    {
        this.decoders = decoders;
        var encodingAnalyzersByLanguage = textAnalyzers
            .SelectMany(analyzer => analyzer.RequiredAnalyzers.Select(lang => (Language: lang, Analyzer: analyzer)))
            .ToDictionary(x => x.Language, x => x.Analyzer);

        encodingAnalyzersDictionary = decoders.ToDictionary(x => x, x => GetEncodingAnalyzers(x, encodingAnalyzersByLanguage));
        invalidCharactersCheckers = decoders.ToDictionary(x => x, x => new CharacterValidator(x.GetInvalidCharacters));
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
                if (decodeResult == null) 
                    continue;
                
                // Console.WriteLine($"text : {decodeResult.Chars.AsString()}");
                Console.WriteLine($"tex2t : {invalidCharactersCheckers[decoder].AllCharactersIsValid(decodeResult.Chars)}");
                
                if (!invalidCharactersCheckers[decoder].AllCharactersIsValid(decodeResult.Chars))
                    continue;

                Console.WriteLine($"encodingAnalyzersDictionary[decoder] : {encodingAnalyzersDictionary[decoder].Count()}");

                var text = decodeResult.Chars.AsString();

                var defaultDetectionProbability = decoder.DefaultDetectionProbability;
                if (defaultDetectionProbability > bestMatchProbability)
                    (bestMatchProbability, resultEncoding, resultTextSample) = (defaultDetectionProbability, decodeResult.Encoding, text);

                
                foreach (var textAnalyzer in encodingAnalyzersDictionary[decoder])
                {
                    var detectionProbability = textAnalyzer.AnalyzeProbability(text, decodeResult.Encoding, out var clarifiedEncoding);

                    Console.WriteLine($"detectionProbability : {detectionProbability} | {textAnalyzer.GetType().Name}");
                    if (detectionProbability <= bestMatchProbability)
                        continue;
                    
                    (bestMatchProbability, resultEncoding, resultTextSample) = (detectionProbability, clarifiedEncoding ?? decodeResult.Encoding, text);
                            
                    if (bestMatchProbability >= DetectionProbability.High)
                        return true;
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