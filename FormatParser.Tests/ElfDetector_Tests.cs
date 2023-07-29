﻿using FluentAssertions;
using FormatParser.Domain;
using FormatParser.ELF;
using FormatParser.Helpers.BinaryReader;
using NUnit.Framework;

namespace FormatParser.Tests;

public class ElfDetector_Tests : TestBase
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
        var fileInfo = await DetectAsync(GetFile(TestFileCategory.LinuxAmd64, "vlc"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness64);
        fileInfo.Architecture.Should().Be(Architecture.Amd64);
        fileInfo.Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Interpreter.Should().Be("/lib64/ld-linux-x86-64.so.2");
    }
    
    [Test]
    public async Task ElfDetector_ShouldParse_armel()
    {
        var fileInfo = await DetectAsync(GetFile(TestFileCategory.LinuxArmEl, "vlc-cache-gen"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness32);
        fileInfo.Architecture.Should().Be(Architecture.Arm);
        fileInfo.Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Interpreter.Should().Be("/lib/ld-linux.so.3");
    }
    
    [Test]
    public async Task ElfDetector_ShouldParse_armhf()
    {
        var fileInfo = await DetectAsync(GetFile(TestFileCategory.LinuxArmHf, "vlc-cache-gen"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness32);
        fileInfo.Architecture.Should().Be(Architecture.Arm);
        fileInfo.Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Interpreter.Should().Be("/lib/ld-linux-armhf.so.3");
    }
    
    [Test]
    public async Task ElfDetector_ShouldParse_s390x()
    {
        var fileInfo = await DetectAsync(GetFile(TestFileCategory.LinuxS390X, "vlc-cache-gen"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness64);
        fileInfo.Architecture.Should().Be(Architecture.S390);
        fileInfo.Endianness.Should().Be(Endianness.BigEndian);
        fileInfo.Interpreter.Should().Be("/lib/ld64.so.1");
    }
    
    [Test]
    public async Task ElfDetector_ShouldParse_arm64()
    {
        var fileInfo = await DetectAsync(GetFile(TestFileCategory.LinuxArm64, "vlc-cache-gen"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness64);
        fileInfo.Architecture.Should().Be(Architecture.Arm64);
        fileInfo.Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Interpreter.Should().Be("/lib/ld-linux-aarch64.so.1");
    }
    
    
    [Test]
    public async Task ElfDetector_ShouldParse_i386()
    {
        var fileInfo = await DetectAsync(GetFile(TestFileCategory.LinuxI386, "vlc-cache-gen"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness32);
        fileInfo.Architecture.Should().Be(Architecture.I386);
        fileInfo.Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Interpreter.Should().Be("/lib/ld-linux.so.2");
    }
    
    [Test]
    public async Task ElfDetector_ShouldParse_mips()
    {
        var fileInfo = await DetectAsync(GetFile(TestFileCategory.LinuxMips, "vlc-cache-gen"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness32);
        fileInfo.Architecture.Should().Be(Architecture.Mips);
        fileInfo.Endianness.Should().Be(Endianness.BigEndian);
        fileInfo.Interpreter.Should().Be("/lib/ld.so.1");
    }
    
    [Test]
    public async Task ElfDetector_ShouldParse_mipsel()
    {
        var fileInfo = await DetectAsync(GetFile(TestFileCategory.LinuxMipsEl, "vlc-cache-gen"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness32);
        fileInfo.Architecture.Should().Be(Architecture.Mips);
        fileInfo.Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Interpreter.Should().Be("/lib/ld.so.1");
    }
    
    [Test]
    public async Task ElfDetector_ShouldParse_mips64el()
    {
        var fileInfo = await DetectAsync(GetFile(TestFileCategory.LinuxMips64El, "vlc-cache-gen"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness64);
        fileInfo.Architecture.Should().Be(Architecture.Mips);
        fileInfo.Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Interpreter.Should().Be("/lib64/ld.so.1");
    }
    
    [Test]
    public async Task ElfDetector_ShouldParse_ppc64el()
    {
        var fileInfo = await DetectAsync(GetFile(TestFileCategory.LinuxPPC64El, "vlc-cache-gen"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness64);
        fileInfo.Architecture.Should().Be(Architecture.Ppc64);
        fileInfo.Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Interpreter.Should().Be("/lib64/ld64.so.2");
    }

    private async Task<ElfFileFormatInfo?> DetectAsync(string filename)
    {
        await using var stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
        var binaryReader = new StreamingBinaryReader(stream, Endianness.BigEndian);

        return await elfDetector.TryDetectAsync(binaryReader) as ElfFileFormatInfo;
    }
}