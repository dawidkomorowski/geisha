using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.Transform3DComponent;
using Geisha.Engine.Core.Math;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.UserInterface.EntityPropertiesEditor.Components.Transform3DComponent
{
    [TestFixture]
    public class Transform3DComponentPropertiesEditorViewModelTests
    {
        private Transform3DComponentModel _transformComponentModel = null!;
        private Transform3DComponentPropertiesEditorViewModel _transformComponentPropertiesEditorViewModel = null!;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            var scene = TestSceneFactory.Create();
            var entity = scene.CreateEntity();

            var transformComponent = entity.CreateComponent<Engine.Core.Components.Transform3DComponent>();
            transformComponent.Translation = new Vector3(1, 2, 3);
            transformComponent.Rotation = new Vector3(4, 5, 6);
            transformComponent.Scale = new Vector3(7, 8, 9);

            _transformComponentModel = new Transform3DComponentModel(transformComponent);
            _transformComponentPropertiesEditorViewModel = new Transform3DComponentPropertiesEditorViewModel(_transformComponentModel);
        }

        [Test]
        public void Translation_ShouldUpdateTransform3DComponentModelTranslation()
        {
            // Assume
            Assert.That(_transformComponentPropertiesEditorViewModel.Translation, Is.EqualTo(new Vector3(1, 2, 3)));

            // Act
            _transformComponentPropertiesEditorViewModel.Translation = new Vector3(123, 456, 789);

            // Assert
            Assert.That(_transformComponentPropertiesEditorViewModel.Translation, Is.EqualTo(new Vector3(123, 456, 789)));
            Assert.That(_transformComponentModel.Translation, Is.EqualTo(new Vector3(123, 456, 789)));
        }

        [Test]
        public void Rotation_ShouldUpdateTransform3DComponentModelRotation()
        {
            // Assume
            Assert.That(_transformComponentPropertiesEditorViewModel.Rotation, Is.EqualTo(new Vector3(4, 5, 6)));

            // Act
            _transformComponentPropertiesEditorViewModel.Rotation = new Vector3(123, 456, 789);

            // Assert
            Assert.That(_transformComponentPropertiesEditorViewModel.Rotation, Is.EqualTo(new Vector3(123, 456, 789)));
            Assert.That(_transformComponentModel.Rotation, Is.EqualTo(new Vector3(123, 456, 789)));
        }

        [Test]
        public void Scale_ShouldUpdateTransform3DComponentModelScale()
        {
            // Assume
            Assert.That(_transformComponentPropertiesEditorViewModel.Scale, Is.EqualTo(new Vector3(7, 8, 9)));

            // Act
            _transformComponentPropertiesEditorViewModel.Scale = new Vector3(123, 456, 789);

            // Assert
            Assert.That(_transformComponentPropertiesEditorViewModel.Scale, Is.EqualTo(new Vector3(123, 456, 789)));
            Assert.That(_transformComponentModel.Scale, Is.EqualTo(new Vector3(123, 456, 789)));
        }
    }
}