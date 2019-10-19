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
    }
}