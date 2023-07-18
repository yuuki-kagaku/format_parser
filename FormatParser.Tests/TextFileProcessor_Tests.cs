using FluentAssertions;
using FormatParser.Text;
using FormatParser.Text.Decoders;
using FormatParser.Text.EncodingAnalyzers;
using FormatParser.TextBasedFormats;
using FormatParser.Windows1251;
using FormatParser.Xml;
using NUnit.Framework;

namespace FormatParser.Tests;

public class TextFileProcessor_Tests : TestBase
{
    private TextFileProcessor textFileProcessor = null!;

    [SetUp]
    public void SetUp()
    {
        var textParserSettings = new TextFileParsingSettings();
        
  
        var languageAnalyzers = new ITextAnalyzer[]
        {
            new AsciiCharactersTextAnalyzer(),
            new UTF16Heuristics(),
            new RuFrequencyTextAnalyzer()
        };

        var textDecoders = new ITextDecoder[]
        {
            new Utf8Decoder(textParserSettings),
            new Utf16LeDecoder(textParserSettings),
            new Utf16BeDecoder(textParserSettings),
            new Utf32LeDecoder(textParserSettings),
            new Utf32BeDecoder(textParserSettings),
            new Windows1251Decoder(textParserSettings)
        };

        var decoder = new CompositeTextFormatDecoder(
            textDecoders, 
            languageAnalyzers,
            textParserSettings);

        var textBasedFormatDetectors = new ITextBasedFormatDetector[]
        {
            new XmlDecoder()
        };
        
        textFileProcessor = new TextFileProcessor(textBasedFormatDetectors, decoder);
    }
    
    [Test]
    public void Should_read_xml()
    {
        var file = GetFile(TestFileCategory.Xml, "file.xml");

        var bytes = File.ReadAllBytes(file);
        var result = textFileProcessor.TryProcess(bytes) as TextFileFormatInfo;

        result.Should().NotBeNull();
        result!.Encoding.Should().Be(EncodingInfo.Utf8NoBOM);
        result.MimeType.Should().Be("text/xml");
    }
}