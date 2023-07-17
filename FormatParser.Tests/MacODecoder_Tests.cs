using FluentAssertions;
using FormatParser.MachO;
using FormatParser.Tests.TestData;
using NUnit.Framework;

namespace FormatParser.Tests;

public class MacODecoder_Tests : TestBase
{
    private MachODetector machODetector = null!;

    [SetUp]
    public void SetUp()
    {
        machODetector = new MachODetector();
    }
    
    [Test]
    public async Task MachDecoder_ShouldParseMacExecutable()
    {
        var fileInfo = await DecodeAsync(GetFile(TestFileCategory.Mac, "VLC")) as MachOFileFormatInfo;

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness64);
        fileInfo!.Architecture.Should().Be(Architecture.Amd64);
        fileInfo!.Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Signed!.Should().Be(true);
    }
    
    [Test]
    public async Task MachDecoder_ShouldParse_fat_file()
    {
        var fileInfo = await DecodeAsync(GetFile(TestFileCategory.MacFat, "DOSBox")) as FatMachOFileFormatInfo;;

        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness32);
        fileInfo!.Endianness.Should().Be(Endianness.BigEndian);
        fileInfo.Datas.Length.Should().Be(3);
        
        fileInfo.Datas[0].Bitness.Should().Be(Bitness.Bitness64);
        fileInfo.Datas[0].Architecture.Should().Be(Architecture.Amd64);
        fileInfo.Datas[0].Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Datas[0].Signed!.Should().Be(true);

        fileInfo.Datas[1].Bitness.Should().Be(Bitness.Bitness32);
        fileInfo.Datas[1].Architecture.Should().Be(Architecture.i386);
        fileInfo.Datas[1].Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Datas[1].Signed!.Should().Be(true);
        
        fileInfo.Datas[2].Bitness.Should().Be(Bitness.Bitness32);
        fileInfo.Datas[2].Architecture.Should().Be(Architecture.PowerPC);
        fileInfo.Datas[2].Endianness.Should().Be(Endianness.BigEndian);
        fileInfo.Datas[2].Signed!.Should().Be(true);
    }
    
    [Test]
    public async Task MachDecoder_ShouldParse_signed_amd64_file()
    {
        var fileInfo = await DecodeAsync(GetFile(TestFileCategory.Mac, "Read_Before_You_Install_iTunes")) as MachOFileFormatInfo;
    
        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness64);
        fileInfo!.Architecture.Should().Be(Architecture.Amd64);
        fileInfo!.Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Signed!.Should().Be(true);
    }
    
    [Test]
    public async Task MachDecoder_ShouldParse_signed_arm_file()
    {
        var fileInfo = await DecodeAsync(GetFile(TestFileCategory.Mac, "MachO-iOS-armv7s-Helloworld")) as MachOFileFormatInfo;
    
        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness32);
        fileInfo!.Architecture.Should().Be(Architecture.Arm);
        fileInfo!.Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Signed!.Should().Be(true);
    }
    
    [Test]
    public async Task MachDecoder_ShouldParse_unsigned_file()
    {
        var fileInfo = await DecodeAsync(GetFile(TestFileCategory.Mac, "hello_world")) as MachOFileFormatInfo;
    
        fileInfo.Should().NotBeNull();
        fileInfo!.Bitness.Should().Be(Bitness.Bitness32);
        fileInfo!.Architecture.Should().Be(Architecture.i386);
        fileInfo!.Endianness.Should().Be(Endianness.LittleEndian);
        fileInfo.Signed!.Should().Be(false);
    }

    
    private async Task<IFileFormatInfo?> DecodeAsync(string filename)
    {
        await using var stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
        var binaryReader = new StreamingBinaryReader(stream);

        return await machODetector.TryDetectAsync(binaryReader);
    }
}