using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.CircleColliderComponent;
using Geisha.Engine.Core.SceneModel;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.UserInterface.EntityPropertiesEditor.Components.CircleColliderComponent
{
    [TestFixture]
    public class CircleColliderComponentPropertiesEditorViewModelTests
    {
        private Entity Entity { get; set; } = null!;

        [SetUp]
        public void SetUp()
        {
            var scene = TestSceneFactory.Create();
            Entity = scene.CreateEntity();
        }

        [Test]
        public void Radius_ShouldUpdateCircleColliderComponentModelRadius()
        {
            // Arrange
            var circleColliderComponent = Entity.CreateComponent<Engine.Physics.Components.CircleColliderComponent>();
            circleColliderComponent.Radius = 123;
            var circleColliderComponentModel = new CircleColliderComponentModel(circleColliderComponent);
            var circleColliderComponentPropertiesEditorViewModel = new CircleColliderComponentPropertiesEditorViewModel(circleColliderComponentModel);

            // Assume
            Assume.That(circleColliderComponentPropertiesEditorViewModel.Radius, Is.EqualTo(123));

            // Act
            circleColliderComponentPropertiesEditorViewModel.Radius = 456;

            // Assert
            Assert.That(circleColliderComponentPropertiesEditorViewModel.Radius, Is.EqualTo(456));
            Assert.That(circleColliderComponentModel.Radius, Is.EqualTo(456));
        }
    }
}