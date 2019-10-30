using Geisha.Common.Math;
using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Engine.Physics.Components;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.Model.Components
{
    [TestFixture]
    public class RectangleColliderComponentModelTests
    {
        private RectangleColliderComponent _rectangleColliderComponent;
        private RectangleColliderComponentModel _rectangleColliderComponentModel;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            _rectangleColliderComponent = new RectangleColliderComponent {Dimension = new Vector2(1, 2)};
            _rectangleColliderComponentModel = new RectangleColliderComponentModel(_rectangleColliderComponent);
        }

        [Test]
        public void DimensionX_ShouldUpdateRectangleColliderComponentDimension()
        {
            // Assume
            Assume.That(_rectangleColliderComponentModel.DimensionX, Is.EqualTo(1));

            // Act
            _rectangleColliderComponentModel.DimensionX = 123;

            // Assert
            Assert.That(_rectangleColliderComponentModel.DimensionX, Is.EqualTo(123));
            Assert.That(_rectangleColliderComponent.Dimension.X, Is.EqualTo(123));
        }

        [Test]
        public void DimensionY_ShouldUpdateRectangleColliderComponentDimension()
        {
            // Assume
            Assume.That(_rectangleColliderComponentModel.DimensionY, Is.EqualTo(2));

            // Act
            _rectangleColliderComponentModel.DimensionY = 123;

            // Assert
            Assert.That(_rectangleColliderComponentModel.DimensionY, Is.EqualTo(123));
            Assert.That(_rectangleColliderComponent.Dimension.Y, Is.EqualTo(123));
        }
    }
}