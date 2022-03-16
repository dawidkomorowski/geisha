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
            var rootTransform2DComponent = CreateRandomTransform2DComponent();
            var expected = rootTransform2DComponent.ToMatrix();

            var rootEntity = Scene.CreateEntity();
            rootEntity.AddComponent(rootTransform2DComponent);

            // Act
            var transformationMatrix = TransformHierarchy.Calculate2DTransformationMatrix(rootEntity);

            // Assert
            Assert.That(transformationMatrix, Is.EqualTo(expected).Using(Matrix3x3Comparer));
        }

        [Test]
        public void Calculate2DTransformationMatrix_ShouldReturnProductOfTransformOfEntityAndTransformOfParentEntity_GivenEntityHasParent()
        {
            // Arrange
            var level0Transform2DComponent = CreateRandomTransform2DComponent();
            var level1Transform2DComponent = CreateRandomTransform2DComponent();
            var expected = level0Transform2DComponent.ToMatrix() * level1Transform2DComponent.ToMatrix();

            var level0Entity = Scene.CreateEntity();
            level0Entity.AddComponent(level0Transform2DComponent);

            var level1Entity = level0Entity.CreateChildEntity();
            level1Entity.AddComponent(level1Transform2DComponent);

            // Act
            var transformationMatrix = TransformHierarchy.Calculate2DTransformationMatrix(level1Entity);

            // Assert
            Assert.That(transformationMatrix, Is.EqualTo(expected).Using(Matrix3x3Comparer));
        }

        private static Transform2DComponent CreateRandomTransform2DComponent()
        {
            return new Transform2DComponent
            {
                Translation = Utils.RandomVector2(),
                Rotation = Utils.Random.NextDouble(),
                Scale = Utils.RandomVector2()
            };
        }
    }
}