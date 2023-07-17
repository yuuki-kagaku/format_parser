using System.Text;
using FluentAssertions;
using FormatParser.Tests.TestData;
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
        var chars = new char[content.Length];
        
        var textDecodingResult = decoder.TryDecodeText(content, chars);
        textDecodingResult.Should().NotBeNull();
        
        textDecodingResult!.Encoding.Should().BeEquivalentTo(decoder.EncodingWithoutBom);
        BuildString(chars).Should().Be(TextSamples.RussianLanguageSample);
    }
}