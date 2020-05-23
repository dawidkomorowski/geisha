using Geisha.Common.Math;
using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.TransformComponent;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.UserInterface.EntityPropertiesEditor.Components.TransformComponent
{
    [TestFixture]
    public class TransformComponentPropertiesEditorViewModelTests
    {
        private TransformComponentModel _transformComponentModel = null!;
        private TransformComponentPropertiesEditorViewModel _transformComponentPropertiesEditorViewModel = null!;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            var transformComponent = new Engine.Core.Components.TransformComponent
            {
                Translation = new Vector3(1, 2, 3),
                Rotation = new Vector3(4, 5, 6),
                Scale = new Vector3(7, 8, 9)
            };
            _transformComponentModel = new TransformComponentModel(transformComponent);
            _transformComponentPropertiesEditorViewModel = new TransformComponentPropertiesEditorViewModel(_transformComponentModel);
        }

        [Test]
        public void Translation_ShouldUpdateTransformComponentModelTranslation()
        {
            // Assume
            Assume.That(_transformComponentPropertiesEditorViewModel.Translation, Is.EqualTo(new Vector3(1, 2, 3)));

            // Act
            _transformComponentPropertiesEditorViewModel.Translation = new Vector3(123, 456, 789);

            // Assert
            Assert.That(_transformComponentPropertiesEditorViewModel.Translation, Is.EqualTo(new Vector3(123, 456, 789)));
            Assert.That(_transformComponentModel.Translation, Is.EqualTo(new Vector3(123, 456, 789)));
        }

        [Test]
        public void Rotation_ShouldUpdateTransformComponentModelRotation()
        {
            // Assume
            Assume.That(_transformComponentPropertiesEditorViewModel.Rotation, Is.EqualTo(new Vector3(4, 5, 6)));

            // Act
            _transformComponentPropertiesEditorViewModel.Rotation = new Vector3(123, 456, 789);

            // Assert
            Assert.That(_transformComponentPropertiesEditorViewModel.Rotation, Is.EqualTo(new Vector3(123, 456, 789)));
            Assert.That(_transformComponentModel.Rotation, Is.EqualTo(new Vector3(123, 456, 789)));
        }

        [Test]
        public void Scale_ShouldUpdateTransformComponentModelScale()
        {
            // Assume
            Assume.That(_transformComponentPropertiesEditorViewModel.Scale, Is.EqualTo(new Vector3(7, 8, 9)));

            // Act
            _transformComponentPropertiesEditorViewModel.Scale = new Vector3(123, 456, 789);

            // Assert
            Assert.That(_transformComponentPropertiesEditorViewModel.Scale, Is.EqualTo(new Vector3(123, 456, 789)));
            Assert.That(_transformComponentModel.Scale, Is.EqualTo(new Vector3(123, 456, 789)));
        }
    }
}