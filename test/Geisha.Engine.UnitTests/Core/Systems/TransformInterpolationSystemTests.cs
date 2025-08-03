using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using Geisha.TestUtils;
using NUnit.Framework;
using System.Collections.Generic;

namespace Geisha.Engine.UnitTests.Core.Systems;

public class TransformInterpolationSystemTests
{
    private const double Epsilon = 0.000001;
    private static IEqualityComparer<Vector2> Vector2Comparer => CommonEqualityComparer.Vector2(Epsilon);

    private TransformInterpolationSystem _transformInterpolationSystem = null!;
    private Scene _scene = null!;

    [SetUp]
    public void SetUp()
    {
        _transformInterpolationSystem = new TransformInterpolationSystem();
        _scene = TestSceneFactory.Create();
        _scene.AddObserver(_transformInterpolationSystem);
    }

    [Test]
    public void Transform2DComponent_ShouldNotBeInterpolated_WhenInterpolationIsDisabled()
    {
        // Arrange
        var entity = _scene.CreateEntity();
        var transformComponent = entity.CreateComponent<Transform2DComponent>();
        transformComponent.IsInterpolated = false;

        // Act
        transformComponent.Translation = new Vector2(10, 20);
        transformComponent.Rotation = 30;
        transformComponent.Scale = new Vector2(2, 3);

        _transformInterpolationSystem.SnapshotTransforms();

        transformComponent.Translation = new Vector2(20, 40);
        transformComponent.Rotation = 60;
        transformComponent.Scale = new Vector2(4, 6);

        _transformInterpolationSystem.SnapshotTransforms();

        _transformInterpolationSystem.InterpolateTransforms(0.5);

        // Assert
        Assert.That(transformComponent.InterpolatedTransform.Translation, Is.EqualTo(new Vector2(20, 40)));
        Assert.That(transformComponent.InterpolatedTransform.Rotation, Is.EqualTo(60));
        Assert.That(transformComponent.InterpolatedTransform.Scale, Is.EqualTo(new Vector2(4, 6)));
    }

    [Test]
    public void Transform2DComponent_ShouldBeInterpolated_ByFactor_0_5_WhenInterpolationIsEnabled()
    {
        // Arrange
        var entity = _scene.CreateEntity();
        var transformComponent = entity.CreateComponent<Transform2DComponent>();
        transformComponent.IsInterpolated = true;

        // Act
        transformComponent.Translation = new Vector2(10, 20);
        transformComponent.Rotation = 30;
        transformComponent.Scale = new Vector2(2, 3);

        _transformInterpolationSystem.SnapshotTransforms();

        transformComponent.Translation = new Vector2(20, 40);
        transformComponent.Rotation = 60;
        transformComponent.Scale = new Vector2(4, 6);

        _transformInterpolationSystem.SnapshotTransforms();

        _transformInterpolationSystem.InterpolateTransforms(0.5);

        // Assert
        Assert.That(transformComponent.InterpolatedTransform.Translation, Is.EqualTo(new Vector2(15, 30)).Using(Vector2Comparer));
        Assert.That(transformComponent.InterpolatedTransform.Rotation, Is.EqualTo(45).Within(Epsilon));
        Assert.That(transformComponent.InterpolatedTransform.Scale, Is.EqualTo(new Vector2(3, 4.5)).Using(Vector2Comparer));
    }

    [Test]
    public void Transform2DComponent_ShouldBeInterpolated_ByFactor_0_25_WhenInterpolationIsEnabled()
    {
        // Arrange
        var entity = _scene.CreateEntity();
        var transformComponent = entity.CreateComponent<Transform2DComponent>();
        transformComponent.IsInterpolated = true;

        // Act
        transformComponent.Translation = new Vector2(10, 20);
        transformComponent.Rotation = 30;
        transformComponent.Scale = new Vector2(2, 3);

        _transformInterpolationSystem.SnapshotTransforms();

        transformComponent.Translation = new Vector2(20, 40);
        transformComponent.Rotation = 60;
        transformComponent.Scale = new Vector2(4, 6);

        _transformInterpolationSystem.SnapshotTransforms();

        _transformInterpolationSystem.InterpolateTransforms(0.25);

        // Assert
        Assert.That(transformComponent.InterpolatedTransform.Translation, Is.EqualTo(new Vector2(12.5, 25)).Using(Vector2Comparer));
        Assert.That(transformComponent.InterpolatedTransform.Rotation, Is.EqualTo(37.5).Within(Epsilon));
        Assert.That(transformComponent.InterpolatedTransform.Scale, Is.EqualTo(new Vector2(2.5, 3.75)).Using(Vector2Comparer));
    }

    [Test]
    public void Transform2DComponent_ShouldBeInterpolated_WhenInterpolationIsEnabled_BeforeTransformInterpolationSystemIsAddedAsSceneObserver()
    {
        // Arrange
        var entity = _scene.CreateEntity();
        _scene.RemoveObserver(_transformInterpolationSystem);
        var transformComponent = entity.CreateComponent<Transform2DComponent>();
        transformComponent.IsInterpolated = true;
        _scene.AddObserver(_transformInterpolationSystem);

        // Act
        transformComponent.Translation = new Vector2(10, 20);
        transformComponent.Rotation = 30;
        transformComponent.Scale = new Vector2(2, 3);

        _transformInterpolationSystem.SnapshotTransforms();

        transformComponent.Translation = new Vector2(20, 40);
        transformComponent.Rotation = 60;
        transformComponent.Scale = new Vector2(4, 6);

        _transformInterpolationSystem.SnapshotTransforms();

        _transformInterpolationSystem.InterpolateTransforms(0.5);

        // Assert
        Assert.That(transformComponent.InterpolatedTransform.Translation, Is.EqualTo(new Vector2(15, 30)).Using(Vector2Comparer));
        Assert.That(transformComponent.InterpolatedTransform.Rotation, Is.EqualTo(45).Within(Epsilon));
        Assert.That(transformComponent.InterpolatedTransform.Scale, Is.EqualTo(new Vector2(3, 4.5)).Using(Vector2Comparer));
    }

    [Test]
    public void Transform2DComponent_SetTransformImmediate_ShouldImmediatelySetInterpolatedTransformToNewValue()
    {
        // Arrange
        var entity = _scene.CreateEntity();
        var transformComponent = entity.CreateComponent<Transform2DComponent>();
        transformComponent.IsInterpolated = true;

        // Act
        transformComponent.Translation = new Vector2(10, 20);
        transformComponent.Rotation = 30;
        transformComponent.Scale = new Vector2(2, 3);

        _transformInterpolationSystem.SnapshotTransforms();

        var transform = new Transform2D(new Vector2(20, 40), 60, new Vector2(4, 6));
        transformComponent.SetTransformImmediate(transform);

        _transformInterpolationSystem.SnapshotTransforms();

        _transformInterpolationSystem.InterpolateTransforms(0.5);

        // Assert
        Assert.That(transformComponent.InterpolatedTransform.Translation, Is.EqualTo(new Vector2(20, 40)));
        Assert.That(transformComponent.InterpolatedTransform.Rotation, Is.EqualTo(60));
        Assert.That(transformComponent.InterpolatedTransform.Scale, Is.EqualTo(new Vector2(4, 6)));
    }

    [Test]
    public void Transform2DComponent_IsInterpolated_ShouldUpdateTransformInterpolationSystemState()
    {
        // Arrange
        var entity = _scene.CreateEntity();
        var transformComponent = entity.CreateComponent<Transform2DComponent>();
        // Act & Assert
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent), Is.False);
        transformComponent.IsInterpolated = true;
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent), Is.True);
        transformComponent.IsInterpolated = false;
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent), Is.False);
        transformComponent.IsInterpolated = true;
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent), Is.True);
        transformComponent.IsInterpolated = false;
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent), Is.False);
        transformComponent.IsInterpolated = true;
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent), Is.True);
        transformComponent.IsInterpolated = true;
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent), Is.True);
        transformComponent.IsInterpolated = true;
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent), Is.True);
        transformComponent.IsInterpolated = false;
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent), Is.False);
        transformComponent.IsInterpolated = false;
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent), Is.False);
        transformComponent.IsInterpolated = false;
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent), Is.False);
        transformComponent.IsInterpolated = true;
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent), Is.True);
    }

    [Test]
    public void AddingAndRemovingTransformInterpolationSystemAsSceneObserver_ShouldUpdateSystemState()
    {
        // Arrange
        var entity = _scene.CreateEntity();
        var transformComponent = entity.CreateComponent<Transform2DComponent>();

        // Act & Assert
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent), Is.False);
        transformComponent.IsInterpolated = true;
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent), Is.True);
        _scene.RemoveObserver(_transformInterpolationSystem);
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent), Is.False);
        transformComponent.IsInterpolated = false;
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent), Is.False);
        _scene.AddObserver(_transformInterpolationSystem);
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent), Is.False);
        transformComponent.IsInterpolated = true;
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent), Is.True);
        _scene.RemoveObserver(_transformInterpolationSystem);
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent), Is.False);
        _scene.AddObserver(_transformInterpolationSystem);
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent), Is.True);
        transformComponent.IsInterpolated = true;
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent), Is.True);
        transformComponent.IsInterpolated = false;
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent), Is.False);
        _scene.RemoveObserver(_transformInterpolationSystem);
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent), Is.False);
        transformComponent.IsInterpolated = true;
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent), Is.False);
        _scene.AddObserver(_transformInterpolationSystem);
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent), Is.True);
    }

    [Test]
    public void RemovingTransform2DComponent_ShouldUpdateTransformInterpolationSystemState()
    {
        // Arrange
        var entity = _scene.CreateEntity();
        var transformComponent = entity.CreateComponent<Transform2DComponent>();
        transformComponent.IsInterpolated = true;

        // Act
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent), Is.True);
        entity.RemoveComponent(transformComponent);

        // Assert
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent), Is.False);
    }

    [Test]
    public void RemovingMultipleTransform2DComponents_ShouldUpdateTransformInterpolationSystemState()
    {
        // Arrange
        var entity1 = _scene.CreateEntity();
        var transformComponent1 = entity1.CreateComponent<Transform2DComponent>();
        transformComponent1.IsInterpolated = true;
        var entity2 = _scene.CreateEntity();
        var transformComponent2 = entity2.CreateComponent<Transform2DComponent>();
        transformComponent2.IsInterpolated = true;

        // Act & Assert
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent1), Is.True);
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent2), Is.True);
        entity1.RemoveComponent(transformComponent2);
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent1), Is.True);
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent2), Is.False);
        entity2.RemoveComponent(transformComponent1);
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent1), Is.False);
        Assert.That(_transformInterpolationSystem.HasTransformData(transformComponent2), Is.False);
    }

    [Test]
    public void TODO_TestInterpolationOfMultipleTransformsOnceSomeOfThemWereRemovedOrDisabledSoInternalIdsAreUpdatedCorrectly()
    {
        Assert.Fail("TODO");
    }
}