using FluentAssertions;
using FormatParser.Domain;
using FormatParser.Text;
using FormatParser.Text.Decoders;
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
        var filename = GetFile(TestFileCategory.TextUtf16, "loren_utf16_le_nobom");
        var content = File.ReadAllBytes(filename);
    
        var textDecodingResult = decoder.TryDecodeText(content);
        textDecodingResult.Should().NotBeNull();

        textDecodingResult!.Encoding.Should().BeEquivalentTo(WellKnownEncodingInfos.Utf16LeNoBom);
        BuildString(textDecodingResult.Chars).Should().BeEquivalentTo(ReadFileAsUtf16Le(filename));
    }
}