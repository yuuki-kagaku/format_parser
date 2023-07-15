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
        var textParserSettings = TextParserSettings.Default;
        
        decoder = new Windows1251Decoder(textParserSettings);
    }
    
    [Test]
    public async Task Should_read_windows1251()
    {
        var content = File.ReadAllBytes(@"./TestData/text/bsd_windows1251");
    
        var deserializer = new InMemoryBinaryReader(content);
        var sb = new StringBuilder();

        var isSuccessful = decoder.TryDecode(deserializer, sb, out var encoding);

        isSuccessful.Should().Be(true);
        encoding.Should().BeEquivalentTo(decoder.Encoding);
        sb.ToString().Should().Be(TextSamples.RussianLanguageSample);
    }
}