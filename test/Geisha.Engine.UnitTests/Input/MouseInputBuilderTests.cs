using Geisha.Engine.Input;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Input
{
    [TestFixture]
    public class MouseInputBuilderTests
    {
        [Test]
        public void Build_CreatesMouseInputWithStateSetAsSpecified()
        {
            // Arrange
            var builder = new MouseInputBuilder
            {
                Position = Utils.RandomVector2(),
                PositionDelta = Utils.RandomVector2(),
                LeftButton = Utils.Random.NextBool(),
                MiddleButton = Utils.Random.NextBool(),
                RightButton = Utils.Random.NextBool(),
                XButton1 = Utils.Random.NextBool(),
                XButton2 = Utils.Random.NextBool(),
                ScrollDelta = Utils.Random.Next()
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
}