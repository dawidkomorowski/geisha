using Geisha.Engine.Rendering;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering;

[TestFixture]
public class FontSizeTests
{
    [TestCase(72.0d, 96.0d)]
    [TestCase(123.456d, 164.608d)]
    public void FromPoints_ShouldReturnInstanceWithCorrectPointsAndDips(double expectedPoints, double expectedDips)
    {
        // Arrange
        // Act
        var actual = FontSize.FromPoints(expectedPoints);

        // Assert
        Assert.That(actual.Points, Is.EqualTo(expectedPoints));
        Assert.That(actual.Dips, Is.EqualTo(expectedDips));
    }

    [TestCase(96.0d, 72.0d)]
    [TestCase(164.608d, 123.456d)]
    public void FromDips_ShouldReturnInstanceWithCorrectPointsAndDips(double expectedDips, double expectedPoints)
    {
        // Arrange
        // Act
        var actual = FontSize.FromDips(expectedDips);

        // Assert
        Assert.That(actual.Points, Is.EqualTo(expectedPoints));
        Assert.That(actual.Dips, Is.EqualTo(expectedDips));
    }
}