using FluentAssertions;
using FormatParser.Tests.TestData;
using FormatParser.Text;
using FormatParser.Text.Encoding;
using FormatParser.Text.UtfDecoders;
using FormatParser.Windows1251;
using NUnit.Framework;

namespace FormatParser.Tests;

public class TextFileProcessor_Tests : TestBase
{
    private TextFileProcessor textFileProcessor = null!;

    [SetUp]
    public void SetUp()
    {
        var textParserSettings = new TextFileParsingSettings();
        
        var nonUnicodeDecoders = new ITextDecoder[]
        {
            new Windows1251Decoder(textParserSettings)
        };
        var languageAnalyzers = new ITextAnalyzer[] {new RuFrequencyTextAnalyzer()};

        var utfDecoders = new IUtfDecoder[]
        {
            new Utf8Decoder(textParserSettings),
            new Utf16LeDecoder(textParserSettings),
            new Utf16BeDecoder(textParserSettings),
            new Utf32LeDecoder(textParserSettings),
            new Utf32BeDecoder(textParserSettings),
        };

        var decoder = new CompositeTextFormatDecoder(
            utfDecoders, 
            nonUnicodeDecoders,
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
        result!.Encoding.Should().Be(EncodingData.Utf8NoBOM);
        result.MimeType.Should().Be("text/xml");
    }
}