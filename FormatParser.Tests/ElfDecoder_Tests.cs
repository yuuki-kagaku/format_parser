using FluentAssertions;
using FormatParser.ELF;
using NUnit.Framework;

namespace FormatParser.Tests;

public class ElfDecoder_Tests
{
    [Test]
    public async Task ElfDecoder_ShouldParseAmd64LinuxExecutable()
    {
        var elfDecoder = new ElfDecoder();

        await using var stream = new FileStream(@"./TestData/linux_amd64/vlc", FileMode.Open, FileAccess.Read);
        var deserializer = new Deserializer(stream);

        var data = await elfDecoder.TryDecodeAsync(deserializer) as ElfFileFormatInfo;

        data.Should().NotBeNull();
        data!.Bitness.Should().Be(Bitness.Bitness64);
        data!.Architecture.Should().Be(Architecture.Amd64);
        data.Endianness.Should().Be(Endianness.LittleEndian);
        data.Interpreter.Should().Be("/lib64/ld-linux-x86-64.so.2");
    }
}