using Geisha.Engine.Core.Math;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Math;

[TestFixture]
public class SizeDTests
{
    [TestCase(1, 2, 1, 2)]
    [TestCase(91.3376, 63.2359, 91, 63)]
    public void ToSize_Test(double width, double height, int expectedWidth, int expectedHeight)
    {
        // Arrange
        var sizeD = new SizeD(width, height);

        // Act
        var size = sizeD.ToSize();

        // Assert
        Assert.That(size.Width, Is.EqualTo(expectedWidth));
        Assert.That(size.Height, Is.EqualTo(expectedHeight));
    }
}