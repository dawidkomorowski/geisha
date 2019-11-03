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
        public void Translation_ShouldUpdateTransformComponentTranslation()
        {
            // Assume
            Assume.That(_transformComponentModel.Translation, Is.EqualTo(new Vector3(1, 2, 3)));

            // Act
            _transformComponentModel.Translation = new Vector3(123, 456, 789);

            // Assert
            Assert.That(_transformComponentModel.Translation, Is.EqualTo(new Vector3(123, 456, 789)));
            Assert.That(_transformComponent.Translation, Is.EqualTo(new Vector3(123, 456, 789)));
        }

        [Test]
        public void Rotation_ShouldUpdateTransformComponentRotation()
        {
            // Assume
            Assume.That(_transformComponentModel.Rotation, Is.EqualTo(new Vector3(4, 5, 6)));

            // Act
            _transformComponentModel.Rotation = new Vector3(123, 456, 789);

            // Assert
            Assert.That(_transformComponentModel.Rotation, Is.EqualTo(new Vector3(123, 456, 789)));
            Assert.That(_transformComponent.Rotation, Is.EqualTo(new Vector3(123, 456, 789)));
        }

        [Test]
        public void Scale_ShouldUpdateTransformComponentScale()
        {
            // Assume
            Assume.That(_transformComponentModel.Scale, Is.EqualTo(new Vector3(7, 8, 9)));

            // Act
            _transformComponentModel.Scale = new Vector3(123, 456, 789);

            // Assert
            Assert.That(_transformComponentModel.Scale, Is.EqualTo(new Vector3(123, 456, 789)));
            Assert.That(_transformComponent.Scale, Is.EqualTo(new Vector3(123, 456, 789)));
        }
    }
}