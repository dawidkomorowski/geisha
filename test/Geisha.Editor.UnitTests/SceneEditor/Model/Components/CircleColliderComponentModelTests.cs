using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Engine.Physics.Components;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.Model.Components
{
    [TestFixture]
    public class CircleColliderComponentModelTests
    {
        [Test]
        public void Radius_ShouldUpdateCircleColliderComponentRadius()
        {
            // Arrange
            var circleColliderComponent = new CircleColliderComponent {Radius = 123};
            var circleColliderComponentModel = new CircleColliderComponentModel(circleColliderComponent);

            // Assume
            Assume.That(circleColliderComponentModel.Radius, Is.EqualTo(123));

            // Act
            circleColliderComponentModel.Radius = 456;

            // Assert
            Assert.That(circleColliderComponentModel.Radius, Is.EqualTo(456));
            Assert.That(circleColliderComponent.Radius, Is.EqualTo(456));
        }
    }
}