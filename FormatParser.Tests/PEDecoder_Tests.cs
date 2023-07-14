using FluentAssertions;
using FormatParser.PE;
using NUnit.Framework;

namespace FormatParser.Tests;

public class PEDecoder_Tests
{
    private PEDecoder peDecoder = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        peDecoder = new PEDecoder();
    }
    
    [Test]
    public async Task PEParser_ShouldParse_Amd64Exe()
    {
        var fileInfo = await DecodeAsync(@"./TestData/pe/windows/procdump64.exe");

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness64);
        fileInfo!.Architecture.Should().Be(Architecture.Amd64);
        fileInfo.IsManaged!.Should().Be(false);
    }
    
    [Test]
    public async Task PEParser_ShouldParse_i386Exe()
    {
        var fileInfo = await DecodeAsync(@"./TestData/pe/windows/procdump.exe");

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness32);
        fileInfo!.Architecture.Should().Be(Architecture.i386);
        fileInfo.IsManaged!.Should().Be(false);
    }
    
    [Test]
    public async Task PEParser_ShouldParse_ArmExe()
    {
        var fileInfo = await DecodeAsync(@"./TestData/pe/windows/procexp64a.exe");

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness64);
        fileInfo!.Architecture.Should().Be(Architecture.Arm64);
        fileInfo.IsManaged!.Should().Be(false);
    }
    
    [Test]
    public async Task PEParser_ShouldParse_Managed_AnyCPUDll()
    {
        var fileInfo = await DecodeAsync(@"./TestData/pe/managed/HelloWorld.Core.AnyCpu.dll");
        
        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness32);
        fileInfo!.Architecture.Should().Be(Architecture.i386);
        fileInfo.IsManaged!.Should().Be(true);
    }
    
    [Test]
    public async Task PEParser_ShouldParse_Managed_amd64_Dll()
    {
        var fileInfo = await DecodeAsync(@"./TestData/pe/managed/HelloWorld.Core.amd64.dll");

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness64);
        fileInfo!.Architecture.Should().Be(Architecture.Amd64);
        fileInfo.IsManaged!.Should().Be(true);
    }
    
    [Test]
    public async Task PEParser_ShouldParse_Managed_arm64_Dll()
    {
        var fileInfo = await DecodeAsync(@"./TestData/pe/managed/HelloWorldCore.arm64.dll");

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness64);
        fileInfo!.Architecture.Should().Be(Architecture.Arm64);
        fileInfo.IsManaged!.Should().Be(true);
    }
    
    [Test]
    public async Task PEParser_ShouldParse_Managed_x86_Dll()
    {
        var fileInfo = await DecodeAsync(@"./TestData/pe/managed/HelloWorld.Core.86.dll");

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness32);
        fileInfo!.Architecture.Should().Be(Architecture.i386);
        fileInfo.IsManaged!.Should().Be(true);
    }
    
    private async Task<PeFileFormatInfo?> DecodeAsync(string filename)
    {
        await using var stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
        var binaryReader = new StreamingBinaryReader(stream);

        return await peDecoder.TryDecodeAsync(binaryReader) as PeFileFormatInfo;
    }
}