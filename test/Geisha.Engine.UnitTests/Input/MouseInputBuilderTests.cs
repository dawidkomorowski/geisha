using Geisha.Engine.Core.Math;
using Geisha.Engine.Input;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Input;

[TestFixture]
public class MouseInputBuilderTests
{
    [Test]
    public void Build_CreatesMouseInputWithStateSetAsSpecified()
    {
        // Arrange
        var builder = new MouseInputBuilder
        {
            Position = new Vector2(12, 34),
            PositionDelta = new Vector2(5, 6),
            LeftButton = true,
            MiddleButton = true,
            RightButton = true,
            XButton1 = true,
            XButton2 = true,
            ScrollDelta = 120
        };

        // Act
        var mouseInput = builder.Build();

        // Assert
        Assert.That(mouseInput.Position, Is.EqualTo(builder.Position));
        Assert.That(mouseInput.PositionDelta, Is.EqualTo(builder.PositionDelta));
        Assert.That(mouseInput.LeftButton, Is.EqualTo(builder.LeftButton));
        Assert.That(mouseInput.MiddleButton, Is.EqualTo(builder.MiddleButton));
        Assert.That(mouseInput.RightButton, Is.EqualTo(builder.RightButton));
        Assert.That(mouseInput.XButton1, Is.EqualTo(builder.XButton1));
        Assert.That(mouseInput.XButton2, Is.EqualTo(builder.XButton2));
        Assert.That(mouseInput.ScrollDelta, Is.EqualTo(builder.ScrollDelta));
    }
}