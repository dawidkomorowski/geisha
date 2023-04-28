using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Backend;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering
{
    [TestFixture]
    public class DebugRendererTests
    {
        private IRenderingContext2D _renderingContext2D = null!;
        private DebugRenderer _debugRenderer = null!;

        [SetUp]
        public void SetUp()
        {
            _renderingContext2D = Substitute.For<IRenderingContext2D>();
            _debugRenderer = new DebugRenderer();
        }

        [Test]
        public void DrawDebugInformation_ShouldDrawCircle()
        {
            // Arrange
            var circle = new Circle(new Vector2(1, 2), 3);
            var color = Color.FromArgb(1, 2, 3, 4);
            _debugRenderer.DrawCircle(circle, color);

            var cameraTransformationMatrix = new Matrix3x3(1, 2, 3, 4, 5, 6, 7, 8, 9);

            // Act
            _debugRenderer.DrawDebugInformation(_renderingContext2D, cameraTransformationMatrix);

            // Assert
            _renderingContext2D.Received(1).DrawEllipse(circle.ToEllipse(), color, false, cameraTransformationMatrix);
        }

        [Test]
        public void DrawDebugInformation_ShouldClear_DrawCircle_DebugInformationToDraw()
        {
            // Arrange
            var circle = new Circle(new Vector2(1, 2), 3);
            var color = Color.FromArgb(1, 2, 3, 4);
            _debugRenderer.DrawCircle(circle, color);
            _debugRenderer.DrawDebugInformation(_renderingContext2D, Matrix3x3.Identity);

            _renderingContext2D.ClearReceivedCalls();

            // Act
            _debugRenderer.DrawDebugInformation(_renderingContext2D, Matrix3x3.Identity);

            // Assert
            _renderingContext2D.DidNotReceiveWithAnyArgs().DrawEllipse(Arg.Any<Ellipse>(), Arg.Any<Color>(), Arg.Any<bool>(), Arg.Any<Matrix3x3>());
        }

        [Test]
        public void DrawDebugInformation_ShouldDrawRectangle()
        {
            // Arrange
            var rectangle = new AxisAlignedRectangle(new Vector2(1, 2), new Vector2(10, 20));
            var color = Color.FromArgb(1, 2, 3, 4);
            var transform = new Matrix3x3(1, 2, 3, 4, 5, 6, 7, 8, 9);
            _debugRenderer.DrawRectangle(rectangle, color, transform);

            var cameraTransformationMatrix = new Matrix3x3(1, 2, 3, 4, 5, 6, 7, 8, 9);

            // Act
            _debugRenderer.DrawDebugInformation(_renderingContext2D, cameraTransformationMatrix);

            // Assert
            var expectedTransform = new Matrix3x3(30, 36, 42, 66, 81, 96, 102, 126, 150);
            _renderingContext2D.Received(1).DrawRectangle(rectangle, color, false, expectedTransform);
        }

        [Test]
        public void DrawDebugInformation_ShouldClear_DrawRectangle_DebugInformationToDraw()
        {
            // Arrange
            var rectangle = new AxisAlignedRectangle(new Vector2(1, 2), new Vector2(10, 20));
            var color = Color.FromArgb(1, 2, 3, 4);
            var transform = new Matrix3x3(1, 2, 3, 4, 5, 6, 7, 8, 9);
            _debugRenderer.DrawRectangle(rectangle, color, transform);
            _debugRenderer.DrawDebugInformation(_renderingContext2D, Matrix3x3.Identity);

            _renderingContext2D.ClearReceivedCalls();

            // Act
            _debugRenderer.DrawDebugInformation(_renderingContext2D, Matrix3x3.Identity);

            // Assert
            _renderingContext2D.DidNotReceiveWithAnyArgs().DrawRectangle(Arg.Any<AxisAlignedRectangle>(), Arg.Any<Color>(), Arg.Any<bool>(), Arg.Any<Matrix3x3>());
        }
    }
}