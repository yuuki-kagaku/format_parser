using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FormatParser.Text;

public class CompositeTextFormatDecoder
{
    private readonly TextParserSettings settings;
    private readonly ITextDecoder[] decoders;
    private readonly Dictionary<ITextDecoder, IEnumerable<ITextAnalyzer>> encodingAnalyzersDictionary;

    public CompositeTextFormatDecoder(
        IUtfDecoder[] utfDecoders,
        ITextDecoder[] nonUtfDecoders, 
        ITextAnalyzer[] encodingAnalyzers,
        TextParserSettings settings)
    {
        decoders = utfDecoders.Concat(nonUtfDecoders).ToArray();
        
        var encodingAnalyzersByLanguage = encodingAnalyzers
            .SelectMany(analyzer => analyzer.SupportedLanguages.Select(lang => (Language: lang, Analyzer: analyzer)))
            .ToDictionary(x => x.Language, x => x.Analyzer);

        encodingAnalyzersDictionary = decoders.ToDictionary(x => x, x => GetEncodingAnalyzers(x, encodingAnalyzersByLanguage));
            
        this.settings = settings;
    }

    public bool TryDecode(InMemoryBinaryReader binaryReader, [NotNullWhen(true)] out string? resultEncoding, [NotNullWhen(true)] out string? resultTextSample)
    {
        var bestMatchProbability = DetectionProbability.No;
        resultEncoding = null;
        resultTextSample = null;
        var stringBuilder = new StringBuilder(settings.SampleSize);
        
        foreach (var decoder in decoders)
        {
            try
            {
                binaryReader.Offset = 0;
                stringBuilder.Clear();

                if (decoder.TryDecode(binaryReader, stringBuilder, out var encoding))
                {
                    var textSample = new TextSample(stringBuilder);

                    var defaultDetectionProbability = decoder.DefaultDetectionProbability;
                    if (defaultDetectionProbability > bestMatchProbability)
                        (bestMatchProbability, resultEncoding, resultTextSample) = (defaultDetectionProbability, encoding, textSample.Text);
                    
                    foreach (var encodingAnalyzer in encodingAnalyzersDictionary[decoder])
                    {
                        var detectionProbability = encodingAnalyzer.AnalyzeProbability(textSample, encoding, out var clarifiedEncoding);

                        if (detectionProbability > bestMatchProbability)
                        {
                            (bestMatchProbability, resultEncoding, resultTextSample) = (detectionProbability, clarifiedEncoding ?? encoding, textSample.Text);
                            
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
    
    private static IEnumerable<ITextAnalyzer> GetEncodingAnalyzers(ITextDecoder decoder, Dictionary<string, ITextAnalyzer> encodingAnalyzersByLanguage)
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