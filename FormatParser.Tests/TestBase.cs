using FormatParser.Tests.TestData;

namespace FormatParser.Tests;

public class TestBase
{
    protected string GetFile(TestFileCategory testFileCategory, string filename)
    {
        var subDirectory = testFileCategory switch
        {
            TestFileCategory.LinuxAmd64 => "linux_amd64",
            TestFileCategory.LinuxArm64 => "linux_aarch64",
            TestFileCategory.LinuxArmEl => "linux_armel",
            TestFileCategory.LinuxArmHf => "linux_armhf",
            TestFileCategory.LinuxI386 => "linux_i386",
            TestFileCategory.LinuxMips => "linux_mips",
            TestFileCategory.LinuxMips64El => "linux_mips64el",
            TestFileCategory.LinuxMipsEl => "linux_mipsel",
            TestFileCategory.LinuxPPC64El => "linux_ppc64el",
            TestFileCategory.LinuxS390X => "linux_s390x",
            TestFileCategory.Mac => "mac",
            TestFileCategory.MacFat => "mac_fat",
            TestFileCategory.PeWindows => "pe/windows",
            TestFileCategory.PeManaged => "pe/managed",
            TestFileCategory.Text => "text",
            TestFileCategory.PseudoText => "text_pseudo_utf16",
            TestFileCategory.TextUtf16 => "text_utf16",
            _ => throw new ArgumentOutOfRangeException(nameof(testFileCategory), testFileCategory, null)
        };
        
        return $"{TestDir}{Path.DirectorySeparatorChar}{subDirectory}{Path.DirectorySeparatorChar}{filename}";
    }

    protected string TestDir = "TestData";
}