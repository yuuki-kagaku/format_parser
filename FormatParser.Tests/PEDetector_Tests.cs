using FluentAssertions;
using FormatParser.Domain;
using FormatParser.Helpers.BinaryReader;
using FormatParser.PE;
using NUnit.Framework;

namespace FormatParser.Tests;

public class PEDetector_Tests : TestBase
{
    private PeDetector peDetector = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        peDetector = new PeDetector();
    }
    
    [Test]
    public async Task PeDetector_ShouldDetect_Amd64_Exe()
    {
        var fileInfo = await DetectAsync(GetFile(TestFileCategory.PeWindows, "procdump64.exe"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness64);
        fileInfo.Architecture.Should().Be(Architecture.Amd64);
        fileInfo.IsManaged.Should().Be(false);
    }
    
    [Test]
    public async Task PeDetector_ShouldDetect_i386_Exe()
    {
        var fileInfo = await DetectAsync(GetFile(TestFileCategory.PeWindows, "procdump.exe"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness32);
        fileInfo.Architecture.Should().Be(Architecture.I386);
        fileInfo.IsManaged.Should().Be(false);
    }
    
    [Test]
    public async Task PeDetector_ShouldDetect_Arm64_Exe()
    {
        var fileInfo = await DetectAsync(GetFile(TestFileCategory.PeWindows, "procexp64a.exe"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness64);
        fileInfo.Architecture.Should().Be(Architecture.Arm64);
        fileInfo.IsManaged.Should().Be(false);
    }
    
    [Test]
    public async Task PeDetector_ShouldDetect_Arm32_Exe()
    {
        var fileInfo = await DetectAsync(GetFile(TestFileCategory.PeWindows, "Notepad2_arm32.exe"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness32);
        fileInfo.Architecture.Should().Be(Architecture.Arm);
        fileInfo.IsManaged.Should().Be(false);
    }
    
    [Test]
    public async Task PeDetector_ShouldDetect_Managed_AnyCPU_Dll()
    {
        var fileInfo = await DetectAsync(GetFile(TestFileCategory.PeManaged, "HelloWorld.Core.AnyCpu.dll"));
        
        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness32);
        fileInfo.Architecture.Should().Be(Architecture.I386);
        fileInfo.IsManaged.Should().Be(true);
    }
    
    [Test]
    public async Task PeDetector_ShouldDetect_Managed_amd64_Dll()
    {
        var fileInfo = await DetectAsync(GetFile(TestFileCategory.PeManaged, "HelloWorld.Core.amd64.dll"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness64);
        fileInfo.Architecture.Should().Be(Architecture.Amd64);
        fileInfo.IsManaged.Should().Be(true);
    }
    
    [Test]
    public async Task PeDetector_ShouldDetect_Managed_arm64_Dll()
    {
        var fileInfo = await DetectAsync(GetFile(TestFileCategory.PeManaged, "HelloWorldCore.arm64.dll"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness64);
        fileInfo.Architecture.Should().Be(Architecture.Arm64);
        fileInfo.IsManaged.Should().Be(true);
    }
    
    [Test]
    public async Task PeDetector_ShouldDetect_Managed_x86_Dll()
    {
        var fileInfo = await DetectAsync(GetFile(TestFileCategory.PeManaged, "HelloWorld.Core.86.dll"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness32);
        fileInfo.Architecture.Should().Be(Architecture.I386);
        fileInfo.IsManaged.Should().Be(true);
    }
    
    private async Task<PeFileFormatInfo?> DetectAsync(string filename)
    {
        await using var stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
        var binaryReader = new StreamingBinaryReader(stream, Endianness.BigEndian);

        return await peDetector.TryDetectAsync(binaryReader) as PeFileFormatInfo;
    }
}