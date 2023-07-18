using System.IO.Compression;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using FormatParser.Archives;
using FormatParser.CLI;
using FormatParser.ELF;
using FormatParser.MachO;
using FormatParser.PE;
using FormatParser.Text;
using FormatParser.Text.Decoders;
using FormatParser.Text.TextAnalyzers;
using FormatParser.Windows1251;
using FormatParser.Xml;

namespace FormatParser.PerformanceTest;

[SimpleJob(RuntimeMoniker.Net70)]
public class InMemoryBenchmark
{
    private List<Stream> streams;
    private FormatDecoder decoder;

    [GlobalSetup]
    public void Setup()
    {
        var zip = ZipFile.Open("data_set.zip", ZipArchiveMode.Read);
        streams = new List<Stream>();

        foreach (var entry in zip.Entries)
        {
            var memoryStream = new MemoryStream();
            using var stream = entry.Open();
            stream.CopyTo(memoryStream);
            streams.Add(memoryStream);
        }
        
        var textParserSettings = new TextFileParsingSettings();
        
        var languageAnalyzers = new ITextAnalyzer[]
        {
            new AsciiCharactersTextAnalyzer(),
            new UTF16Heuristics(),
            new RuDictionaryTextAnalyzer()
        };

        var binaryFormatDetectors = new IBinaryFormatDetector[]
        {
            new ElfDetector(),
            new PeDetector(),
            new MachODetector(),
            new ArchDetector()
        };

        var textDecoders = new ITextDecoder[]
        {
            new Utf8Decoder(textParserSettings),
            new Utf16LeDecoder(textParserSettings),
            new Utf16BeDecoder(textParserSettings),
            new Utf32LeDecoder(textParserSettings),
            new Utf32BeDecoder(textParserSettings),
            new Windows1251Decoder(textParserSettings),
        };

        var compositeTextFormatDecoder = new CompositeTextFormatDecoder(textDecoders, languageAnalyzers);
        
        var textBasedFormatDetectors = new ITextBasedFormatDetector[]
        {
            new XmlDecoder()
        };

         decoder = new FormatDecoder(binaryFormatDetectors,
             new TextFileProcessor(textBasedFormatDetectors, compositeTextFormatDecoder),
             new TextFileParsingSettings()
         );
    }

    [IterationSetup]
    public void IterationSetup()
    {
    }

    [Benchmark]
    public async Task Run()
    {
        for (int i = 0; i < 1000; i++)
        {
            foreach (var stream in streams)
            {
                await decoder.Decode(stream);
            }
        }
    }

}