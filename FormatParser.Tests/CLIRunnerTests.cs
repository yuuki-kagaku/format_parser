using FluentAssertions;
using FormatParser.Domain;
using FormatParser.Test.Helpers;
using FormatParser.Text;
using NUnit.Framework;

namespace FormatParser.Tests;

public class CLIRunnerTests : TestBase
{
    [Test]
    public void Should_run_cli_utility()
    {
        var output = ShellRunner.RunCommand("dotnet", $"FormatParser.CLI.dll {TestDir}{Path.DirectorySeparatorChar}for_cli");

        var expected = $"[     3] : Unknown{Environment.NewLine}[     2] : {new TextFileFormatInfo(TextFileFormatInfo.DefaultTextType, WellKnownEncodingInfos.Utf16LeNoBom).ToPrettyString()}{Environment.NewLine}[     1] : {new TextFileFormatInfo(TextFileFormatInfo.DefaultTextType, WellKnownEncodingInfos.Utf16BeNoBom).ToPrettyString()}";
        output.Should().Contain(expected);
    }
}