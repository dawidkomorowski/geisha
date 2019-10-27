using System.Threading;
using Geisha.Common.Math;
using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.TransformComponent;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.UserInterface.EntityPropertiesEditor.Components.TransformComponent
{
    [TestFixture]
    public class TransformComponentPropertiesEditorViewModelTests
    {
        private TransformComponentModel _transformComponentModel;
        private TransformComponentPropertiesEditorViewModel _transformComponentPropertiesEditorViewModel;

        [SetUp]
        public void SetUp()
        {
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
        [Apartment(ApartmentState.STA)]
        public void TranslationX_ShouldUpdateTransformComponentModelTranslationX()
        {
            // Assume
            Assume.That(_transformComponentPropertiesEditorViewModel.TranslationX, Is.EqualTo(1));

            // Act
            _transformComponentPropertiesEditorViewModel.TranslationX = 123;

            // Assert
            Assert.That(_transformComponentPropertiesEditorViewModel.TranslationX, Is.EqualTo(123));
            Assert.That(_transformComponentModel.TranslationX, Is.EqualTo(123));
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void TranslationY_ShouldUpdateTransformComponentModelTranslationY()
        {
            // Assume
            Assume.That(_transformComponentPropertiesEditorViewModel.TranslationY, Is.EqualTo(2));

            // Act
            _transformComponentPropertiesEditorViewModel.TranslationY = 123;

            // Assert
            Assert.That(_transformComponentPropertiesEditorViewModel.TranslationY, Is.EqualTo(123));
            Assert.That(_transformComponentModel.TranslationY, Is.EqualTo(123));
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void TranslationZ_ShouldUpdateTransformComponentModelTranslationZ()
        {
            // Assume
            Assume.That(_transformComponentPropertiesEditorViewModel.TranslationZ, Is.EqualTo(3));

            // Act
            _transformComponentPropertiesEditorViewModel.TranslationZ = 123;

            // Assert
            Assert.That(_transformComponentPropertiesEditorViewModel.TranslationZ, Is.EqualTo(123));
            Assert.That(_transformComponentModel.TranslationZ, Is.EqualTo(123));
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void RotationX_ShouldUpdateTransformComponentModelRotationX()
        {
            // Assume
            Assume.That(_transformComponentPropertiesEditorViewModel.RotationX, Is.EqualTo(4));

            // Act
            _transformComponentPropertiesEditorViewModel.RotationX = 123;

            // Assert
            Assert.That(_transformComponentPropertiesEditorViewModel.RotationX, Is.EqualTo(123));
            Assert.That(_transformComponentModel.RotationX, Is.EqualTo(123));
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void RotationY_ShouldUpdateTransformComponentModelRotationY()
        {
            // Assume
            Assume.That(_transformComponentPropertiesEditorViewModel.RotationY, Is.EqualTo(5));

            // Act
            _transformComponentPropertiesEditorViewModel.RotationY = 123;

            // Assert
            Assert.That(_transformComponentPropertiesEditorViewModel.RotationY, Is.EqualTo(123));
            Assert.That(_transformComponentModel.RotationY, Is.EqualTo(123));
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void RotationZ_ShouldUpdateTransformComponentModelRotationZ()
        {
            // Assume
            Assume.That(_transformComponentPropertiesEditorViewModel.RotationZ, Is.EqualTo(6));

            // Act
            _transformComponentPropertiesEditorViewModel.RotationZ = 123;

            // Assert
            Assert.That(_transformComponentPropertiesEditorViewModel.RotationZ, Is.EqualTo(123));
            Assert.That(_transformComponentModel.RotationZ, Is.EqualTo(123));
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ScaleX_ShouldUpdateTransformComponentModelScaleX()
        {
            // Assume
            Assume.That(_transformComponentPropertiesEditorViewModel.ScaleX, Is.EqualTo(7));

            // Act
            _transformComponentPropertiesEditorViewModel.ScaleX = 123;

            // Assert
            Assert.That(_transformComponentPropertiesEditorViewModel.ScaleX, Is.EqualTo(123));
            Assert.That(_transformComponentModel.ScaleX, Is.EqualTo(123));
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ScaleY_ShouldUpdateTransformComponentModelScaleY()
        {
            // Assume
            Assume.That(_transformComponentPropertiesEditorViewModel.ScaleY, Is.EqualTo(8));

            // Act
            _transformComponentPropertiesEditorViewModel.ScaleY = 123;

            // Assert
            Assert.That(_transformComponentPropertiesEditorViewModel.ScaleY, Is.EqualTo(123));
            Assert.That(_transformComponentModel.ScaleY, Is.EqualTo(123));
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ScaleZ_ShouldUpdateTransformComponentModelScaleZ()
        {
            // Assume
            Assume.That(_transformComponentPropertiesEditorViewModel.ScaleZ, Is.EqualTo(9));

            // Act
            _transformComponentPropertiesEditorViewModel.ScaleZ = 123;

            // Assert
            Assert.That(_transformComponentPropertiesEditorViewModel.ScaleZ, Is.EqualTo(123));
            Assert.That(_transformComponentModel.ScaleZ, Is.EqualTo(123));
        }
    }
}