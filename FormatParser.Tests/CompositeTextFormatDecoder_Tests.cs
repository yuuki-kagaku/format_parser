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
        
        var textBasedFormatDetectors = new ITextBasedFormatDetector[]
        {
        };

        decoder = new CompositeTextFormatDecoder(textDecoders, textAnalyzers, textBasedFormatDetectors);
    }
  
    private static IEnumerable<TestCaseData> TestData()
    {
        yield return CreateTestCase(TestFileCategory.TextUtf8, "loren_utf8_no_bom", ReadFileAsUtf8, WellKnownEncodingInfos.Ascii, "Should_read_ascii");
        
        yield return CreateTestCase(TestFileCategory.TextUtf8, "utf8_bmp", ReadFileAsUtf8, WellKnownEncodingInfos.Utf8NoBom, "Should_read_utf8_without_bom_with_japanese_characters_inside_bmp");
        
        yield return CreateTestCase(TestFileCategory.TextUtf8, "utf8_bom", ReadFileAsUtf8, WellKnownEncodingInfos.Utf8Bom, "Should_read_utf8_with_bom");
   
        yield return CreateTestCase(TestFileCategory.TextUtf16, "loren_utf16_be_nobom", ReadFileAsUtf16Be, WellKnownEncodingInfos.Utf16BeNoBom, "Should_read_utf16_be_no_bom");

        yield return CreateTestCase(TestFileCategory.TextUtf16, "loren_utf16_le_nobom", ReadFileAsUtf16Le, WellKnownEncodingInfos.Utf16LeNoBom, "Should_read_utf16_le_no_bom");

        yield return CreateTestCase(TestFileCategory.Text, "bsd_windows1251", ReadFileAsWindows1251, new ("Windows-1251", Endianness.NotAllowed, false), "Should_read_windows1251");
        
        yield return CreateTestCase(TestFileCategory.TextUtf16, "Japanese_language_sample_le", ReadFileAsUtf16Le, WellKnownEncodingInfos.Utf16LeNoBom, "Should_read_utf16le_with_regular_japanese_text");
        
        yield return CreateTestCase(TestFileCategory.TextUtf16, "Japanese_language_sample_be", ReadFileAsUtf16Be, WellKnownEncodingInfos.Utf16BeNoBom, "Should_read_utf16be_with_regular_japanese_text");
        
        yield return CreateTestCase(TestFileCategory.TextUtf32, "loren_utf32_le_no_bom", ReadFileAsUtf32Le, WellKnownEncodingInfos.Utf32LeNoBom, "Should_read_utf32_le_no_bom");

        yield return CreateTestCase(TestFileCategory.TextUtf32, "loren_utf32_le_bom", ReadFileAsUtf32Le, WellKnownEncodingInfos.Utf32LeBom, "Should_read_utf32_le_bom");
        
        yield return CreateTestCase(TestFileCategory.TextUtf32, "loren_utf32_be_bom", ReadFileAsUtf32Be, WellKnownEncodingInfos.Utf32BeBom, "Should_read_utf32_be_bom");

        yield return CreateTestCase(TestFileCategory.TextUtf32, "loren_utf32_be_nobom", ReadFileAsUtf32Be, WellKnownEncodingInfos.Utf32BeNoBom, "Should_read_utf32_be_no_bom");
        
        yield return CreateTestCase(TestFileCategory.TextUtf16, "literary_chinese_utf16be", ReadFileAsUtf16Be, WellKnownEncodingInfos.Utf16BeNoBom, "Should_read_utf16be_with_classical_chinese_text");

        yield return CreateTestCase(TestFileCategory.TextUtf16, "literary_chinese_utf16le", ReadFileAsUtf16Le, WellKnownEncodingInfos.Utf16LeNoBom, "Should_read_utf16le_with_classical_chinese_text");
        
        yield return CreateTestCase(TestFileCategory.TextUtf16, "cuneiform_le", ReadFileAsUtf16Le, WellKnownEncodingInfos.Utf16LeNoBom, "Should_read_utf16le_with_cuneiform_text_with_characters_outside_bmp");

        yield return CreateTestCase(TestFileCategory.TextUtf16, "cuneiform_be", ReadFileAsUtf16Be, WellKnownEncodingInfos.Utf16BeNoBom, "Should_read_utf16be_with_cuneiform_text_with_characters_outside_bmp");

        yield return CreateTestCase(TestFileCategory.TextUtf16, "thai_mixed_with_chinese_le", ReadFileAsUtf16Le, WellKnownEncodingInfos.Utf16LeNoBom, "Should_read_utf16le_with_mixed_thai_and_chinese_characters");

        yield return CreateTestCase(TestFileCategory.TextUtf16, "thai_mixed_with_chinese_be", ReadFileAsUtf16Be, WellKnownEncodingInfos.Utf16BeNoBom,"Should_read_utf16be_with_mixed_thai_and_chinese_characters");
        
        yield break;

        static TestCaseData CreateTestCase(TestFileCategory testFileCategory, string file, Func<string, string> getContent, EncodingInfo expectedEncoding, string name) => 
            new TestCaseData(testFileCategory, file, new Func<string, string>(getContent), expectedEncoding).SetName(name);
    }
    
    [TestCaseSource(nameof(TestData))]
    public async Task Should_correctly_read_text_and_identify_encoding(TestFileCategory testFileCategory, string file, Func<string, string> getContent, EncodingInfo expectedEncoding)
    {
        var filename = GetFile(testFileCategory, file);
        var buffer = await GetBufferAsync(filename);

        var isSuccessful = decoder.TryDecode(buffer, out var encoding);

        isSuccessful.Should().BeTrue();
        encoding.Should().BeEquivalentTo(new TextFileFormatInfo("text/plain", expectedEncoding));
    }

    [TestCase(TestFileCategory.PseudoText, "be_tasks.svgz", TestName = "Should_not_read_pseudo_utf16be_file_when_there_is_a_lot_of_uncommon_cjk_chars_and_no_chars_outside_bmp")]
    [TestCase(TestFileCategory.PseudoText, "le_apport.svgz", TestName = "Should_not_read_pseudo_utf16le_file_when_there_is_a_lot_of_uncommon_cjk_chars_and_no_chars_outside_bmp")]
    public async Task Should_not_read_binary_file_that_can_be_interpreted_as_text(TestFileCategory testFileCategory, string filename)
    {
        var buffer = await GetBufferAsync(GetFile(testFileCategory, filename));
    
        var isSuccessful = decoder.TryDecode(buffer, out var encoding);

        isSuccessful.Should().BeFalse();
        encoding.Should().Be(null);
    }

    private static async Task<ArraySegment<byte>> GetBufferAsync(string filename)
    {
        var content = await File.ReadAllBytesAsync(filename);
        return content;
    }
}