using FluentAssertions;
using FormatParser.PE;
using NUnit.Framework;

namespace FormatParser.Tests;

public class PEParser_Tests
{
    private PEParser peParser = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        peParser = new PEParser();
    }
    
    [Test]
    public async Task PEParser_ShouldParse_Amd64Exe()
    {
        var stream = new FileStream(@"./TestData/pe/windows/procdump64.exe", FileMode.Open);
        var data = (await peParser.DeserializeAsync(stream)) as PEData;

        data.Should().NotBeNull();
        data!.Bitness.Should().Be(Bitness.Bitness64);
        data!.Architecture.Should().Be(Architecture.Amd64);
        data.IsManaged!.Should().Be(false);
    }
    
    [Test]
    public async Task PEParser_ShouldParse_i386Exe()
    {
        var stream = new FileStream(@"./TestData/pe/windows/procdump.exe", FileMode.Open);
        var data = (await peParser.DeserializeAsync(stream)) as PEData;

        data.Should().NotBeNull();
        data!.Bitness.Should().Be(Bitness.Bitness32);
        data!.Architecture.Should().Be(Architecture.i386);
        data.IsManaged!.Should().Be(false);
    }
    
    [Test]
    public async Task PEParser_ShouldParse_ArmExe()
    {
        var stream = new FileStream(@"./TestData/pe/windows/procexp64a.exe", FileMode.Open);
        var data = (await peParser.DeserializeAsync(stream)) as PEData;

        data.Should().NotBeNull();
        data!.Bitness.Should().Be(Bitness.Bitness64);
        data!.Architecture.Should().Be(Architecture.Arm64);
        data.IsManaged!.Should().Be(false);
    }
    
    [Test]
    public async Task PEParser_ShouldParse_Managed_AnyCPUDll()
    {
        var stream = new FileStream(@"./TestData/pe/managed/HelloWorld.Core.AnyCpu.dll", FileMode.Open);
        var data = (await peParser.DeserializeAsync(stream)) as PEData;

        data.Should().NotBeNull();
        data!.Bitness.Should().Be(Bitness.Bitness32);
        data!.Architecture.Should().Be(Architecture.i386);
        data.IsManaged!.Should().Be(true);
    }
    
    [Test]
    public async Task PEParser_ShouldParse_Managed_amd64_Dll()
    {
        var stream = new FileStream(@"./TestData/pe/managed/HelloWorld.Core.amd64.dll", FileMode.Open);
        var data = (await peParser.DeserializeAsync(stream)) as PEData;

        data.Should().NotBeNull();
        data!.Bitness.Should().Be(Bitness.Bitness64);
        data!.Architecture.Should().Be(Architecture.Amd64);
        data.IsManaged!.Should().Be(true);
    }
    
    [Test]
    public async Task PEParser_ShouldParse_Managed_arm64_Dll()
    {
        var stream = new FileStream(@"./TestData/pe/managed/HelloWorldCore.arm64.dll", FileMode.Open);
        var data = (await peParser.DeserializeAsync(stream)) as PEData;

        data.Should().NotBeNull();
        data!.Bitness.Should().Be(Bitness.Bitness64);
        data!.Architecture.Should().Be(Architecture.Arm64);
        data.IsManaged!.Should().Be(true);
    }
    
    [Test]
    public async Task PEParser_ShouldParse_Managed_x86_Dll()
    {
        var stream = new FileStream(@"./TestData/pe/managed/HelloWorld.Core.86.dll", FileMode.Open);
        var data = (await peParser.DeserializeAsync(stream)) as PEData;

        data.Should().NotBeNull();
        data!.Bitness.Should().Be(Bitness.Bitness32);
        data!.Architecture.Should().Be(Architecture.i386);
        data.IsManaged!.Should().Be(true);
    }
    
}