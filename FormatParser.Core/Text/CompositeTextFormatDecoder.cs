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
    private readonly ITextBasedFormatDetector[] textBasedFormatDetectors;
    private readonly Dictionary<ITextDecoder, IEnumerable<ITextAnalyzer>> encodingAnalyzersDictionary;
    private readonly Dictionary<ITextDecoder, Func<char, bool>> validCharactersCheckers;

    public CompositeTextFormatDecoder(ITextDecoder[] decoders, ITextAnalyzer[] textAnalyzers, ITextBasedFormatDetector[] textBasedFormatDetectors)
    {
        this.decoders = decoders;
        this.textBasedFormatDetectors = textBasedFormatDetectors;
        
        var encodingAnalyzersById = textAnalyzers
            .SelectMany(analyzer => analyzer.AnalyzerIds.Select(id => (Id: id, Analyzer: analyzer)))
            .GroupBy(x => x.Id)
            .ToDictionary(x => x.Key, x => x.Select(y => y.Analyzer).ToArray());

        encodingAnalyzersDictionary = decoders.ToDictionary(x => x, x => GetEncodingAnalyzers(x, encodingAnalyzersById));
        validCharactersCheckers = decoders.ToDictionary(x => x, x => new Func<char, bool>(x.IsCharacterValid));
    }

    public bool TryDecode(ArraySegment<byte> bytes, [NotNullWhen(true)] out TextFileFormatInfo? resultFormatInfo)
    {
        var bestMatchProbability = new MatchQuality(DetectionProbability.No, false);
        resultFormatInfo = null;
        
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
                var matchedTextBasedFormat = false;

                var defaultDetectionProbability = new MatchQuality(decoder.DefaultDetectionProbability, matchedTextBasedFormat);
                if (defaultDetectionProbability > bestMatchProbability && !decoder.RequireTextBasedFormatMatch)
                    (bestMatchProbability, resultFormatInfo) = (defaultDetectionProbability, CreatePlainTextFileFormat(decodeResult.Encoding));

                var textFileFormatInfo = null as TextFileFormatInfo;
                
                foreach (var textBasedFormatDetector in textBasedFormatDetectors)
                {
                    textFileFormatInfo = MatchTextBasedFormat(textBasedFormatDetector, text, decodeResult.Encoding);

                    if (textFileFormatInfo == null)
                        continue;

                    if (textFileFormatInfo.Encoding != decodeResult.Encoding)
                    {
                        resultFormatInfo = textFileFormatInfo;
                        return true;
                    }

                    matchedTextBasedFormat = true;
                    defaultDetectionProbability = defaultDetectionProbability with { MatchedTextBasedFormat = true };
                        
                    if (defaultDetectionProbability > bestMatchProbability)
                        (bestMatchProbability, resultFormatInfo) = (defaultDetectionProbability, textFileFormatInfo);
                }
                
                if (decoder.RequireTextBasedFormatMatch && !matchedTextBasedFormat)
                    continue;
                
                foreach (var textAnalyzer in encodingAnalyzersDictionary[decoder])
                {
                    var detectionProbability = new MatchQuality(textAnalyzer.AnalyzeProbability(text, decodeResult.Encoding, out var clarifiedEncoding), matchedTextBasedFormat);

                    if (detectionProbability < bestMatchProbability)
                        continue;

                    bestMatchProbability = detectionProbability;
                    var encoding = clarifiedEncoding ?? decodeResult.Encoding;
                    resultFormatInfo = textFileFormatInfo == null ? CreatePlainTextFileFormat(encoding) : textFileFormatInfo with {Encoding = encoding};
                    
                    if (bestMatchProbability is { MatchedTextBasedFormat: true, DetectionProbability: >= DetectionProbability.High })
                        return true;
                }
            }
            catch (DecoderFallbackException)
            {
            }
        }

        return bestMatchProbability > new MatchQuality(DetectionProbability.No, false);
    }

    private static TextFileFormatInfo? MatchTextBasedFormat(ITextBasedFormatDetector textBasedFormatDetector, string text, EncodingInfo encoding)
    {
        try
        {
            return textBasedFormatDetector.TryMatchFormat(text, encoding);
        }
        catch
        {
            return null;
        }
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

    private static TextFileFormatInfo CreatePlainTextFileFormat(EncodingInfo encoding) => new(TextFileFormatInfo.DefaultTextType, encoding);

    private readonly record struct MatchQuality(DetectionProbability DetectionProbability, bool MatchedTextBasedFormat) : IComparable<MatchQuality>
    {
        public int CompareTo(MatchQuality other)
        {
            var compareMatchedTextBasedFormat = MatchedTextBasedFormat.CompareTo(other.MatchedTextBasedFormat);
            
            if (compareMatchedTextBasedFormat != 0)
                return compareMatchedTextBasedFormat;

            return DetectionProbability.CompareTo(other.DetectionProbability);
        }
        
        public static bool operator > (MatchQuality item, MatchQuality other) => item.CompareTo(other) > 0;

        public static bool operator < (MatchQuality item, MatchQuality other) => item.CompareTo(other) < 0;
    }
}