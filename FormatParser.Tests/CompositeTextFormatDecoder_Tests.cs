using System.Text;
using FluentAssertions;
using FormatParser.Tests.TestData;
using FormatParser.Text;
using FormatParser.Text.Encoding;
using FormatParser.Windows1251;
using NUnit.Framework;

namespace FormatParser.Tests;

public class CompositeTextFormatDecoder_Tests : TestBase
{
    private CompositeTextFormatDecoder decoder = null!;

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
            new Utf8Decoder(),
            new Utf16LeDecoder(),
            new Utf16BeDecoder(),
            new Utf32LeDecoder(),
            new Utf32BeDecoder(),
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
        var binaryReader = await GetBufferAsync(GetFile(TestFileCategory.Text, "loren_utf8_no_bom"));

        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        text.Should().Be(TextSamples.TextWithOnlyAsciiChars);
        encoding.Should().BeEquivalentTo(WellKnownEncodings.ASCII);
    }
    
    [Test]
    public async Task Should_read_utf8_without_bom_with_japanese_characters_inside_bmp()
    {
        var file = GetFile(TestFileCategory.Text, "utf8_bmp");
        var binaryReader = await GetBufferAsync(file);

        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        text.Should().Be(ReadFileAsUtf8(file));
        encoding.Should().BeEquivalentTo(WellKnownEncodings.Utf8NoBOM);
    }
        
    [Test]
    public async Task Should_read_utf16_be_no_bom()
    {
        var binaryReader = await GetBufferAsync(GetFile(TestFileCategory.Text, "loren_utf16_be_nobom"));

        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        text.Should().Be(TextSamples.TextWithOnlyAsciiChars);
        encoding.Should().BeEquivalentTo(WellKnownEncodings.UTF16BeNoBom);
    }
    
    [Test]
    public async Task Should_read_utf16_le_no_bom()
    {
        var binaryReader = await GetBufferAsync(GetFile(TestFileCategory.Text, "loren_utf16_le_nobom"));

        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(WellKnownEncodings.UTF16LeNoBom);
        text.Should().Be(TextSamples.TextWithOnlyAsciiChars);
    }
    
    [Test]
    public async Task Should_read_windows1251()
    {
        var binaryReader = await GetBufferAsync(GetFile(TestFileCategory.Text, "bsd_windows1251"));
    
        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(new Windows1251Decoder(new TextFileParsingSettings()).EncodingWithoutBom);
        text.Should().Be(TextSamples.RussianLanguageSample);
    }
    
    [Test]
    public async Task Should_not_read_pseudo_utf16be_file_when_there_is_a_lot_of_uncommon_cjk_chars_and_no_chars_outside_bmp()
    {
        var binaryReader = await GetBufferAsync(GetFile(TestFileCategory.PseudoText, "be_tasks.svgz"));
    
        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeFalse();
        encoding.Should().BeEquivalentTo(null);
        text.Should().Be(null);
    }
    
    [Test]
    public async Task Should_not_read_pseudo_utf16le_file_when_there_is_a_lot_of_uncommon_cjk_chars_and_no_chars_outside_bmp()
    {
        var binaryReader = await GetBufferAsync(GetFile(TestFileCategory.PseudoText, "le_apport.svgz"));
    
        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeFalse();
        encoding.Should().BeEquivalentTo(null);
        text.Should().Be(null);
    }

    [Test]
    public async Task Should_read_utf16le_with_regular_japanese_text()
    {
        var filename = GetFile(TestFileCategory.TextUtf16, "Japanese_language_sample_le");
        var binaryReader = await GetBufferAsync(filename);

        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        text.Should().Be(ReadFileAsUtf16Le(filename));
        encoding.Should().BeEquivalentTo(WellKnownEncodings.UTF16LeNoBom);
    }

    [Test]
    public async Task Should_read_utf16be_with_regular_japanese_text()
    {
        var filename = GetFile(TestFileCategory.TextUtf16, "Japanese_language_sample_be");
        var binaryReader = await GetBufferAsync(filename);

        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(WellKnownEncodings.UTF16BeNoBom);
        text.Should().Be(ReadFileAsUtf16Be(filename));
    }

    [Test]
    public async Task Should_read_utf16be_with_classical_chinese_text()
    {
        var filename = GetFile(TestFileCategory.TextUtf16, "literary_chinese_utf16be");
        var binaryReader = await GetBufferAsync(filename);

        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(WellKnownEncodings.UTF16BeNoBom);
        text.Should().Be(ReadFileAsUtf16Be(filename));
    }
    
    [Test]
    public async Task Should_read_utf16le_with_classical_chinese_text()
    {
        var filename = GetFile(TestFileCategory.TextUtf16, "literary_chinese_utf16le");
        var binaryReader = await GetBufferAsync(filename);

        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(WellKnownEncodings.UTF16LeNoBom);
        text.Should().Be(ReadFileAsUtf16Le(filename));
    }
    
    [Test]
    public async Task Should_read_utf16le_with_cuneiform_text_with_characters_outside_bmp()
    {
        var filename = GetFile(TestFileCategory.TextUtf16, "cuneiform_le");
        var binaryReader = await GetBufferAsync(filename);

        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(WellKnownEncodings.UTF16LeNoBom);
        text.Should().Be(ReadFileAsUtf16Le(filename));
    }
    
    [Test]
    public async Task Should_read_utf16be_with_cuneiform_text_with_characters_outside_bmp()
    {
        var filename = GetFile(TestFileCategory.TextUtf16, "cuneiform_be");
        var binaryReader = await GetBufferAsync(filename);

        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(WellKnownEncodings.UTF16BeNoBom);
        text.Should().Be(ReadFileAsUtf16Be(filename));
    }
    
    [Test]
    public async Task Should_read_utf16le_with_mixed_thai_and_chinese_characters()
    {
        var filename = GetFile(TestFileCategory.TextUtf16, "thai_mixed_with_chinese_le");
        var binaryReader = await GetBufferAsync(filename);

        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(WellKnownEncodings.UTF16LeNoBom);
        text.Should().Be(ReadFileAsUtf16Le(filename));
    }
    
    [Test]
    public async Task Should_read_utf16be_with_mixed_thai_and_chinese_characters()
    {
        var filename = GetFile(TestFileCategory.TextUtf16, "thai_mixed_with_chinese_be");
        var binaryReader = await GetBufferAsync(filename);

        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(WellKnownEncodings.UTF16BeNoBom);
        text.Should().Be(ReadFileAsUtf16Be(filename));
    }

    private static async Task<ArraySegment<byte>> GetBufferAsync(string filename)
    {
        var content = await File.ReadAllBytesAsync(filename);
        return content;
    }
}