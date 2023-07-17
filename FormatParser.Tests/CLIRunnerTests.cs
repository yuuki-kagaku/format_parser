using FluentAssertions;
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

        var expected = @$"[     3] : Unknown{Environment.NewLine}[     2] : text/plain ; {EncodingData.UTF16LeNoBom}{Environment.NewLine}[     1] : text/plain ; {EncodingData.UTF16BeNoBom}";
        output.Should().Contain(expected);
    }
}