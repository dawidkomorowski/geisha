using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.Model.Components
{
    [TestFixture]
    public class Transform3DComponentModelTests
    {
        private Transform3DComponent _transformComponent = null!;
        private Transform3DComponentModel _transformComponentModel = null!;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            var scene = TestSceneFactory.Create();
            var entity = scene.CreateEntity();

            _transformComponent = entity.CreateComponent<Transform3DComponent>();
            _transformComponent.Translation = new Vector3(1, 2, 3);
            _transformComponent.Rotation = new Vector3(4, 5, 6);
            _transformComponent.Scale = new Vector3(7, 8, 9);

            _transformComponentModel = new Transform3DComponentModel(_transformComponent);
        }

        [Test]
        public void Translation_ShouldUpdateTransform3DComponentTranslation()
        {
            // Assume
            Assert.That(_transformComponentModel.Translation, Is.EqualTo(new Vector3(1, 2, 3)));

            // Act
            _transformComponentModel.Translation = new Vector3(123, 456, 789);

            // Assert
            Assert.That(_transformComponentModel.Translation, Is.EqualTo(new Vector3(123, 456, 789)));
            Assert.That(_transformComponent.Translation, Is.EqualTo(new Vector3(123, 456, 789)));
        }

        [Test]
        public void Rotation_ShouldUpdateTransform3DComponentRotation()
        {
            // Assume
            Assert.That(_transformComponentModel.Rotation, Is.EqualTo(new Vector3(4, 5, 6)));

            // Act
            _transformComponentModel.Rotation = new Vector3(123, 456, 789);

            // Assert
            Assert.That(_transformComponentModel.Rotation, Is.EqualTo(new Vector3(123, 456, 789)));
            Assert.That(_transformComponent.Rotation, Is.EqualTo(new Vector3(123, 456, 789)));
        }

        [Test]
        public void Scale_ShouldUpdateTransform3DComponentScale()
        {
            // Assume
            Assert.That(_transformComponentModel.Scale, Is.EqualTo(new Vector3(7, 8, 9)));

            // Act
            _transformComponentModel.Scale = new Vector3(123, 456, 789);

            // Assert
            Assert.That(_transformComponentModel.Scale, Is.EqualTo(new Vector3(123, 456, 789)));
            Assert.That(_transformComponent.Scale, Is.EqualTo(new Vector3(123, 456, 789)));
        }
    }
}