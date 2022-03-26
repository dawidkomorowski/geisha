using Geisha.Common.Math;
using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Engine.Physics.Components;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.Model.Components
{
    [TestFixture]
    public class RectangleColliderComponentModelTests
    {
        private RectangleColliderComponent _rectangleColliderComponent = null!;
        private RectangleColliderComponentModel _rectangleColliderComponentModel = null!;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            var scene = TestSceneFactory.Create();
            var entity = scene.CreateEntity();

            _rectangleColliderComponent = entity.CreateComponent<RectangleColliderComponent>();
            _rectangleColliderComponent.Dimension = new Vector2(1, 2);
            _rectangleColliderComponentModel = new RectangleColliderComponentModel(_rectangleColliderComponent);
        }

        [Test]
        public void Dimension_ShouldUpdateRectangleColliderComponentDimension()
        {
            // Assume
            Assume.That(_rectangleColliderComponentModel.Dimension, Is.EqualTo(new Vector2(1, 2)));

            // Act
            _rectangleColliderComponentModel.Dimension = new Vector2(123, 456);

            // Assert
            Assert.That(_rectangleColliderComponentModel.Dimension, Is.EqualTo(new Vector2(123, 456)));
            Assert.That(_rectangleColliderComponent.Dimension, Is.EqualTo(new Vector2(123, 456)));
        }
    }
}