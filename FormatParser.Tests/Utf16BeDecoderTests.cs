using System.Text;
using FluentAssertions;
using FormatParser.Text;
using NUnit.Framework;

namespace FormatParser.Tests;

public class Utf16BeDecoder_Tests
{
    private Utf16BeDecoder decoder;

    [SetUp]
    public void SetUp()
    {
        var codepointConverter = new CodepointConverter();
        var textParserSettings = TextParserSettings.Default;
        
        decoder = new Utf16BeDecoder(codepointConverter, textParserSettings);
    }
    
    [Test]
    public async Task Should_read_utf16_be_no_bom()
    {
        var content = File.ReadAllBytes(@"./TestData/text/loren_utf16_be_nobom");
    
        var deserializer = new InMemoryBinaryReader(content);
        var sb = new StringBuilder();

        var isSuccessful = decoder.TryDecode(deserializer, sb, out var encoding);

        isSuccessful.Should().Be(true);
        encoding.Should().BeEquivalentTo(WellKnownEncodings.UTF16BeNoBom);
        sb.ToString().Should().BeEquivalentTo(TextSamples.TextWithOnlyAsciiChars);
    }
}