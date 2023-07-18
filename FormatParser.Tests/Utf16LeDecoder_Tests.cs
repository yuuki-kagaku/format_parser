using FluentAssertions;
using FormatParser.Text;
using FormatParser.Text.UtfDecoders;
using NUnit.Framework;

namespace FormatParser.Tests;

public class Utf16LeDecoder_Tests : TestBase
{
    private Utf16LeDecoder decoder = null!;

    [SetUp]
    public void SetUp()
    {
        var textParserSettings = new TextFileParsingSettings();
        
        decoder = new Utf16LeDecoder(textParserSettings);
    }
    
    [Test]
    public void Should_read_utf16_le_no_bom()
    {
        var filename = GetFile(TestFileCategory.Text, "loren_utf16_le_nobom");
        var content = File.ReadAllBytes(filename);
    
        var chars = new char[content.Length];
        
        var textDecodingResult = decoder.TryDecodeText(content, chars);
        textDecodingResult.Should().NotBeNull();

        textDecodingResult!.Encoding.Should().BeEquivalentTo(EncodingInfo.UTF16LeNoBom);
        BuildString(textDecodingResult.Chars).Should().BeEquivalentTo(ReadFileAsUtf16Le(filename));
    }
}