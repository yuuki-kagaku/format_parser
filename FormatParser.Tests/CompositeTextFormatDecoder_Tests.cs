using FluentAssertions;
using FormatParser.Domain;
using FormatParser.Text;
using FormatParser.Text.Decoders;
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
        
        var textAnalyzers = new ITextAnalyzer[]
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

        decoder = new CompositeTextFormatDecoder(textDecoders, textAnalyzers);
    }
  
    private static IEnumerable<TestCaseData> TestData()
    {
        yield return new TestCaseData(
                TestFileCategory.TextUtf8, "loren_utf8_no_bom", new Func<string, string>(ReadFileAsUtf8), WellKnownEncodingInfos.Ascii)
            .SetName("Should_read_ascii");
        
        yield return new TestCaseData(
                TestFileCategory.TextUtf8, "utf8_bmp", new Func<string, string>(ReadFileAsUtf8), WellKnownEncodingInfos.Utf8NoBom)
            .SetName("Should_read_utf8_without_bom_with_japanese_characters_inside_bmp");
        
        yield return new TestCaseData(
                TestFileCategory.TextUtf8, "utf8_bom", new Func<string, string>(ReadFileAsUtf8), WellKnownEncodingInfos.Utf8Bom)
            .SetName("Should_read_utf8_with_bom");
   
        yield return new TestCaseData(
                TestFileCategory.TextUtf16, "loren_utf16_be_nobom", new Func<string, string>(ReadFileAsUtf16Be), WellKnownEncodingInfos.Utf16BeNoBom)
            .SetName("Should_read_utf16_be_no_bom");

        yield return new TestCaseData(
                TestFileCategory.TextUtf16, "loren_utf16_le_nobom", new Func<string, string>(ReadFileAsUtf16Le), WellKnownEncodingInfos.Utf16LeNoBom)
            .SetName("Should_read_utf16_le_no_bom");

        yield return new TestCaseData(
                TestFileCategory.Text, "bsd_windows1251", new Func<string, string>(ReadFileAsWindows1251), new Windows1251Decoder(new TextFileParsingSettings()).EncodingWithoutBom)
            .SetName("Should_read_windows1251");
        
        yield return new TestCaseData(
                TestFileCategory.TextUtf16, "Japanese_language_sample_le", new Func<string, string>(ReadFileAsUtf16Le), WellKnownEncodingInfos.Utf16LeNoBom)
            .SetName("Should_read_utf16le_with_regular_japanese_text");
        
        yield return new TestCaseData(
                TestFileCategory.TextUtf16, "Japanese_language_sample_be", new Func<string, string>(ReadFileAsUtf16Be), WellKnownEncodingInfos.Utf16BeNoBom)
            .SetName("Should_read_utf16be_with_regular_japanese_text");
        
        yield return new TestCaseData(
                TestFileCategory.TextUtf32, "loren_utf32_le_no_bom", new Func<string, string>(ReadFileAsUtf32Le), WellKnownEncodingInfos.Utf32LeNoBom)
            .SetName("Should_read_utf32_le_no_bom");

        yield return new TestCaseData(
                TestFileCategory.TextUtf32, "loren_utf32_le_bom", new Func<string, string>(ReadFileAsUtf32Le), WellKnownEncodingInfos.Utf32LeBom)
            .SetName("Should_read_utf32_le_bom");
        
        yield return new TestCaseData(
                TestFileCategory.TextUtf32, "loren_utf32_be_bom", new Func<string, string>(ReadFileAsUtf32Be), WellKnownEncodingInfos.Utf32BeBom)
            .SetName("Should_read_utf32_be_bom");

        yield return new TestCaseData(
                TestFileCategory.TextUtf32, "loren_utf32_be_nobom", new Func<string, string>(ReadFileAsUtf32Be), WellKnownEncodingInfos.Utf32BeNoBom)
            .SetName("Should_read_utf32_be_no_bom");
        
        yield return new TestCaseData(
                TestFileCategory.TextUtf16, "literary_chinese_utf16be", new Func<string, string>(ReadFileAsUtf16Be), WellKnownEncodingInfos.Utf16BeNoBom)
            .SetName("Should_read_utf16be_with_classical_chinese_text");

        yield return new TestCaseData(
                TestFileCategory.TextUtf16, "literary_chinese_utf16le", new Func<string, string>(ReadFileAsUtf16Le), WellKnownEncodingInfos.Utf16LeNoBom)
            .SetName("Should_read_utf16le_with_classical_chinese_text");
        
        yield return new TestCaseData(
                TestFileCategory.TextUtf16, "cuneiform_le", new Func<string, string>(ReadFileAsUtf16Le), WellKnownEncodingInfos.Utf16LeNoBom)
            .SetName("Should_read_utf16le_with_cuneiform_text_with_characters_outside_bmp");

        yield return new TestCaseData(
                TestFileCategory.TextUtf16, "cuneiform_be", new Func<string, string>(ReadFileAsUtf16Be), WellKnownEncodingInfos.Utf16BeNoBom)
            .SetName("Should_read_utf16be_with_cuneiform_text_with_characters_outside_bmp");

        yield return new TestCaseData(
                TestFileCategory.TextUtf16, "thai_mixed_with_chinese_le", new Func<string, string>(ReadFileAsUtf16Le), WellKnownEncodingInfos.Utf16LeNoBom)
            .SetName("Should_read_utf16le_with_mixed_thai_and_chinese_characters");

        yield return new TestCaseData(
                TestFileCategory.TextUtf16, "thai_mixed_with_chinese_be", new Func<string, string>(ReadFileAsUtf16Be), WellKnownEncodingInfos.Utf16BeNoBom)
            .SetName("Should_read_utf16be_with_mixed_thai_and_chinese_characters");
    }

    [TestCaseSource(nameof(TestData))]
    public async Task Should_correctly_read_text_and_identify_encoding(TestFileCategory testFileCategory, string file, Func<string, string> getContent, EncodingInfo expectedEncoding)
    {
        var filename = GetFile(testFileCategory, file);
        var buffer = await GetBufferAsync(filename);

        var isSuccessful = decoder.TryDecode(buffer, out var encoding, out var text);

        isSuccessful.Should().BeTrue();
        text.Should().Be(getContent(filename));
        encoding.Should().BeEquivalentTo(expectedEncoding);
    }

    [TestCase(TestFileCategory.PseudoText, "be_tasks.svgz", TestName = "Should_not_read_pseudo_utf16be_file_when_there_is_a_lot_of_uncommon_cjk_chars_and_no_chars_outside_bmp")]
    [TestCase(TestFileCategory.PseudoText, "le_apport.svgz", TestName = "Should_not_read_pseudo_utf16le_file_when_there_is_a_lot_of_uncommon_cjk_chars_and_no_chars_outside_bmp")]
    public async Task Should_not_read_binary_file_that_can_be_interpreted_as_text(TestFileCategory testFileCategory, string filename)
    {
        var buffer = await GetBufferAsync(GetFile(testFileCategory, filename));
    
        var isSuccessful = decoder.TryDecode(buffer, out var encoding, out var text);

        isSuccessful.Should().BeFalse();
        encoding.Should().Be(null);
        text.Should().Be(null);
    }

    private static async Task<ArraySegment<byte>> GetBufferAsync(string filename)
    {
        var content = await File.ReadAllBytesAsync(filename);
        return content;
    }
}