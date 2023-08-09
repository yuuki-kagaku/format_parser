using System.Diagnostics.CodeAnalysis;
using System.Text;
using FormatParser.Domain;
using FormatParser.Helpers;
using FormatParser.Text.Decoders;
using EncodingInfo = FormatParser.Domain.EncodingInfo;

namespace FormatParser.Text;

public class CompositeTextFormatDecoder
{
    private readonly ITextDecoder[] decoders;
    private readonly Dictionary<ITextDecoder, IEnumerable<ITextAnalyzer>> encodingAnalyzersDictionary;
    private readonly Dictionary<ITextDecoder, Func<char, bool>> validCharactersCheckers;

    public CompositeTextFormatDecoder(ITextDecoder[] decoders, ITextAnalyzer[] textAnalyzers)
    {
        this.decoders = decoders;
        var encodingAnalyzersById = textAnalyzers
            .SelectMany(analyzer => analyzer.AnalyzerIds.Select(id => (Id: id, Analyzer: analyzer)))
            .GroupBy(x => x.Id)
            .ToDictionary(x => x.Key, x => x.Select(y => y.Analyzer).ToArray());

        encodingAnalyzersDictionary = decoders.ToDictionary(x => x, x => GetEncodingAnalyzers(x, encodingAnalyzersById));
        validCharactersCheckers = decoders.ToDictionary(x => x, x => new Func<char, bool>(x.IsCharacterValid));
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
                
                if (!decodeResult.Chars.All(validCharactersCheckers[decoder]))
                    continue;

                var text = decodeResult.Chars.AsString();

                var defaultDetectionProbability = decoder.DefaultDetectionProbability;
                if (defaultDetectionProbability > bestMatchProbability)
                    (bestMatchProbability, resultEncoding, resultTextSample) = (defaultDetectionProbability, decodeResult.Encoding, text);

                foreach (var textAnalyzer in encodingAnalyzersDictionary[decoder])
                {
                    var detectionProbability = textAnalyzer.AnalyzeProbability(text, decodeResult.Encoding, out var clarifiedEncoding);

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
    
    
    private static IEnumerable<ITextAnalyzer> GetEncodingAnalyzers(ITextDecoder decoder, Dictionary<string, ITextAnalyzer[]> encodingAnalyzersByLanguage)
    {
        if (decoder.RequiredEncodingAnalyzers == null)
            yield break;

        foreach (var encodingAnalyzerId in decoder.RequiredEncodingAnalyzers)
        {
            if (!encodingAnalyzersByLanguage.TryGetValue(encodingAnalyzerId, out var analyzers))
                throw new Exception($"Could not find analyzer for id: {encodingAnalyzerId}.");
            
            foreach (var analyzer in analyzers)
                yield return analyzer;
        }
    }
}