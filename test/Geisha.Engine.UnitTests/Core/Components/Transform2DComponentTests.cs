using System.Collections.Generic;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Components
{
    [TestFixture]
    [DefaultFloatingPointTolerance(Epsilon)]
    public class Transform2DComponentTests
    {
        private const double Epsilon = 0.0001;
        private static IEqualityComparer<Vector2> Vector2Comparer => CommonEqualityComparer.Vector2(Epsilon);

        private Entity Entity { get; set; } = null!;

        [SetUp]
        public void SetUp()
        {
            var scene = TestSceneFactory.Create();
            Entity = scene.CreateEntity();
        }

        [Test]
        public void Constructor_ShouldReturnTransform2DComponentWithZeroTranslationZeroRotationAndScaleEqualOne()
        {
            // Arrange
            // Act
            var transformComponent = Entity.CreateComponent<Transform2DComponent>();

            // Assert
            Assert.That(transformComponent.Translation, Is.EqualTo(Vector2.Zero));
            Assert.That(transformComponent.Rotation, Is.EqualTo(0));
            Assert.That(transformComponent.Scale, Is.EqualTo(Vector2.One));
        }

        [Test]
        public void Constructor_ShouldThrowException_WhenTransform2DComponentIsAddedToEntityWithTransform2DComponent()
        {
            // Arrange
            Entity.CreateComponent<Transform2DComponent>();

            // Act
            // Assert
            Assert.That(() => Entity.CreateComponent<Transform2DComponent>(), Throws.ArgumentException);
        }

        [Test]
        public void Constructor_ShouldThrowException_WhenTransform2DComponentIsAddedToEntityWithTransform3DComponent()
        {
            // Arrange
            Entity.CreateComponent<Transform3DComponent>();

            // Act
            // Assert
            Assert.That(() => Entity.CreateComponent<Transform2DComponent>(), Throws.ArgumentException);
        }

        [TestCase(0, 0, 0, 1, 1,
            1, 0, 0,
            0, 1, 0,
            0, 0, 1)]
        [TestCase(1, -2, 0, 1, 1,
            1, 0, 1,
            0, 1, -2,
            0, 0, 1)]
        [TestCase(0, 0, System.Math.PI / 2, 1, 1,
            0, -1, 0,
            1, 0, 0,
            0, 0, 1)]
        [TestCase(0, 0, 0, 2, -3,
            2, 0, 0,
            0, -3, 0,
            0, 0, 1)]
        [TestCase(1, -2, System.Math.PI / 4, 2, -3,
            1.4142, 2.1213, 1,
            1.4142, -2.1213, -2,
            0, 0, 1)]
        public void ToMatrix(double tx, double ty, double r, double sx, double sy, double m11, double m12, double m13, double m21, double m22, double m23,
            double m31, double m32, double m33)
        {
            // Arrange
            var transformComponent = Entity.CreateComponent<Transform2DComponent>();
            transformComponent.Translation = new Vector2(tx, ty);
            transformComponent.Rotation = r;
            transformComponent.Scale = new Vector2(sx, sy);

            // Act
            var matrix = transformComponent.ToMatrix();

            // Assert
            Assert.That(matrix.M11, Is.EqualTo(m11));
            Assert.That(matrix.M12, Is.EqualTo(m12));
            Assert.That(matrix.M13, Is.EqualTo(m13));

            Assert.That(matrix.M21, Is.EqualTo(m21));
            Assert.That(matrix.M22, Is.EqualTo(m22));
            Assert.That(matrix.M23, Is.EqualTo(m23));

            Assert.That(matrix.M31, Is.EqualTo(m31));
            Assert.That(matrix.M32, Is.EqualTo(m32));
            Assert.That(matrix.M33, Is.EqualTo(m33));
        }

        [TestCase(0, 1, 0)]
        [TestCase(System.Math.PI / 2, 0, 1)]
        [TestCase(-3.3659, -0.97495, 0.22243)]
        public void VectorX(double r, double vx, double vy)
        {
            // Arrange
            var transformComponent = Entity.CreateComponent<Transform2DComponent>();
            transformComponent.Rotation = r;

            // Act
            var vectorX = transformComponent.VectorX;

            // Assert
            Assert.That(vectorX, Is.EqualTo(new Vector2(vx, vy)).Using(Vector2Comparer));
        }

        [TestCase(0, 0, 1)]
        [TestCase(System.Math.PI / 2, -1, 0)]
        [TestCase(-3.3659, -0.22243, -0.97495)]
        public void VectorY(double r, double vx, double vy)
        {
            // Arrange
            var transformComponent = Entity.CreateComponent<Transform2DComponent>();
            transformComponent.Rotation = r;

            // Act
            var vectorY = transformComponent.VectorY;

            // Assert
            Assert.That(vectorY, Is.EqualTo(new Vector2(vx, vy)).Using(Vector2Comparer));
        }
    }
}