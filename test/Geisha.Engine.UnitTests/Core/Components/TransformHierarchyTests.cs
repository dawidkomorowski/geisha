using System.Collections.Generic;
using Geisha.Common.Math;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Components
{
    [TestFixture]
    public sealed class TransformHierarchyTests
    {
        private const double Epsilon = 0.000001;
        private static IEqualityComparer<Matrix3x3> Matrix3x3Comparer => CommonEqualityComparer.Matrix3x3(Epsilon);

        private Scene Scene { get; set; } = null!;

        [SetUp]
        public void SetUp()
        {
            Scene = TestSceneFactory.Create();
        }

        [Test]
        public void Calculate2DTransformationMatrix_ShouldReturnTransformOfEntity_GivenEntityIsRoot()
        {
            // Arrange
            var rootEntity = Scene.CreateEntity();
            var rootTransform2DComponent = rootEntity.CreateComponent<Transform2DComponent>();
            SetRandomValues(rootTransform2DComponent);
            var expected = rootTransform2DComponent.ToMatrix();

            // Act
            var transformationMatrix = TransformHierarchy.Calculate2DTransformationMatrix(rootEntity);

            // Assert
            Assert.That(transformationMatrix, Is.EqualTo(expected).Using(Matrix3x3Comparer));
        }

        [Test]
        public void Calculate2DTransformationMatrix_ShouldReturnProductOfTransformOfEntityAndTransformOfParentEntity_GivenEntityHasParent()
        {
            // Arrange
            var level0Entity = Scene.CreateEntity();
            var level0Transform2DComponent = level0Entity.CreateComponent<Transform2DComponent>();
            SetRandomValues(level0Transform2DComponent);

            var level1Entity = level0Entity.CreateChildEntity();
            var level1Transform2DComponent = level1Entity.CreateComponent<Transform2DComponent>();
            SetRandomValues(level1Transform2DComponent);

            var expected = level0Transform2DComponent.ToMatrix() * level1Transform2DComponent.ToMatrix();

            // Act
            var transformationMatrix = TransformHierarchy.Calculate2DTransformationMatrix(level1Entity);

            // Assert
            Assert.That(transformationMatrix, Is.EqualTo(expected).Using(Matrix3x3Comparer));
        }

        private static void SetRandomValues(Transform2DComponent transform2DComponent)
        {
            transform2DComponent.Translation = Utils.RandomVector2();
            transform2DComponent.Rotation = Utils.Random.NextDouble();
            transform2DComponent.Scale = Utils.RandomVector2();
        }
    }
}