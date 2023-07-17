using FluentAssertions;
using FormatParser.ELF;
using FormatParser.Tests.TestData;
using NUnit.Framework;

namespace FormatParser.Tests;

public class ElfDecoder_Tests : TestBase
{
    private ElfDetector elfDetector = null!;

    [SetUp]
    public void SetUp()
    {
        elfDetector = new ElfDetector();
    }
    
    [Test]
    public async Task ElfDecoder_ShouldParseAmd64LinuxExecutable()
    {
        var fileInfo = await DecodeAsync(GetFile(TestFileCategory.LinuxAmd64, "vlc"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness64);
        fileInfo.Architecture.Should().Be(Architecture.Amd64);
        fileInfo.Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Interpreter.Should().Be("/lib64/ld-linux-x86-64.so.2");
    }
    
    [Test]
    public async Task ElfDecoder_ShouldParse_armel()
    {
        var fileInfo = await DecodeAsync(GetFile(TestFileCategory.LinuxArmEl, "vlc-cache-gen"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness32);
        fileInfo.Architecture.Should().Be(Architecture.Arm);
        fileInfo.Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Interpreter.Should().Be("/lib/ld-linux.so.3");
    }
    
    [Test]
    public async Task ElfDecoder_ShouldParse_armhf()
    {
        var fileInfo = await DecodeAsync(GetFile(TestFileCategory.LinuxArmHf, "vlc-cache-gen"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness32);
        fileInfo.Architecture.Should().Be(Architecture.Arm);
        fileInfo.Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Interpreter.Should().Be("/lib/ld-linux-armhf.so.3");
    }
    
    [Test]
    public async Task ElfDecoder_ShouldParse_s390x()
    {
        var fileInfo = await DecodeAsync(GetFile(TestFileCategory.LinuxS390X, "vlc-cache-gen"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness64);
        fileInfo.Architecture.Should().Be(Architecture.s390);
        fileInfo.Endianness.Should().Be(Endianness.BigEndian);
        fileInfo.Interpreter.Should().Be("/lib/ld64.so.1");
    }
    
    [Test]
    public async Task ElfDecoder_ShouldParse_arm64()
    {
        var fileInfo = await DecodeAsync(GetFile(TestFileCategory.LinuxArm64, "vlc-cache-gen"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness64);
        fileInfo.Architecture.Should().Be(Architecture.Arm64);
        fileInfo.Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Interpreter.Should().Be("/lib/ld-linux-aarch64.so.1");
    }
    
    
    [Test]
    public async Task ElfDecoder_ShouldParse_i386()
    {
        var fileInfo = await DecodeAsync(GetFile(TestFileCategory.LinuxI386, "vlc-cache-gen"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness32);
        fileInfo.Architecture.Should().Be(Architecture.i386);
        fileInfo.Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Interpreter.Should().Be("/lib/ld-linux.so.2");
    }
    
    [Test]
    public async Task ElfDecoder_ShouldParse_mips()
    {
        var fileInfo = await DecodeAsync(GetFile(TestFileCategory.LinuxMips, "vlc-cache-gen"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness32);
        fileInfo.Architecture.Should().Be(Architecture.mips);
        fileInfo.Endianness.Should().Be(Endianness.BigEndian);
        fileInfo.Interpreter.Should().Be("/lib/ld.so.1");
    }
    
    [Test]
    public async Task ElfDecoder_ShouldParse_mipsel()
    {
        var fileInfo = await DecodeAsync(GetFile(TestFileCategory.LinuxMipsEl, "vlc-cache-gen"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness32);
        fileInfo.Architecture.Should().Be(Architecture.mips);
        fileInfo.Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Interpreter.Should().Be("/lib/ld.so.1");
    }
    
    [Test]
    public async Task ElfDecoder_ShouldParse_mips64el()
    {
        var fileInfo = await DecodeAsync(GetFile(TestFileCategory.LinuxMips64El, "vlc-cache-gen"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness64);
        fileInfo.Architecture.Should().Be(Architecture.mips);
        fileInfo.Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Interpreter.Should().Be("/lib64/ld.so.1");
    }
    
    [Test]
    public async Task ElfDecoder_ShouldParse_ppc64el()
    {
        var fileInfo = await DecodeAsync(GetFile(TestFileCategory.LinuxPPC64El, "vlc-cache-gen"));

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

        return await elfDetector.TryDetectAsync(binaryReader) as ElfFileFormatInfo;
    }
}