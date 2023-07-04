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
    
    [Test]
    public async Task MachOParser_ShouldParseFatMacExecutable()
    {
        var macParser = new MachOParser();
        
        var stream = new FileStream(@"/home/q/Downloads/testo/MacFamilyTree 10", FileMode.Open);
        var data = (await macParser.DeserializeAsync(stream)) as FatMachOData;

        data.Should().NotBeNull();
        data!.Bitness.Should().Be(Bitness.Bitness32);
        // data!.Architecture.Should().Be(Architecture.Amd64);
        data!.Endianess.Should().Be(Endianess.BigEndian);
        data!.Datas.Length.Should().Be(2);
        // data.Signed!.Should().Be(false);
    }
}