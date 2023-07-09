using FluentAssertions;
using FormatParser.ELF;
using FormatParser.MachO;
using NUnit.Framework;

namespace FormatParser.Tests;

public class MacParser_Tests
{
    [Test]
    public async Task MachOParser_ShouldParseMacExecutable()
    {
        var macParser = new MachODecoder();
        
        await using var stream = new FileStream(@"./TestData/mac/VLC", FileMode.Open, FileAccess.Read);
        var deserializer = new Deserializer(stream);
        
        var data = (await macParser.TryDecodeAsync(deserializer)) as MachOFileFormatInfo;

        data.Should().NotBeNull();
        data!.Bitness.Should().Be(Bitness.Bitness64);
        data!.Architecture.Should().Be(Architecture.Amd64);
        data!.Endianness.Should().Be(Endianness.LittleEndian);
        data.Signed!.Should().Be(false);
    }
}