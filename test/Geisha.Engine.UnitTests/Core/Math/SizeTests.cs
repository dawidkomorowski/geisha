using Geisha.Engine.Core.Math;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Math;

[TestFixture]
public class SizeTests
{
    [TestCase(1, 2)]
    public void ToSizeD_Test(int width, int height)
    {
        // Arrange
        var size = new Size(width, height);

        // Act
        var sizeD = size.ToSizeD();

        // Assert
        Assert.That(sizeD.Width, Is.EqualTo(width));
        Assert.That(sizeD.Height, Is.EqualTo(height));
    }
}