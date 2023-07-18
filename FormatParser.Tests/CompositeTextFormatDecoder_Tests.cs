using FluentAssertions;
using FormatParser.Text;
using FormatParser.Text.Decoders;
using FormatParser.Text.EncodingAnalyzers;
using FormatParser.Text.TextAnalyzers;
using FormatParser.Windows1251;
using NUnit.Framework;
using EncodingInfo = FormatParser.Domain.EncodingInfo;

namespace FormatParser.Tests;

public class CompositeTextFormatDecoder_Tests : TestBase
{
    private CompositeTextFormatDecoder decoder = null!;

    [SetUp]
    public void SetUp()
    {
        var textParserSettings = new TextFileParsingSettings();
        
        var languageAnalyzers = new ITextAnalyzer[]
        {
            new AsciiCharactersTextAnalyzer(),
            new UTF16Heuristics(),
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
        };

        decoder = new CompositeTextFormatDecoder(
            textDecoders, 
            languageAnalyzers,
            textParserSettings);
    }
    
    [Test]
    public async Task Should_read_ascii()
    {
        var filename = GetFile(TestFileCategory.TextUtf8, "loren_utf8_no_bom");
        var buffer = await GetBufferAsync(filename);

        var isSuccessful = decoder.TryDecode(buffer, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        text.Should().Be(ReadFileAsUtf8(filename));
        encoding.Should().BeEquivalentTo(EncodingInfo.ASCII);
    }
    
    [Test]
    public async Task Should_read_utf8_without_bom_with_japanese_characters_inside_bmp()
    {
        var file = GetFile(TestFileCategory.TextUtf8, "utf8_bmp");
        var buffer = await GetBufferAsync(file);

        var isSuccessful = decoder.TryDecode(buffer, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        text.Should().Be(ReadFileAsUtf8(file));
        encoding.Should().BeEquivalentTo(EncodingInfo.Utf8NoBOM);
    }
    
    [Test]
    public async Task Should_read_utf8_with_bom()
    {
        var file = GetFile(TestFileCategory.TextUtf8, "utf8_bom");
        var buffer = await GetBufferAsync(file);

        var isSuccessful = decoder.TryDecode(buffer, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        text.Should().Be(ReadFileAsUtf8(file));
        encoding.Should().BeEquivalentTo(EncodingInfo.Utf8BOM);
    }

        
    [Test]
    public async Task Should_read_utf16_be_no_bom()
    {
        var filename = GetFile(TestFileCategory.TextUtf16, "loren_utf16_be_nobom");
        var buffer = await GetBufferAsync(filename);

        var isSuccessful = decoder.TryDecode(buffer, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        text.Should().Be(ReadFileAsUtf16Be(filename));
        encoding.Should().BeEquivalentTo(EncodingInfo.UTF16BeNoBom);
    }
    
    [Test]
    public async Task Should_read_utf16_le_no_bom()
    {
        var filename = GetFile(TestFileCategory.TextUtf16, "loren_utf16_le_nobom");
        var buffer = await GetBufferAsync(filename);

        var isSuccessful = decoder.TryDecode(buffer, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(EncodingInfo.UTF16LeNoBom);
        text.Should().Be(ReadFileAsUtf16Le(filename));
    }
    
    [Test]
    public async Task Should_read_windows1251()
    {
        var filename = GetFile(TestFileCategory.Text, "bsd_windows1251");
        var buffer = await GetBufferAsync(filename);
    
        var isSuccessful = decoder.TryDecode(buffer, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(new Windows1251Decoder(new TextFileParsingSettings()).EncodingWithoutBom);
        text.Should().Be(ReadFileAsWindows1251(filename));
    }
    
    [Test]
    public async Task Should_not_read_pseudo_utf16be_file_when_there_is_a_lot_of_uncommon_cjk_chars_and_no_chars_outside_bmp()
    {
        var buffer = await GetBufferAsync(GetFile(TestFileCategory.PseudoText, "be_tasks.svgz"));
    
        var isSuccessful = decoder.TryDecode(buffer, out var encoding, out var text);

        isSuccessful.Should().BeFalse();
        encoding.Should().Be(null);
        text.Should().Be(null);
    }
    
    [Test]
    public async Task Should_not_read_pseudo_utf16le_file_when_there_is_a_lot_of_uncommon_cjk_chars_and_no_chars_outside_bmp()
    {
        var buffer = await GetBufferAsync(GetFile(TestFileCategory.PseudoText, "le_apport.svgz"));
    
        var isSuccessful = decoder.TryDecode(buffer, out var encoding, out var text);

        isSuccessful.Should().BeFalse();
        encoding.Should().Be(null);
        text.Should().Be(null);
    }

    [Test]
    public async Task Should_read_utf16le_with_regular_japanese_text()
    {
        var filename = GetFile(TestFileCategory.TextUtf16, "Japanese_language_sample_le");
        var buffer = await GetBufferAsync(filename);

        var isSuccessful = decoder.TryDecode(buffer, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        text.Should().Be(ReadFileAsUtf16Le(filename));
        encoding.Should().BeEquivalentTo(EncodingInfo.UTF16LeNoBom);
    }

    [Test]
    public async Task Should_read_utf16be_with_regular_japanese_text()
    {
        var filename = GetFile(TestFileCategory.TextUtf16, "Japanese_language_sample_be");
        var buffer = await GetBufferAsync(filename);

        var isSuccessful = decoder.TryDecode(buffer, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(EncodingInfo.UTF16BeNoBom);
        text.Should().Be(ReadFileAsUtf16Be(filename));
    }
    
    [Test]
    public async Task Should_read_utf32_le_no_bom()
    {
        var filename = GetFile(TestFileCategory.TextUtf32, "loren_utf32_le_no_bom");

        var binaryReader = await GetBufferAsync(filename);
        
        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);
        
        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(EncodingInfo.UTF32LeNoBom);
        text.Should().Be(ReadFileAsUtf32Le(filename));
    }
    
    [Test]
    public async Task Should_read_utf32_le_bom()
    {
        var filename = GetFile(TestFileCategory.TextUtf32, "loren_utf32_le_bom");

        var binaryReader = await GetBufferAsync(filename);
        
        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);
        
        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(EncodingInfo.UTF32LeBom);
        text.Should().Be(ReadFileAsUtf32Le(filename));
    }
    
        
    [Test]
    public async Task Should_read_utf32_be_bom()
    {
        var filename = GetFile(TestFileCategory.TextUtf32, "loren_utf32_be_bom");

        var binaryReader = await GetBufferAsync(filename);
        
        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);
        
        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(EncodingInfo.UTF32BeBom);
        text.Should().Be(ReadFileAsUtf32Be(filename));
    }
    
    [Test]
    public async Task Should_read_utf32_be_no_bom()
    {
        var filename = GetFile(TestFileCategory.TextUtf32, "loren_utf32_be_nobom");

        var binaryReader = await GetBufferAsync(filename);
        
        var isSuccessful = decoder.TryDecode(binaryReader, out var encoding, out var text);
        
        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(EncodingInfo.UTF32BeNoBom);
        text.Should().Be(ReadFileAsUtf32Be(filename));
    }

    [Test]
    public async Task Should_read_utf16be_with_classical_chinese_text()
    {
        var filename = GetFile(TestFileCategory.TextUtf16, "literary_chinese_utf16be");
        var buffer = await GetBufferAsync(filename);

        var isSuccessful = decoder.TryDecode(buffer, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(EncodingInfo.UTF16BeNoBom);
        text.Should().Be(ReadFileAsUtf16Be(filename));
    }
    
    [Test]
    public async Task Should_read_utf16le_with_classical_chinese_text()
    {
        var filename = GetFile(TestFileCategory.TextUtf16, "literary_chinese_utf16le");
        var buffer = await GetBufferAsync(filename);

        var isSuccessful = decoder.TryDecode(buffer, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(EncodingInfo.UTF16LeNoBom);
        text.Should().Be(ReadFileAsUtf16Le(filename));
    }
    
    [Test]
    public async Task Should_read_utf16le_with_cuneiform_text_with_characters_outside_bmp()
    {
        var filename = GetFile(TestFileCategory.TextUtf16, "cuneiform_le");
        var buffer = await GetBufferAsync(filename);

        var isSuccessful = decoder.TryDecode(buffer, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(EncodingInfo.UTF16LeNoBom);
        text.Should().Be(ReadFileAsUtf16Le(filename));
    }
    
    [Test]
    public async Task Should_read_utf16be_with_cuneiform_text_with_characters_outside_bmp()
    {
        var filename = GetFile(TestFileCategory.TextUtf16, "cuneiform_be");
        var buffer = await GetBufferAsync(filename);

        var isSuccessful = decoder.TryDecode(buffer, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(EncodingInfo.UTF16BeNoBom);
        text.Should().Be(ReadFileAsUtf16Be(filename));
    }
    
    [Test]
    public async Task Should_read_utf16le_with_mixed_thai_and_chinese_characters()
    {
        var filename = GetFile(TestFileCategory.TextUtf16, "thai_mixed_with_chinese_le");
        var buffer = await GetBufferAsync(filename);

        var isSuccessful = decoder.TryDecode(buffer, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(EncodingInfo.UTF16LeNoBom);
        text.Should().Be(ReadFileAsUtf16Le(filename));
    }
    
    [Test]
    public async Task Should_read_utf16be_with_mixed_thai_and_chinese_characters()
    {
        var filename = GetFile(TestFileCategory.TextUtf16, "thai_mixed_with_chinese_be");
        var buffer = await GetBufferAsync(filename);

        var isSuccessful = decoder.TryDecode(buffer, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(EncodingInfo.UTF16BeNoBom);
        text.Should().Be(ReadFileAsUtf16Be(filename));
    }

    private static async Task<ArraySegment<byte>> GetBufferAsync(string filename)
    {
        var content = await File.ReadAllBytesAsync(filename);
        return content;
    }
}