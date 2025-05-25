using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Physics;
using Geisha.Engine.Physics.Components;
using NUnit.Framework;
using System;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

public class TweakingParametersTests : PhysicsSystemTestsBase
{
    [Test]
    public void Constructor_ShouldThrowException_GivenSubstepsBelow_1()
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            Substeps = 0
        };

        // Act & Assert
        Assert.That(() => GetPhysicsSystem(physicsConfiguration), Throws.ArgumentException);
    }

    [Test]
    public void Constructor_ShouldNotThrowException_GivenSubstepsAbove_0()
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            Substeps = 1
        };

        // Act & Assert
        Assert.That(() => GetPhysicsSystem(physicsConfiguration), Throws.Nothing);
    }

    [TestCase(1, 50)]
    [TestCase(3, -5)]
    public void IncreasingSubsteps_PreventsHighVelocityBodyFromTunneling(int substeps, double targetX)
    {
        // Arrange
        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
        var physicsConfiguration = new PhysicsConfiguration
        {
            Substeps = substeps,
            RenderCollisionGeometry = true
        };

        var physicsSystem = GetPhysicsSystem(physicsConfiguration);
        var kinematicBody = CreateRectangleKinematicBody(-10, 5, 10, 10);
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity = new Vector2(300, 0);
        CreateRectangleStaticBody(5, 5, 10, 10);

        // Act
        SaveVisualOutput(physicsSystem, 0, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 1, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 2, 10);

        // Assert
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(targetX, 5)));
    }
}