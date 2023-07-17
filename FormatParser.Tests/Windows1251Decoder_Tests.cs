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
        var textParserSettings = new TextFileParsingSettings();
        
        decoder = new Windows1251Decoder(textParserSettings);
    }
    
    [Test]
    public async Task Should_read_windows1251()
    {
        var content = File.ReadAllBytes(@"./TestData/text/bsd_windows1251");
        var chars = new char[content.Length];
        
        var textDecodingResult = decoder.TryDecodeText(content, chars);
        var text = new StringBuilder().Append(new Memory<char>(textDecodingResult.Chars.Array, textDecodingResult.Chars.Offset, textDecodingResult.Chars.Count)).ToString();

        textDecodingResult.Should().NotBeNull();
        textDecodingResult.Encoding.Should().BeEquivalentTo(decoder.EncodingWithoutBom);
        text.Should().Be(TextSamples.RussianLanguageSample);
    }
}