using FluentAssertions;
using FormatParser.ELF;
using NUnit.Framework;

namespace FormatParser.Tests;

public class ElfParser_Tests
{
    [Test]
    public async Task ElfParser_ShouldParseAmd64LinuxExecutable()
    {
        var stream = new FileStream(@"./TestData/linux_amd64/vlc", FileMode.Open, FileAccess.Read);
        var data = await ElfParser.DeserializeAsync(stream);

        data.Result.Should().NotBeNull();
        data.Result!.Bitness.Should().Be(Bitness.Bitness64);
        data.Result!.Architecture.Should().Be(Architecture.Amd64);
        data.Result!.Endianess.Should().Be(Endianess.LittleEndian);
        data.Result!.Interpreter.Should().Be("/lib64/ld-linux-x86-64.so.2");
    }
}