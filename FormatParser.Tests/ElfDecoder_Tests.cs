using FluentAssertions;
using FormatParser.ELF;
using NUnit.Framework;

namespace FormatParser.Tests;

public class ElfDecoder_Tests
{
    private ElfDecoder elfDecoder = null!;

    [SetUp]
    public void SetUp()
    {
        elfDecoder = new ElfDecoder();
    }
    
    [Test]
    public async Task ElfDecoder_ShouldParseAmd64LinuxExecutable()
    {
        var fileInfo = await DecodeAsync(@"./TestData/linux_amd64/vlc");

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness64);
        fileInfo.Architecture.Should().Be(Architecture.Amd64);
        fileInfo.Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Interpreter.Should().Be("/lib64/ld-linux-x86-64.so.2");
    }
    
    [Test]
    public async Task ElfDecoder_ShouldParse_armel()
    {
        var fileInfo = await DecodeAsync(@"./TestData/linux_armel/vlc-cache-gen");

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness32);
        fileInfo.Architecture.Should().Be(Architecture.Arm);
        fileInfo.Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Interpreter.Should().Be("/lib/ld-linux.so.3");
    }
    
    [Test]
    public async Task ElfDecoder_ShouldParse_armhf()
    {
        var fileInfo = await DecodeAsync(@"./TestData/linux_armhf/vlc-cache-gen");

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness32);
        fileInfo.Architecture.Should().Be(Architecture.Arm);
        fileInfo.Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Interpreter.Should().Be("/lib/ld-linux-armhf.so.3");
    }
    
    [Test]
    public async Task ElfDecoder_ShouldParse_s390x()
    {
        var fileInfo = await DecodeAsync(@"./TestData/linux_s390x/vlc-cache-gen");

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness64);
        fileInfo.Architecture.Should().Be(Architecture.s390);
        fileInfo.Endianness.Should().Be(Endianness.BigEndian);
        fileInfo.Interpreter.Should().Be("/lib/ld64.so.1");
    }
    
    [Test]
    public async Task ElfDecoder_ShouldParse_arm64()
    {
        var fileInfo = await DecodeAsync(@"./TestData/linux_aarch64/vlc-cache-gen");

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness64);
        fileInfo.Architecture.Should().Be(Architecture.Arm64);
        fileInfo.Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Interpreter.Should().Be("/lib/ld-linux-aarch64.so.1");
    }
    
    
    [Test]
    public async Task ElfDecoder_ShouldParse_i386()
    {
        var fileInfo = await DecodeAsync(@"./TestData/linux_i386/vlc-cache-gen");

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness32);
        fileInfo.Architecture.Should().Be(Architecture.i386);
        fileInfo.Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Interpreter.Should().Be("/lib/ld-linux.so.2");
    }
    
    [Test]
    public async Task ElfDecoder_ShouldParse_mips()
    {
        var fileInfo = await DecodeAsync(@"./TestData/linux_mips/vlc-cache-gen");

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness32);
        fileInfo.Architecture.Should().Be(Architecture.mips);
        fileInfo.Endianness.Should().Be(Endianness.BigEndian);
        fileInfo.Interpreter.Should().Be("/lib/ld.so.1");
    }
    
    [Test]
    public async Task ElfDecoder_ShouldParse_mipsel()
    {
        var fileInfo = await DecodeAsync(@"./TestData/linux_mipsel/vlc-cache-gen");

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness32);
        fileInfo.Architecture.Should().Be(Architecture.mips);
        fileInfo.Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Interpreter.Should().Be("/lib/ld.so.1");
    }
    
    [Test]
    public async Task ElfDecoder_ShouldParse_mips64el()
    {
        var fileInfo = await DecodeAsync(@"./TestData/linux_mips64el/vlc-cache-gen");

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness64);
        fileInfo.Architecture.Should().Be(Architecture.mips);
        fileInfo.Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Interpreter.Should().Be("/lib64/ld.so.1");
    }
    
    [Test]
    public async Task ElfDecoder_ShouldParse_ppc64el()
    {
        var fileInfo = await DecodeAsync(@"./TestData/linux_ppc64el/vlc-cache-gen");

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness64);
        fileInfo.Architecture.Should().Be(Architecture.ppc64);
        fileInfo.Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Interpreter.Should().Be("/lib64/ld64.so.2");
    }

    private async Task<ElfFileFormatInfo?> DecodeAsync(string filename)
    {
        await using var stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
        var binaryReader = new StreamingBinaryReader(stream);

        return await elfDecoder.TryDecodeAsync(binaryReader) as ElfFileFormatInfo;
    }
}