using System.Text;
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
        var codepointConverter = new CodepointConverter();
        var textParserSettings = TextParserSettings.Default;
        
        var nonUnicodeDecoders = new ITextDecoder[]
        {
            new Windows1251Decoder(textParserSettings)
        };
        var languageAnalyzers = new ITextAnalyzer[] {new RuFrequencyTextAnalyzer()};

        var utfDecoders = new IUtfDecoder[]
        {
            new Utf8Decoder(codepointConverter, textParserSettings),
            new Utf16LeDecoder(codepointConverter, textParserSettings),
            new Utf16BeDecoder(codepointConverter, textParserSettings),
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
    public async Task Should_read_utf8_without_bom_with_japanese_characters_inside_bmp()
    {
        var file = @"./TestData/text/utf8_bmp";
        var binaryReader = await CreateBinaryReaderAsync(file);

        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        text.Should().Be(ReadFileAsUtf8(file));
        encoding.Should().BeEquivalentTo(WellKnownEncodings.Utf8NoBOM);
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
        encoding.Should().BeEquivalentTo(new Windows1251Decoder(TextParserSettings.Default).Encoding);
        text.Should().Be(TextSamples.RussianLanguageSample);
    }
    
    [Test]
    public async Task Should_not_read_pseudo_utf16be_file_when_there_is_a_lot_of_uncommon_cjk_chars_and_no_chars_outside_bmp()
    {
        var binaryReader = await CreateBinaryReaderAsync(@"./TestData/text_pseudo_utf16/be_tasks.svgz");
    
        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeFalse();
        encoding.Should().BeEquivalentTo(null);
        text.Should().Be(null);
    }
    
    [Test]
    public async Task Should_not_read_pseudo_utf16le_file_when_there_is_a_lot_of_uncommon_cjk_chars_and_no_chars_outside_bmp()
    {
        var binaryReader = await CreateBinaryReaderAsync(@"./TestData/text_pseudo_utf16/le_apport.svgz");
    
        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeFalse();
        encoding.Should().BeEquivalentTo(null);
        text.Should().Be(null);
    }

    [Test]
    public async Task Should_read_utf16le_with_regular_japanese_text()
    {
        var filename = @"./TestData/text_utf16/Japanese_language_sample_le";
        var binaryReader = await CreateBinaryReaderAsync(filename);

        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        text.Should().Be(ReadFileAsUtf16Le(filename));
        encoding.Should().BeEquivalentTo(WellKnownEncodings.UTF16LeNoBom);
    }

    [Test]
    public async Task Should_read_utf16be_with_regular_japanese_text()
    {
        var filename = @"./TestData/text_utf16/Japanese_language_sample_be";
        var binaryReader = await CreateBinaryReaderAsync(filename);

        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(WellKnownEncodings.UTF16BeNoBom);
        text.Should().Be(ReadFileAsUtf16Be(filename));
    }

    [Test]
    public async Task Should_read_utf16be_with_classical_chinise_text()
    {
        var filename = @"./TestData/text_utf16/literary_chinese_utf16be";
        var binaryReader = await CreateBinaryReaderAsync(filename);

        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(WellKnownEncodings.UTF16BeNoBom);
        text.Should().Be(ReadFileAsUtf16Be(filename));
    }
    
    [Test]
    public async Task Should_read_utf16le_with_classical_chinise_text()
    {
        var filename = @"./TestData/text_utf16/literary_chinese_utf16le";
        var binaryReader = await CreateBinaryReaderAsync(filename);

        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(WellKnownEncodings.UTF16LeNoBom);
        text.Should().Be(ReadFileAsUtf16Le(filename));
    }
    
    [Test]
    public async Task Should_read_utf16le_with_cuneiform_text_with_characters_outside_bmp()
    {
        var filename = @"./TestData/text_utf16/cuneiform_le";
        var binaryReader = await CreateBinaryReaderAsync(filename);

        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(WellKnownEncodings.UTF16LeNoBom);
        text.Should().Be(ReadFileAsUtf16Le(filename));
    }
    
    [Test]
    public async Task Should_read_utf16be_with_cuneiform_text_with_characters_outside_bmp()
    {
        var filename = @"./TestData/text_utf16/cuneiform_be";
        var binaryReader = await CreateBinaryReaderAsync(filename);

        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(WellKnownEncodings.UTF16BeNoBom);
        text.Should().Be(ReadFileAsUtf16Be(filename));
    }
    
    [Test]
    public async Task Should_read_utf16le_with_mixed_thai_and_chinese_characters()
    {
        var filename = @"./TestData/text_utf16/thai_mixed_with_chinese_le";
        var binaryReader = await CreateBinaryReaderAsync(filename);

        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(WellKnownEncodings.UTF16LeNoBom);
        text.Should().Be(ReadFileAsUtf16Le(filename));
    }
    
    [Test]
    public async Task Should_read_utf16be_with_mixed_thai_and_chinese_characters()
    {
        var filename = @"./TestData/text_utf16/thai_mixed_with_chinese_be";
        var binaryReader = await CreateBinaryReaderAsync(filename);

        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(WellKnownEncodings.UTF16BeNoBom);
        text.Should().Be(ReadFileAsUtf16Be(filename));
    }

    private static string ReadFileAsUtf8(string file)
    {
        return Encoding.UTF8.GetString(File.ReadAllBytes(file));
    }
    
    private static string ReadFileAsUtf16Le(string file)
    {
        return Encoding.Unicode.GetString(File.ReadAllBytes(file));
    }
    
    private static string ReadFileAsUtf16Be(string file)
    {
        return Encoding.BigEndianUnicode.GetString(File.ReadAllBytes(file));
    }

    private static async Task<InMemoryBinaryReader> CreateBinaryReaderAsync(string filename)
    {
        var content = await File.ReadAllBytesAsync(filename);
        return new InMemoryBinaryReader(content);
    }
}