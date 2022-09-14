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
    public class Transform3DComponentTests
    {
        private const double Epsilon = 0.0001;
        private static IEqualityComparer<Vector3> Vector3Comparer => CommonEqualityComparer.Vector3(Epsilon);

        private Entity Entity { get; set; } = null!;

        [SetUp]
        public void SetUp()
        {
            var scene = TestSceneFactory.Create();
            Entity = scene.CreateEntity();
        }

        [Test]
        public void Constructor_ShouldReturnTransform3DComponentWithZeroTranslationZeroRotationAndScaleEqualOne()
        {
            // Arrange
            // Act
            var transformComponent = Entity.CreateComponent<Transform3DComponent>();

            // Assert
            Assert.That(transformComponent.Translation, Is.EqualTo(Vector3.Zero));
            Assert.That(transformComponent.Rotation, Is.EqualTo(Vector3.Zero));
            Assert.That(transformComponent.Scale, Is.EqualTo(Vector3.One));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 1, 1, 1,
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1)]
        [TestCase(1, -2, 3, 0, 0, 0, 1, 1, 1,
            1, 0, 0, 1,
            0, 1, 0, -2,
            0, 0, 1, 3,
            0, 0, 0, 1)]
        [TestCase(0, 0, 0, 1, -2, 3, 1, 1, 1,
            0.3040, 0.8162, -0.4913, 0,
            0.0762, -0.5349, -0.8415, 0,
            -0.9496, 0.2184, -0.2248, 0,
            0, 0, 0, 1)]
        [TestCase(0, 0, 0, 0, 0, 0, 2, -3, 4,
            2, 0, 0, 0,
            0, -3, 0, 0,
            0, 0, 4, 0,
            0, 0, 0, 1)]
        [TestCase(1, -2, 3, 1, -2, 3, 2, -3, 4,
            0.6080, -2.4487, -1.9652, 1,
            0.1525, 1.6047, -3.3659, -2,
            -1.8992, -0.6551, -0.8994, 3,
            0, 0, 0, 1)]
        public void ToMatrix(double tx, double ty, double tz, double rx, double ry, double rz,
            double sx, double sy, double sz, double m11, double m12, double m13, double m14, double m21, double m22,
            double m23, double m24, double m31, double m32, double m33, double m34, double m41, double m42, double m43,
            double m44)
        {
            // Arrange
            var transformComponent = Entity.CreateComponent<Transform3DComponent>();
            transformComponent.Translation = new Vector3(tx, ty, tz);
            transformComponent.Rotation = new Vector3(rx, ry, rz);
            transformComponent.Scale = new Vector3(sx, sy, sz);

            // Act
            var matrix = transformComponent.ToMatrix();

            // Assert
            Assert.That(matrix.M11, Is.EqualTo(m11));
            Assert.That(matrix.M12, Is.EqualTo(m12));
            Assert.That(matrix.M13, Is.EqualTo(m13));
            Assert.That(matrix.M14, Is.EqualTo(m14));

            Assert.That(matrix.M21, Is.EqualTo(m21));
            Assert.That(matrix.M22, Is.EqualTo(m22));
            Assert.That(matrix.M23, Is.EqualTo(m23));
            Assert.That(matrix.M24, Is.EqualTo(m24));

            Assert.That(matrix.M31, Is.EqualTo(m31));
            Assert.That(matrix.M32, Is.EqualTo(m32));
            Assert.That(matrix.M33, Is.EqualTo(m33));
            Assert.That(matrix.M34, Is.EqualTo(m34));

            Assert.That(matrix.M41, Is.EqualTo(m41));
            Assert.That(matrix.M42, Is.EqualTo(m42));
            Assert.That(matrix.M43, Is.EqualTo(m43));
            Assert.That(matrix.M44, Is.EqualTo(m44));
        }

        [TestCase(0, 0, 0,
            1, 0, 0)]
        [TestCase(System.Math.PI / 2, 0, 0,
            1, 0, 0)]
        [TestCase(0, System.Math.PI / 2, 0,
            0, 0, -1)]
        [TestCase(0, 0, System.Math.PI / 2,
            0, 1, 0)]
        [TestCase(0.1525, 1.6047, -3.3659,
            0.066818001720415354, 0.2198496659535544, 0.97324276469244875)]
        public void VectorX(double rx, double ry, double rz, double vx, double vy, double vz)
        {
            // Arrange
            var transformComponent = Entity.CreateComponent<Transform3DComponent>();
            transformComponent.Rotation = new Vector3(rx, ry, rz);

            // Act
            var vectorX = transformComponent.VectorX;

            // Assert
            Assert.That(vectorX, Is.EqualTo(new Vector3(vx, vy, vz)).Using(Vector3Comparer));
        }

        [TestCase(0, 0, 0,
            0, 1, 0)]
        [TestCase(System.Math.PI / 2, 0, 0,
            0, 0, 1)]
        [TestCase(0, System.Math.PI / 2, 0,
            0, 1, 0)]
        [TestCase(0, 0, System.Math.PI / 2,
            -1, 0, 0)]
        [TestCase(0.1525, 1.6047, -3.3659,
            -0.14047911485484807, -0.96363354064912776, 0.22732359671979577)]
        public void VectorY(double rx, double ry, double rz, double vx, double vy, double vz)
        {
            // Arrange
            var transformComponent = Entity.CreateComponent<Transform3DComponent>();
            transformComponent.Rotation = new Vector3(rx, ry, rz);

            // Act
            var vectorY = transformComponent.VectorY;

            // Assert
            Assert.That(vectorY, Is.EqualTo(new Vector3(vx, vy, vz)).Using(Vector3Comparer));
        }

        [TestCase(0, 0, 0,
            0, 0, 1)]
        [TestCase(System.Math.PI / 2, 0, 0,
            0, -1, 0)]
        [TestCase(0, System.Math.PI / 2, 0,
            1, 0, 0)]
        [TestCase(0, 0, System.Math.PI / 2,
            0, 0, 1)]
        [TestCase(0.1525, 1.6047, -3.3659,
            0.98782638805393785, -0.15190959059959469, -0.033503781102654007)]
        public void VectorZ(double rx, double ry, double rz, double vx, double vy, double vz)
        {
            // Arrange
            var transformComponent = Entity.CreateComponent<Transform3DComponent>();
            transformComponent.Rotation = new Vector3(rx, ry, rz);

            // Act
            var vectorZ = transformComponent.VectorZ;

            // Assert
            Assert.That(vectorZ, Is.EqualTo(new Vector3(vx, vy, vz)).Using(Vector3Comparer));
        }
    }
}