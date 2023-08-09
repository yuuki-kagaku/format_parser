using FormatParser.Helpers;
using NUnit.Framework;

namespace FormatParser.Tests;

[SetUpFixture]
public class OneTimeSetUp
{
    [OneTimeSetUp]
    public void SetUp()
    {
        Environment.CurrentDirectory = Path.GetDirectoryName(typeof(OneTimeSetUp).Assembly.Location)!;
        PluginLoadHelper.LoadPlugins();
    }
}