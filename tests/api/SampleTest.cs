using NUnit.Framework;

namespace miniblocks.API.Tests;

[TestFixture]
public class SampleTest
{
    private string sampleString;

    [SetUp]
    public void Setup()
    {
        sampleString = "Hello, world!";
    }

    [Test]
    public void SampleString_Correct()
        => Assert.AreEqual("Hello, world!", sampleString);
}
