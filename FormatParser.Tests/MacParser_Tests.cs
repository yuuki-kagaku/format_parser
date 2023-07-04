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
        var macParser = new MachOParser();
        
        var stream = new FileStream(@"./TestData/mac/VLC", FileMode.Open);
        var data = (await macParser.DeserializeAsync(stream)) as MachOData;

        data.Should().NotBeNull();
        data!.Bitness.Should().Be(Bitness.Bitness64);
        data!.Architecture.Should().Be(Architecture.Amd64);
        data!.Endianess.Should().Be(Endianess.LittleEndian);
        data.Signed!.Should().Be(false);
    }
}