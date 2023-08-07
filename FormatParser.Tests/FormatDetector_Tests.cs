using FluentAssertions;
using FormatParser.Domain;
using FormatParser.Text;
using FormatParser.Text.Decoders;
using FormatParser.Text.TextAnalyzers;
using FormatParser.Windows1251;
using FormatParser.Xml;
using NUnit.Framework;

namespace FormatParser.Tests;

public class FormatDetector_Tests : TestBase
{
    private FormatDetector formatDetector = null!;

    [SetUp]
    public void SetUp()
    {
        var textParserSettings = new TextFileParsingSettings();
        
        var textAnalyzers = new ITextAnalyzer[]
        {
            new AsciiCharactersTextAnalyzer(),
            new UTF16Heuristics(),
            new HeaderTextAnalyzer(),
            new RuDictionaryTextAnalyzer()
        };

        var textDecoders = new ITextDecoder[]
        {
            new Utf8Decoder(textParserSettings),
            new Utf16LeDecoder(textParserSettings),
            new Utf16BeDecoder(textParserSettings),
            new Utf32LeDecoder(textParserSettings),
            new Utf32BeDecoder(textParserSettings),
            new Windows1251Decoder(textParserSettings),
            new EBCDICDecoder(textParserSettings)
        };

        var decoder = new CompositeTextFormatDecoder(textDecoders, textAnalyzers);

        var textBasedFormatDetectors = new ITextBasedFormatDetector[]
        {
            new XmlDecoder()
        };
        
        formatDetector = new FormatDetector(Array.Empty<IBinaryFormatDetector>(), textBasedFormatDetectors, decoder, textParserSettings);
    }
    
    [Test]
    public async Task Should_read_xml()
    {
        var file = GetFile(TestFileCategory.Xml, "file.xml");

        await using var stream =  new FileStream(file, FileMode.Open, FileAccess.ReadWrite);
        var result = (await formatDetector.DetectAsync(stream)) as TextFileFormatInfo;

        result.Should().NotBeNull();
        result!.Encoding.Should().Be(WellKnownEncodingInfos.Utf8NoBom);
        result.MimeType.Should().Be("text/xml");
    }
    
    [Test]
    public async Task Should_read_ebcdic_xml()
    {
        var file = GetFile(TestFileCategory.Ebcdic, "ebcdic.xml");

        await using var stream =  new FileStream(file, FileMode.Open, FileAccess.ReadWrite);
        var result = (await formatDetector.DetectAsync(stream)) as TextFileFormatInfo;

        result.Should().NotBeNull();
        result!.Encoding.Should().Be(new EncodingInfo("IBM037", Endianness.NotAllowed, false));
        result.MimeType.Should().Be("text/xml");
    }
}