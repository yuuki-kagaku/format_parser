using FluentAssertions;
using FormatParser.BinaryReader;
using FormatParser.PE;
using FormatParser.Tests.TestData;
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
    public async Task PEParser_ShouldParse_Amd64Exe()
    {
        var fileInfo = await DecodeAsync(GetFile(TestFileCategory.PeWindows, "procdump64.exe"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness64);
        fileInfo!.Architecture.Should().Be(Architecture.Amd64);
        fileInfo.IsManaged!.Should().Be(false);
    }
    
    [Test]
    public async Task PEParser_ShouldParse_i386Exe()
    {
        var fileInfo = await DecodeAsync(GetFile(TestFileCategory.PeWindows, "procdump.exe"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness32);
        fileInfo!.Architecture.Should().Be(Architecture.i386);
        fileInfo.IsManaged!.Should().Be(false);
    }
    
    [Test]
    public async Task PEParser_ShouldParse_ArmExe()
    {
        var fileInfo = await DecodeAsync(GetFile(TestFileCategory.PeWindows, "procexp64a.exe"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness64);
        fileInfo!.Architecture.Should().Be(Architecture.Arm64);
        fileInfo.IsManaged!.Should().Be(false);
    }
    
    [Test]
    public async Task PEParser_ShouldParse_Managed_AnyCPUDll()
    {
        var fileInfo = await DecodeAsync(GetFile(TestFileCategory.PeManaged, "HelloWorld.Core.AnyCpu.dll"));
        
        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness32);
        fileInfo!.Architecture.Should().Be(Architecture.i386);
        fileInfo.IsManaged!.Should().Be(true);
    }
    
    [Test]
    public async Task PEParser_ShouldParse_Managed_amd64_Dll()
    {
        var fileInfo = await DecodeAsync(GetFile(TestFileCategory.PeManaged, "HelloWorld.Core.amd64.dll"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness64);
        fileInfo!.Architecture.Should().Be(Architecture.Amd64);
        fileInfo.IsManaged!.Should().Be(true);
    }
    
    [Test]
    public async Task PEParser_ShouldParse_Managed_arm64_Dll()
    {
        var fileInfo = await DecodeAsync(GetFile(TestFileCategory.PeManaged, "HelloWorldCore.arm64.dll"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness64);
        fileInfo!.Architecture.Should().Be(Architecture.Arm64);
        fileInfo.IsManaged!.Should().Be(true);
    }
    
    [Test]
    public async Task PEParser_ShouldParse_Managed_x86_Dll()
    {
        var fileInfo = await DecodeAsync(GetFile(TestFileCategory.PeManaged, "HelloWorld.Core.86.dll"));

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness32);
        fileInfo!.Architecture.Should().Be(Architecture.i386);
        fileInfo.IsManaged!.Should().Be(true);
    }
    
    private async Task<PeFileFormatInfo?> DecodeAsync(string filename)
    {
        await using var stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
        var binaryReader = new StreamingBinaryReader(stream);

        return await peDetector.TryDetectAsync(binaryReader) as PeFileFormatInfo;
    }
}