using NUnit.Framework;

namespace FormatParser.Tests;

public class OneTimeSetUp
{
    [OneTimeSetUp]
    public void SetUp()
    {
        Environment.CurrentDirectory = Path.GetDirectoryName(typeof(OneTimeSetUp).Assembly.Location)!;
    }
}