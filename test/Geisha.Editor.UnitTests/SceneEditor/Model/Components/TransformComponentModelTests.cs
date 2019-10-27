using Geisha.Common.Math;
using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Engine.Core.Components;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.Model.Components
{
    [TestFixture]
    public class TransformComponentModelTests
    {
        private TransformComponent _transformComponent;
        private TransformComponentModel _transformComponentModel;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            _transformComponent = new TransformComponent
            {
                Translation = new Vector3(1, 2, 3),
                Rotation = new Vector3(4, 5, 6),
                Scale = new Vector3(7, 8, 9)
            };
            _transformComponentModel = new TransformComponentModel(_transformComponent);
        }

        [Test]
        public void TranslationX_ShouldUpdateTransformComponentTranslation()
        {
            // Assume
            Assume.That(_transformComponentModel.TranslationX, Is.EqualTo(1));

            // Act
            _transformComponentModel.TranslationX = 123;

            // Assert
            Assert.That(_transformComponentModel.TranslationX, Is.EqualTo(123));
            Assert.That(_transformComponent.Translation.X, Is.EqualTo(123));
        }

        [Test]
        public void TranslationY_ShouldUpdateTransformComponentTranslation()
        {
            // Assume
            Assume.That(_transformComponentModel.TranslationY, Is.EqualTo(2));

            // Act
            _transformComponentModel.TranslationY = 123;

            // Assert
            Assert.That(_transformComponentModel.TranslationY, Is.EqualTo(123));
            Assert.That(_transformComponent.Translation.Y, Is.EqualTo(123));
        }

        [Test]
        public void TranslationZ_ShouldUpdateTransformComponentTranslation()
        {
            // Assume
            Assume.That(_transformComponentModel.TranslationZ, Is.EqualTo(3));

            // Act
            _transformComponentModel.TranslationZ = 123;

            // Assert
            Assert.That(_transformComponentModel.TranslationZ, Is.EqualTo(123));
            Assert.That(_transformComponent.Translation.Z, Is.EqualTo(123));
        }

        [Test]
        public void RotationX_ShouldUpdateTransformComponentRotation()
        {
            // Assume
            Assume.That(_transformComponentModel.RotationX, Is.EqualTo(4));

            // Act
            _transformComponentModel.RotationX = 123;

            // Assert
            Assert.That(_transformComponentModel.RotationX, Is.EqualTo(123));
            Assert.That(_transformComponent.Rotation.X, Is.EqualTo(123));
        }

        [Test]
        public void RotationY_ShouldUpdateTransformComponentRotation()
        {
            // Assume
            Assume.That(_transformComponentModel.RotationY, Is.EqualTo(5));

            // Act
            _transformComponentModel.RotationY = 123;

            // Assert
            Assert.That(_transformComponentModel.RotationY, Is.EqualTo(123));
            Assert.That(_transformComponent.Rotation.Y, Is.EqualTo(123));
        }

        [Test]
        public void RotationZ_ShouldUpdateTransformComponentRotation()
        {
            // Assume
            Assume.That(_transformComponentModel.RotationZ, Is.EqualTo(6));

            // Act
            _transformComponentModel.RotationZ = 123;

            // Assert
            Assert.That(_transformComponentModel.RotationZ, Is.EqualTo(123));
            Assert.That(_transformComponent.Rotation.Z, Is.EqualTo(123));
        }

        [Test]
        public void ScaleX_ShouldUpdateTransformComponentScale()
        {
            // Assume
            Assume.That(_transformComponentModel.ScaleX, Is.EqualTo(7));

            // Act
            _transformComponentModel.ScaleX = 123;

            // Assert
            Assert.That(_transformComponentModel.ScaleX, Is.EqualTo(123));
            Assert.That(_transformComponent.Scale.X, Is.EqualTo(123));
        }

        [Test]
        public void ScaleY_ShouldUpdateTransformComponentScale()
        {
            // Assume
            Assume.That(_transformComponentModel.ScaleY, Is.EqualTo(8));

            // Act
            _transformComponentModel.ScaleY = 123;

            // Assert
            Assert.That(_transformComponentModel.ScaleY, Is.EqualTo(123));
            Assert.That(_transformComponent.Scale.Y, Is.EqualTo(123));
        }

        [Test]
        public void ScaleZ_ShouldUpdateTransformComponentScale()
        {
            // Arrange
            Assume.That(_transformComponentModel.ScaleZ, Is.EqualTo(9));

            // Act
            _transformComponentModel.ScaleZ = 123;

            // Assert
            Assert.That(_transformComponentModel.ScaleZ, Is.EqualTo(123));
            Assert.That(_transformComponent.Scale.Z, Is.EqualTo(123));
        }
    }
}