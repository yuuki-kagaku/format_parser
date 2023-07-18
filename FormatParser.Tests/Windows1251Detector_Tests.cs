using FluentAssertions;
using FormatParser.Text;
using FormatParser.Windows1251;
using NUnit.Framework;

namespace FormatParser.Tests;

public class Windows1251Detector_Tests : TestBase
{
    private Windows1251Decoder decoder = null!;

    [SetUp]
    public void SetUp()
    {
        var textParserSettings = new TextFileParsingSettings();
        
        decoder = new Windows1251Decoder(textParserSettings);
    }
    
    [Test]
    public void Should_read_windows1251()
    {
        var content = File.ReadAllBytes(GetFile(TestFileCategory.Text, "bsd_windows1251"));
        
        var textDecodingResult = decoder.TryDecodeText(content);
        textDecodingResult.Should().NotBeNull();
        
        textDecodingResult!.Encoding.Should().BeEquivalentTo(decoder.EncodingWithoutBom);
        BuildString(textDecodingResult.Chars).Should().Be(TextSamples.RussianLanguageSample);
    }
}