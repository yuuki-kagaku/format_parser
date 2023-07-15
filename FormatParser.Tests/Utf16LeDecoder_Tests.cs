using System.Text;
using FluentAssertions;
using FormatParser.Text;
using NUnit.Framework;

namespace FormatParser.Tests;

public class Utf16LeDecoder_Tests
{
    private Utf16LeDecoder decoder;

    [SetUp]
    public void SetUp()
    {
        var codepointConverter = new CodepointConverter();
        var textParserSettings = TextParserSettings.Default;
        
        decoder = new Utf16LeDecoder(codepointConverter, textParserSettings);
    }
    
    [Test]
    public async Task Should_read_utf16_le_no_bom()
    {
        var content = File.ReadAllBytes(@"./TestData/text//loren_utf16_le_nobom");
    
        var deserializer = new InMemoryBinaryReader(content);
        var sb = new StringBuilder();

        var isSuccessful = decoder.TryDecode(deserializer, sb, out var encoding);
        
        isSuccessful.Should().Be(true);
        encoding.Should().BeEquivalentTo(WellKnownEncodings.UTF16LeNoBom);
        sb.ToString().Should().BeEquivalentTo(TextSamples.TextWithOnlyAsciiChars);
    }
}