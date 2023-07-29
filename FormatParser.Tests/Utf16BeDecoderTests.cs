using FluentAssertions;
using FormatParser.Domain;
using FormatParser.Text;
using FormatParser.Text.Decoders;
using NUnit.Framework;

namespace FormatParser.Tests;

public class Utf16BeDecoder_Tests : TestBase
{
    private Utf16BeDecoder decoder = null!;

    [SetUp]
    public void SetUp()
    {
        var textParserSettings = new TextFileParsingSettings();
        
        decoder = new Utf16BeDecoder(textParserSettings);
    }
    
    [Test]
    public void Should_read_utf16_be_no_bom()
    {
        var filename = GetFile(TestFileCategory.TextUtf16, "loren_utf16_be_nobom");
        var content = File.ReadAllBytes(filename);
    
        var chars = new char[content.Length];
        
        var textDecodingResult = decoder.TryDecodeText(content);
        textDecodingResult.Should().NotBeNull();
        
        textDecodingResult!.Encoding.Should().BeEquivalentTo(WellKnownEncodingInfos.Utf16BeNoBom);
        BuildString(textDecodingResult.Chars).Should().BeEquivalentTo(ReadFileAsUtf16Be(filename));
    }
}