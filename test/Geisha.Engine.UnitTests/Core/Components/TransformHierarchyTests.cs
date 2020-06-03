using System.Collections.Generic;
using Geisha.Common.Math;
using Geisha.Common.TestUtils;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Components
{
    [TestFixture]
    public sealed class TransformHierarchyTests
    {
        private const double Epsilon = 0.000001;
        private static IEqualityComparer<Matrix3x3> Matrix3x3Comparer => CommonEqualityComparer.Matrix3x3(Epsilon);

        [Test]
        public void CalculateTransformationMatrix_ShouldReturnTransformOfEntity_GivenEntityIsRoot()
        {
            // Arrange
            var rootTransformComponent = CreateRandomTransformComponent();
            var expected = rootTransformComponent.Create2DTransformationMatrix();

            var rootEntity = new Entity();
            rootEntity.AddComponent(rootTransformComponent);

            // Act
            var transformationMatrix = TransformHierarchy.CalculateTransformationMatrix(rootEntity);

            // Assert
            Assert.That(transformationMatrix, Is.EqualTo(expected).Using(Matrix3x3Comparer));
        }

        [Test]
        public void CalculateTransformationMatrix_ShouldReturnProductOfTransformOfEntityAndTransformOfParentEntity_GivenEntityHasParent()
        {
            // Arrange
            var level0TransformComponent = CreateRandomTransformComponent();
            var level1TransformComponent = CreateRandomTransformComponent();
            var expected = level0TransformComponent.Create2DTransformationMatrix() * level1TransformComponent.Create2DTransformationMatrix();

            var level0Entity = new Entity();
            level0Entity.AddComponent(level0TransformComponent);

            var level1Entity = new Entity();
            level1Entity.AddComponent(level1TransformComponent);

            level0Entity.AddChild(level1Entity);

            // Act
            var transformationMatrix = TransformHierarchy.CalculateTransformationMatrix(level1Entity);

            // Assert
            Assert.That(transformationMatrix, Is.EqualTo(expected).Using(Matrix3x3Comparer));
        }

        private static TransformComponent CreateRandomTransformComponent()
        {
            return new TransformComponent
            {
                Translation = Utils.RandomVector3(),
                Rotation = Utils.RandomVector3(),
                Scale = Utils.RandomVector3()
            };
        }
    }
}