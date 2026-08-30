using Geisha.Engine.Core.Math;
using Geisha.Engine.Input;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Input;

[TestFixture]
public class MouseInputBuilderTests
{
    [TestCase(true, false, true, false, true)]
    [TestCase(false, true, false, true, false)]
    public void Build_CreatesMouseInputWithStateSetAsSpecified(bool leftButton, bool middleButton, bool rightButton, bool xButton1, bool xButton2)
    {
        // Arrange
        var builder = new MouseInputBuilder
        {
            Position = new Vector2(12, 34),
            PositionDelta = new Vector2(5, 6),
            LeftButton = leftButton,
            MiddleButton = middleButton,
            RightButton = rightButton,
            XButton1 = xButton1,
            XButton2 = xButton2,
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