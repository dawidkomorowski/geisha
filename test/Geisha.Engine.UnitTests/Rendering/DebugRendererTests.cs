using Geisha.Common.Math;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Backend;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering
{
    [TestFixture]
    public class DebugRendererTests
    {
        private IRenderer2D _renderer2D = null!;
        private DebugRenderer _debugRenderer = null!;

        [SetUp]
        public void SetUp()
        {
            _renderer2D = Substitute.For<IRenderer2D>();
            _debugRenderer = new DebugRenderer();
        }

        [Test]
        public void DrawDebugInformation_ShouldDrawCircle()
        {
            // Arrange
            var circle = new Circle(new Vector2(1, 2), 3);
            var color = Color.FromArgb(255, 255, 0, 0);
            _debugRenderer.DrawCircle(circle, color);

            // Act
            _debugRenderer.DrawDebugInformation(_renderer2D, Matrix3x3.Identity);

            // Assert
            _renderer2D.Received(1).RenderEllipse(circle.ToEllipse(), color, false, Matrix3x3.Identity);
        }

        [Test]
        public void DrawDebugInformation_ShouldClearDebugInformationToDraw()
        {
            // Arrange
            var circle = new Circle(new Vector2(1, 2), 3);
            var color = Color.FromArgb(255, 255, 0, 0);
            _debugRenderer.DrawCircle(circle, color);
            _debugRenderer.DrawDebugInformation(_renderer2D, Matrix3x3.Identity);

            _renderer2D.ClearReceivedCalls();

            // Act
            _debugRenderer.DrawDebugInformation(_renderer2D, Matrix3x3.Identity);

            // Assert
            _renderer2D.DidNotReceiveWithAnyArgs().RenderEllipse(Arg.Any<Ellipse>(), Arg.Any<Color>(), Arg.Any<bool>(), Arg.Any<Matrix3x3>());
        }
    }
}