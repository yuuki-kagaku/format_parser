using FluentAssertions;
using FormatParser.Text;
using FormatParser.Text.UtfDecoders;
using NUnit.Framework;

namespace FormatParser.Tests;

public class Utf8EncodingDecoder_Tests : TestBase
{
    private Utf8Decoder decoder = null!;
    private string expectedString= null!;

    [SetUp]
    public void SetUp()
    {
        var textFileParsingSettings = new TextFileParsingSettings();
        decoder = new Utf8Decoder(textFileParsingSettings);
        expectedString = ReadFileAsUtf8(GetFile(TestFileCategory.Text, "utf8_bmp"));
    }
    
    [Test]
    public void Should_not_throw_when_broken_character_positions_at_the_end_of_input()
    {
        var file = GetFile(TestFileCategory.Text, "utf8_bmp_broken_at_end");

        var bytes = File.ReadAllBytes(file);
        var result = decoder.TryDecodeText(bytes);

        result.Should().NotBeNull();
        var str = BuildString(result!.Chars);
        str.Should().Be(expectedString[..^1]);
    }
    
    [Test]
    public void Should_not_throw_when_input_is_correct()
    {
        var file = GetFile(TestFileCategory.Text, "utf8_bmp");

        var bytes = File.ReadAllBytes(file);
        var result = decoder.TryDecodeText(bytes);

        result.Should().NotBeNull();
        var str = BuildString(result!.Chars);
        str.Should().Be(expectedString);
    }
    
    [Test]
    public void Should_return_null_when_broken_character_positions_in_the_middle_of_the_input()
    {
        var file = GetFile(TestFileCategory.Text, "utf8_bmp_broken_at_end_and_in_the_middle");

        var bytes = File.ReadAllBytes(file);
        var result = decoder.TryDecodeText(bytes);

        result.Should().BeNull();
    }
}