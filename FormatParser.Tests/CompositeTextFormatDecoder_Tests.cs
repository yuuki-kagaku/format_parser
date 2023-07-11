using FluentAssertions;
using FormatParser.Text;
using NUnit.Framework;

namespace FormatParser.Tests;

public class CompositeTextFormatDecoder_Tests
{
    private CompositeTextFormatDecoder decoder = null!;

    [SetUp]
    public void SetUp()
    {
        var textChecker = new CodepointChecker();
        var codepointConverter = new CodepointConverter();
        var textParserSettings = TextParserSettings.Default;
        
        var nonUnicodeDecoders = new ITextDecoder[] {new Windows1251Decoder(new CodepointChecker(), textParserSettings)};
        var languageAnalyzers = new ILanguageAnalyzer[] {new RussianLanguageAnalyzer()};

        var utfDecoders = new IUtfDecoder[]
        {
            new Utf8Decoder(textChecker, codepointConverter, textParserSettings),
            new Utf16LeDecoder(textChecker, codepointConverter, textParserSettings),
            new Utf16BeDecoder(textChecker, codepointConverter, textParserSettings),
        };

        decoder = new CompositeTextFormatDecoder(
            utfDecoders, 
            nonUnicodeDecoders,
            languageAnalyzers,
            textParserSettings);
    }
    
    [Test]
    public async Task Should_read_ascii()
    {
        var binaryReader = await CreateBinaryReaderAsync(@"./TestData/text/loren_utf8_no_bom");

        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        text.Should().Be(TextSamples.TextWithOnlyAsciiChars);
        encoding.Should().BeEquivalentTo(WellKnownEncodings.ASCII);
    }
    
    [Test]
    public async Task Should_read_utf16_be_no_bom()
    {
        var binaryReader = await CreateBinaryReaderAsync(@"./TestData/text/loren_utf16_be_nobom");

        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        text.Should().Be(TextSamples.TextWithOnlyAsciiChars);
        encoding.Should().BeEquivalentTo(WellKnownEncodings.UTF16BeNoBom);
    }
    
    [Test]
    public async Task Should_read_utf16_le_no_bom()
    {
        var binaryReader = await CreateBinaryReaderAsync(@"./TestData/text/loren_utf16_le_nobom");

        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(WellKnownEncodings.UTF16LeNoBom);
        text.Should().Be(TextSamples.TextWithOnlyAsciiChars);
    }
    
    [Test]
    public async Task Should_read_windows1251()
    {
        var binaryReader = await CreateBinaryReaderAsync(@"./TestData/text/bsd_windows1251");
    
        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(Windows1251Decoder.Encoding);
        text.Should().Be(TextSamples.RussianLanguageSample);
    }

    private static async Task<InMemoryBinaryReader> CreateBinaryReaderAsync(string filename)
    {
        var content = await File.ReadAllBytesAsync(filename);
        return new InMemoryBinaryReader(content);
    }
}