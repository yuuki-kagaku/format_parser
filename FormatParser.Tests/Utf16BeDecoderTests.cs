using System.Text;
using FluentAssertions;
using FormatParser.Helpers;
using FormatParser.Tests.TestData;
using FormatParser.Text;
using NUnit.Framework;

namespace FormatParser.Tests;

public class Utf16BeDecoder_Tests : TestBase
{
    private Utf16BeDecoder decoder = null!;

    [SetUp]
    public void SetUp()
    {
        var textParserSettings = new TextFileParsingSettings();
        
        decoder = new Utf16BeDecoder();
    }
    
    [Test]
    public void Should_read_utf16_be_no_bom()
    {
        var filename = GetFile(TestFileCategory.Text, "loren_utf16_be_nobom");
        var content = File.ReadAllBytes(filename);
    
        var chars = new char[content.Length];
        
        var textDecodingResult = decoder.TryDecodeText(content, chars);
        textDecodingResult.Should().NotBeNull();
        
        textDecodingResult!.Encoding.Should().BeEquivalentTo(WellKnownEncodings.UTF16BeNoBom);
        BuildString(textDecodingResult.Chars).Should().BeEquivalentTo(ReadFileAsUtf16Be(filename));
    }
}