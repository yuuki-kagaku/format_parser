using System.Text;
using FluentAssertions;
using FormatParser.Text;
using NUnit.Framework;

namespace FormatParser.Tests;

public class Windows1251Decoder_Tests
{
    private Windows1251Decoder decoder;

    [SetUp]
    public void SetUp()
    {
        var codepointChecker = new CodepointChecker();
        var textParserSettings = TextParserSettings.Default;
        
        decoder = new Windows1251Decoder(codepointChecker, textParserSettings);
    }
    
    [Test]
    public async Task Should_read_windows1251()
    {
        var content = File.ReadAllBytes(@"./TestData/text/bsd_windows1251");
    
        var deserializer = new InMemoryBinaryReader(content);
        var sb = new StringBuilder();

        var isSuccessful = decoder.TryDecode(deserializer, sb, out var encoding, out var probability);

        isSuccessful.Should().Be(true);
        encoding.Should().BeEquivalentTo(Windows1251Decoder.Encoding);
        sb.ToString().Should().Be(TextSamples.RussianLanguageSample);
    }
}