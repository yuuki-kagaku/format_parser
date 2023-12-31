using System.Text;
using FormatParser.Helpers;

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
            TestFileCategory.PeWindows => $"pe{Path.DirectorySeparatorChar}windows",
            TestFileCategory.PeManaged => $"pe{Path.DirectorySeparatorChar}managed",
            TestFileCategory.Text => "text",
            TestFileCategory.TextUtf16 => "text_utf16",
            TestFileCategory.TextUtf32 => "text_utf32",
            TestFileCategory.TextUtf8 => "text_utf8",
            TestFileCategory.Xml => "xml",
            TestFileCategory.Ebcdic => "text_ebcdic",
            TestFileCategory.PseudoText => "text_pseudo_utf16",
            _ => throw new ArgumentOutOfRangeException(nameof(testFileCategory), testFileCategory, null)
        };
        
        return $"{TestDir}{Path.DirectorySeparatorChar}{subDirectory}{Path.DirectorySeparatorChar}{filename}";
    }

    protected string TestDir = "TestData";

    protected static string BuildString(ArraySegment<char> chars) => new StringBuilder().Append(chars.ToMemory()).ToString();
    
    protected static string ReadFileAsUtf8(string file) => File.ReadAllText(file);

    protected static string ReadFileAsUtf16Le(string file) => Encoding.Unicode.GetString(File.ReadAllBytes(file));

    protected static string ReadFileAsUtf16Be(string file) => Encoding.BigEndianUnicode.GetString(File.ReadAllBytes(file));

    protected static string ReadFileAsUtf32Be(string file) => File.ReadAllText(file, new UTF32Encoding(true, true));

    protected static string ReadFileAsUtf32Le(string file) => File.ReadAllText(file, new UTF32Encoding(false, true));

    protected static string ReadFileAsWindows1251(string file) => File.ReadAllText(file, Encoding.GetEncoding("windows-1251"));
}