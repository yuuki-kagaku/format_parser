using FluentAssertions;
using FormatParser.Domain;
using FormatParser.Ebcdic;
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
            new EBCDICDecoder(textParserSettings),
            new AsciiBasedFallbackDecoder(textParserSettings),
        };
        
        var textBasedFormatDetectors = new ITextBasedFormatDetector[]
        {
            new XmlDecoder()
        };

        var decoder = new CompositeTextFormatDecoder(textDecoders, textAnalyzers, textBasedFormatDetectors);
        
        formatDetector = new FormatDetector(Array.Empty<IBinaryFormatDetector>(), decoder, textParserSettings);
    }
    
    [Test]
    public async Task Should_read_utf8_xml()
    {
        var result = await DetectFileAsync(TestFileCategory.Xml, "file.xml");
     
        result.Should().NotBeNull();
        result!.Encoding.Should().Be(WellKnownEncodingInfos.Utf8NoBom);
        result.MimeType.Should().Be("text/xml");
    }

    [Test]
    public async Task Should_read_utf16be_xml()
    {
        var result = await DetectFileAsync(TestFileCategory.Xml, "file_utf16be.xml");
     
        result.Should().NotBeNull();
        result!.Encoding.Should().Be(WellKnownEncodingInfos.Utf16BeNoBom);
        result.MimeType.Should().Be("text/xml");
    }
    
    [Test]
    public async Task Should_read_utf16le_xml()
    {
        var result = await DetectFileAsync(TestFileCategory.Xml, "file_utf16le.xml");
     
        result.Should().NotBeNull();
        result!.Encoding.Should().Be(WellKnownEncodingInfos.Utf16LeNoBom);
        result.MimeType.Should().Be("text/xml");
    }
    
    [Test]
    public async Task Should_read_utf16be_xml_without_encoding_attribute_with_bom()
    {
        var result = await DetectFileAsync(TestFileCategory.Xml, "file_utf16be_without_encoding_attribute_bom.xml");
     
        result.Should().NotBeNull();
        result!.Encoding.Should().Be(WellKnownEncodingInfos.Utf16BeBom);
        result.MimeType.Should().Be("text/xml");
    }
    
    [Test]
    public async Task Should_read_utf16be_xml_without_encoding_attribute_without_bom()
    {
        var result = await DetectFileAsync(TestFileCategory.Xml, "file_utf16be_without_encoding_attribute_nobom.xml");
     
        result.Should().NotBeNull();
        result!.Encoding.Should().Be(WellKnownEncodingInfos.Utf16BeNoBom);
        result.MimeType.Should().Be("text/xml");
    }
    
    [Test]
    public async Task Should_read_utf16le_xml_without_encoding_attribute_with_bom()
    {
        var result = await DetectFileAsync(TestFileCategory.Xml, "file_utf16le_without_encoding_attribute_bom.xml");
     
        result.Should().NotBeNull();
        result!.Encoding.Should().Be(WellKnownEncodingInfos.Utf16LeBom);
        result.MimeType.Should().Be("text/xml");
    }
    
    [Test]
    public async Task Should_read_utf16le_xml_without_encoding_attribute_without_bom()
    {
        var result = await DetectFileAsync(TestFileCategory.Xml, "file_utf16le_without_encoding_attribute_nobom.xml");
     
        result.Should().NotBeNull();
        result!.Encoding.Should().Be(WellKnownEncodingInfos.Utf16LeNoBom);
        result.MimeType.Should().Be("text/xml");
    }
    
    [Test]
    public async Task Should_read_windows_1251_without_bom()
    {
        var result = await DetectFileAsync(TestFileCategory.Xml, "file_win1251.xml");
     
        result.Should().NotBeNull();
        result!.Encoding.Should().Be(new EncodingInfo("Windows-1251", Endianness.NotAllowed, false));
        result.MimeType.Should().Be("text/xml");
    }
    
    [Test]
    public async Task Should_read_windows_1251_with_bom()
    {
        var result = await DetectFileAsync(TestFileCategory.Xml, "file_win1251_with_bom.xml");
     
        result.Should().NotBeNull();
        result!.Encoding.Should().Be(new EncodingInfo("Windows-1251", Endianness.NotAllowed, true));
        result.MimeType.Should().Be("text/xml");
    }
    
    [Test]
    public async Task Should_read_ebcdic_xml()
    {
        var result = await DetectFileAsync(TestFileCategory.Xml, "ebcdic.xml");

        result.Should().NotBeNull();
        result!.Encoding.Should().Be(new EncodingInfo("IBM037", Endianness.NotAllowed, false));
        result.MimeType.Should().Be("text/xml");
    }
    
    [Test]
    public async Task Should_read_ebcdic_xml_with_german_encoding()
    {
        var result = await DetectFileAsync(TestFileCategory.Xml, "ebcidic-de.xml");

        result.Should().NotBeNull();
        result!.Encoding.Should().Be(new EncodingInfo("IBM273", Endianness.NotAllowed, false));
        result.MimeType.Should().Be("text/xml");
    }

    private async Task<TextFileFormatInfo?> DetectFileAsync(TestFileCategory testFileCategory, string filename)
    {
        var file = GetFile(testFileCategory, filename);

        await using var stream =  new FileStream(file, FileMode.Open, FileAccess.ReadWrite);
        return (await formatDetector.DetectAsync(stream)) as TextFileFormatInfo;
    }
}