using System.Text;
using System.Threading.Channels;
using FluentAssertions;
using FormatParser.Tests.TestData;
using FormatParser.Text;
using FormatParser.Text.Encoding;
using FormatParser.Text.UtfDecoders;
using NUnit.Framework;

namespace FormatParser.Tests;

public class Utf8EncodingDecoder_Tests : TestBase
{
    private Utf8Decoder decoder = null!;
    private char[] charBuffer = null!;
    private string expectedString= null!;

    [SetUp]
    public void SetUp()
    {
        var textFileParsingSettings = new TextFileParsingSettings();
        decoder = new Utf8Decoder(textFileParsingSettings);
        charBuffer = new char[8096];
        expectedString = ReadFileAsUtf8(GetFile(TestFileCategory.Text, "utf8_bmp"));
    }
    
    [Test]
    public void Should_not_throw_when_broken_character_positions_at_the_end_of_input()
    {
        var file = GetFile(TestFileCategory.Text, "utf8_bmp_broken_at_end");

        var bytes = File.ReadAllBytes(file);
        var result = decoder.TryDecodeText(bytes, charBuffer);

        result.Should().NotBeNull();
        var str = GetString(result!.Chars);
        str.Should().Be(expectedString[..^1]);
    }
    
    [Test]
    public void Should_not_throw_when_input_is_correct()
    {
        var file = GetFile(TestFileCategory.Text, "utf8_bmp");

        var bytes = File.ReadAllBytes(file);
        var result = decoder.TryDecodeText(bytes, charBuffer);

        result.Should().NotBeNull();
        var str = GetString(result!.Chars);
        str.Should().Be(expectedString);
    }
    
    [Test]
    public void Should_return_null_when_broken_character_positions_in_the_middle_of_the_input()
    {
        var file = GetFile(TestFileCategory.Text, "utf8_bmp_broken_at_end_and_in_the_middle");

        var bytes = File.ReadAllBytes(file);
        var result = decoder.TryDecodeText(bytes, charBuffer);

        result.Should().BeNull();
    }

    private static string GetString(ArraySegment<char> arraySegment)
    {
        var memory = new Memory<char>(arraySegment.Array!, arraySegment.Offset, arraySegment.Count);
        var sb = new StringBuilder().Append(memory);
        return sb.ToString();
    }
}