using System.Collections.Generic;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Components;

[TestFixture]
[DefaultFloatingPointTolerance(Epsilon)]
public class Transform2DComponentTests
{
    private const double Epsilon = 0.0001;
    private static IEqualityComparer<Vector2> Vector2Comparer => CommonEqualityComparer.Vector2(Epsilon);

    private Scene Scene { get; set; } = null!;
    private Entity Entity { get; set; } = null!;

    [SetUp]
    public void SetUp()
    {
        Scene = TestSceneFactory.Create();
        Entity = Scene.CreateEntity();
    }

    [Test]
    public void Constructor_ShouldCreateEmptyTransform2DComponent()
    {
        // Arrange
        // Act
        var transformComponent = Entity.CreateComponent<Transform2DComponent>();

        // Assert
        Assert.That(transformComponent.Translation, Is.EqualTo(Vector2.Zero));
        Assert.That(transformComponent.Rotation, Is.EqualTo(0));
        Assert.That(transformComponent.Scale, Is.EqualTo(Vector2.One));
        Assert.That(transformComponent.IsInterpolated, Is.False);
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

    [Test]
    public void Transform_Get_ShouldReturnTransform2DReflectingTranslationRotationAndScale()
    {
        // Arrange
        var transform2DComponent = Entity.CreateComponent<Transform2DComponent>();
        transform2DComponent.Translation = new Vector2(1, 2);
        transform2DComponent.Rotation = 3;
        transform2DComponent.Scale = new Vector2(4, 5);

        // Act
        var transform2D = transform2DComponent.Transform;

        // Assert
        Assert.That(transform2D.Translation, Is.EqualTo(transform2DComponent.Translation));
        Assert.That(transform2D.Rotation, Is.EqualTo(transform2DComponent.Rotation));
        Assert.That(transform2D.Scale, Is.EqualTo(transform2DComponent.Scale));
    }

    [Test]
    public void Transform_Set_ShouldSetTranslationRotationAndScale()
    {
        // Arrange
        var transform2D = new Transform2D
        {
            Translation = new Vector2(1, 2),
            Rotation = 3,
            Scale = new Vector2(4, 5)
        };
        var transform2DComponent = Entity.CreateComponent<Transform2DComponent>();

        // Act
        transform2DComponent.Transform = transform2D;

        // Assert
        Assert.That(transform2DComponent.Translation, Is.EqualTo(transform2D.Translation));
        Assert.That(transform2DComponent.Rotation, Is.EqualTo(transform2D.Rotation));
        Assert.That(transform2DComponent.Scale, Is.EqualTo(transform2D.Scale));
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

    [Test]
    public void InterpolatedTransform_ShouldReturnTransform_WhenIsNotInterpolated()
    {
        // Arrange
        var transform2DComponent = Entity.CreateComponent<Transform2DComponent>();
        transform2DComponent.Translation = new Vector2(1, 2);
        transform2DComponent.Rotation = 3;
        transform2DComponent.Scale = new Vector2(4, 5);
        transform2DComponent.IsInterpolated = false;

        // Act
        var interpolatedTransform = transform2DComponent.InterpolatedTransform;

        // Assert
        Assert.That(interpolatedTransform, Is.EqualTo(transform2DComponent.Transform));
    }

    [Test]
    public void InterpolatedTransform_ShouldReturnTransform_WhenIsInterpolated_ButNotYetManagedByTransformInterpolationSystem()
    {
        // Arrange
        var transform2DComponent = Entity.CreateComponent<Transform2DComponent>();
        transform2DComponent.Translation = new Vector2(1, 2);
        transform2DComponent.Rotation = 3;
        transform2DComponent.Scale = new Vector2(4, 5);
        transform2DComponent.IsInterpolated = true;

        // Act
        var interpolatedTransform = transform2DComponent.InterpolatedTransform;

        // Assert
        Assert.That(interpolatedTransform, Is.EqualTo(transform2DComponent.Transform));
    }

    [Test]
    public void SetTransformImmediate_ShouldSetTranslationRotationAndScale()
    {
        // Arrange
        var transform2D = new Transform2D
        {
            Translation = new Vector2(1, 2),
            Rotation = 3,
            Scale = new Vector2(4, 5)
        };
        var transform2DComponent = Entity.CreateComponent<Transform2DComponent>();

        // Act
        transform2DComponent.SetTransformImmediate(transform2D);

        // Assert
        Assert.That(transform2DComponent.Translation, Is.EqualTo(transform2D.Translation));
        Assert.That(transform2DComponent.Rotation, Is.EqualTo(transform2D.Rotation));
        Assert.That(transform2DComponent.Scale, Is.EqualTo(transform2D.Scale));
    }

    [Test]
    public void ComputeWorldTransformMatrix_ShouldReturnWorldTransform_GivenDefaultTransformOnRootEntity()
    {
        // Arrange
        var transform2DComponent = Entity.CreateComponent<Transform2DComponent>();

        // Act
        var worldTransformMatrix = transform2DComponent.ComputeWorldTransformMatrix();

        // Assert
        Assert.That(worldTransformMatrix, Is.EqualTo(Matrix3x3.Identity));
    }

    [Test]
    public void ComputeWorldTransformMatrix_ShouldReturnWorldTransform_GivenNonDefaultTransformOnRootEntity()
    {
        // Arrange
        var transform2DComponent = Entity.CreateComponent<Transform2DComponent>();
        transform2DComponent.Translation = new Vector2(1, 2);
        transform2DComponent.Rotation = 3;
        transform2DComponent.Scale = new Vector2(4, 5);

        var expectedMatrix = transform2DComponent.ToMatrix();

        // Act
        var worldTransformMatrix = transform2DComponent.ComputeWorldTransformMatrix();

        // Assert
        Assert.That(worldTransformMatrix, Is.EqualTo(expectedMatrix));
    }

    [Test]
    public void ComputeWorldTransformMatrix_ShouldReturnWorldTransform_GivenNonDefaultTransformOnChildEntity_AndParentEntityWithNoTransform()
    {
        // Arrange
        var parent = Scene.CreateEntity();

        var child = parent.CreateChildEntity();
        var childTransform = child.CreateComponent<Transform2DComponent>();
        childTransform.Translation = new Vector2(1, 2);
        childTransform.Rotation = 3;
        childTransform.Scale = new Vector2(4, 5);

        var expectedMatrix = childTransform.ToMatrix();

        // Assume
        Assert.That(parent.HasComponent<Transform2DComponent>(), Is.False);

        // Act
        var worldTransformMatrix = childTransform.ComputeWorldTransformMatrix();

        // Assert
        Assert.That(worldTransformMatrix, Is.EqualTo(expectedMatrix));
    }

    [Test]
    public void ComputeWorldTransformMatrix_ShouldReturnWorldTransform_GivenNonDefaultTransformOnChildEntity_AndParentEntityWithNonDefaultTransform()
    {
        // Arrange
        var parent = Scene.CreateEntity();
        var parentTransform = parent.CreateComponent<Transform2DComponent>();
        parentTransform.Translation = new Vector2(1, 2);
        parentTransform.Rotation = 3;
        parentTransform.Scale = new Vector2(4, 5);

        var child = parent.CreateChildEntity();
        var childTransform = child.CreateComponent<Transform2DComponent>();
        childTransform.Translation = new Vector2(10, 20);
        childTransform.Rotation = 30;
        childTransform.Scale = new Vector2(40, 50);

        var expectedMatrix = parentTransform.ToMatrix() * childTransform.ToMatrix();

        // Act
        var worldTransformMatrix = childTransform.ComputeWorldTransformMatrix();

        // Assert
        Assert.That(worldTransformMatrix, Is.EqualTo(expectedMatrix));
    }

    [Test]
    public void ComputeWorldTransformMatrix_ShouldReturnWorldTransform_GivenThreeLevelHierarchyOfNonDefaultTransforms()
    {
        // Arrange
        var parent = Scene.CreateEntity();
        var parentTransform = parent.CreateComponent<Transform2DComponent>();
        parentTransform.Translation = new Vector2(1, 2);
        parentTransform.Rotation = 3;
        parentTransform.Scale = new Vector2(4, 5);

        var child = parent.CreateChildEntity();
        var childTransform = child.CreateComponent<Transform2DComponent>();
        childTransform.Translation = new Vector2(10, 20);
        childTransform.Rotation = 30;
        childTransform.Scale = new Vector2(40, 50);

        var grandChild = child.CreateChildEntity();
        var grandChildTransform = grandChild.CreateComponent<Transform2DComponent>();
        grandChildTransform.Translation = new Vector2(100, 200);
        grandChildTransform.Rotation = 300;
        grandChildTransform.Scale = new Vector2(400, 500);

        var expectedMatrix = parentTransform.ToMatrix() * childTransform.ToMatrix() * grandChildTransform.ToMatrix();

        // Act
        var worldTransformMatrix = grandChildTransform.ComputeWorldTransformMatrix();

        // Assert
        Assert.That(worldTransformMatrix, Is.EqualTo(expectedMatrix));
    }

    [Test]
    public void ComputeWorldTransformMatrix_ShouldReturnWorldTransform_GivenThreeLevelHierarchy_WhereMiddleEntityHasNoTransform()
    {
        // Arrange
        var parent = Scene.CreateEntity();
        var parentTransform = parent.CreateComponent<Transform2DComponent>();
        parentTransform.Translation = new Vector2(1, 2);
        parentTransform.Rotation = 3;
        parentTransform.Scale = new Vector2(4, 5);

        var child = parent.CreateChildEntity();

        var grandChild = child.CreateChildEntity();
        var grandChildTransform = grandChild.CreateComponent<Transform2DComponent>();
        grandChildTransform.Translation = new Vector2(10, 20);
        grandChildTransform.Rotation = 30;
        grandChildTransform.Scale = new Vector2(40, 50);

        var expectedMatrix = parentTransform.ToMatrix() * grandChildTransform.ToMatrix();

        // Assume
        Assert.That(child.HasComponent<Transform2DComponent>(), Is.False);

        // Act
        var worldTransformMatrix = grandChildTransform.ComputeWorldTransformMatrix();

        // Assert
        Assert.That(worldTransformMatrix, Is.EqualTo(expectedMatrix));
    }
}