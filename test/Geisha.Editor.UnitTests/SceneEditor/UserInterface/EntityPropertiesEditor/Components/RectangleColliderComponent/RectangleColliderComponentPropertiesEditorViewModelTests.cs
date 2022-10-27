using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.RectangleColliderComponent;
using Geisha.Engine.Core.Math;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.UserInterface.EntityPropertiesEditor.Components.RectangleColliderComponent
{
    [TestFixture]
    public class RectangleColliderComponentPropertiesEditorViewModelTests
    {
        private RectangleColliderComponentModel _rectangleColliderComponentModel = null!;
        private RectangleColliderComponentPropertiesEditorViewModel _rectangleColliderComponentPropertiesEditorViewModel = null!;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            var scene = TestSceneFactory.Create();
            var entity = scene.CreateEntity();

            var rectangleColliderComponent = entity.CreateComponent<Engine.Physics.Components.RectangleColliderComponent>();
            rectangleColliderComponent.Dimensions = new Vector2(1, 2);

            _rectangleColliderComponentModel = new RectangleColliderComponentModel(rectangleColliderComponent);
            _rectangleColliderComponentPropertiesEditorViewModel = new RectangleColliderComponentPropertiesEditorViewModel(_rectangleColliderComponentModel);
        }

        [Test]
        public void Dimensions_ShouldUpdateRectangleColliderComponentModelDimensions()
        {
            // Assume
            Assume.That(_rectangleColliderComponentPropertiesEditorViewModel.Dimensions, Is.EqualTo(new Vector2(1, 2)));

            // Act
            _rectangleColliderComponentPropertiesEditorViewModel.Dimensions = new Vector2(123, 456);

            // Assert
            Assert.That(_rectangleColliderComponentPropertiesEditorViewModel.Dimensions, Is.EqualTo(new Vector2(123, 456)));
            Assert.That(_rectangleColliderComponentModel.Dimensions, Is.EqualTo(new Vector2(123, 456)));
        }
    }
}