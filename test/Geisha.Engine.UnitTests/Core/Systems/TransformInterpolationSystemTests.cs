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
    public void Transform2D_ShouldBeInterpolated_WhenInterpolationIsEnabled()
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
}