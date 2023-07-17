using System.Text;
using FluentAssertions;
using FormatParser.Tests.TestData;
using FormatParser.Text;
using NUnit.Framework;

namespace FormatParser.Tests;

public class Utf16LeDecoder_Tests : TestBase
{
    private Utf16LeDecoder decoder = null!;

    [SetUp]
    public void SetUp()
    {
        var codepointConverter = new CodepointConverter();
        var textParserSettings = new TextFileParsingSettings();
        
        decoder = new Utf16LeDecoder();
    }
    
    [Test]
    public void Should_read_utf16_le_no_bom()
    {
        var content = File.ReadAllBytes(GetFile(TestFileCategory.Text, "loren_utf16_le_nobom"));
    
        var chars = new char[content.Length];
        
        var textDecodingResult = decoder.TryDecodeText(content, chars);
        var text = new StringBuilder().Append(new Memory<char>(textDecodingResult.Chars.Array, textDecodingResult.Chars.Offset, textDecodingResult.Chars.Count)).ToString();

        textDecodingResult.Should().NotBeNull();
        textDecodingResult.Encoding.Should().BeEquivalentTo(WellKnownEncodings.UTF16LeNoBom);
        text.Should().BeEquivalentTo(TextSamples.TextWithOnlyAsciiChars);
    }
}