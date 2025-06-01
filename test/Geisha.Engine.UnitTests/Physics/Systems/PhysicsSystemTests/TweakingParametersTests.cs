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
    public void SubstepsTest_Constructor_ShouldThrowException_GivenSubstepsBelow_1()
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
    public void SubstepsTest_Constructor_ShouldNotThrowException_GivenSubstepsAbove_0()
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
    public void SubstepsTest_IncreasingSubsteps_PreventsHighVelocityBodyFromTunneling(int substeps, double targetX)
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

    [Test]
    public void VelocityIterationsTest_Constructor_ShouldThrowException_GivenVelocityIterationsBelow_1()
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            VelocityIterations = 0
        };

        // Act & Assert
        Assert.That(() => GetPhysicsSystem(physicsConfiguration), Throws.ArgumentException);
    }

    [Test]
    public void VelocityIterationsTest_Constructor_ShouldNotThrowException_GivenVelocityIterationsAbove_0()
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            VelocityIterations = 1
        };
        // Act & Assert
        Assert.That(() => GetPhysicsSystem(physicsConfiguration), Throws.Nothing);
    }

    [TestCase(1, 37.5)]
    [TestCase(2, 33.59375)]
    [TestCase(3, 25.048828125)]
    public void VelocityIterationsTest_IncreasingVelocityIterations_MakesVelocitiesOfBodiesMoreAccurate(int velocityIterations, double expectedVelocityX)
    {
        // Arrange
        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
        var physicsConfiguration = new PhysicsConfiguration
        {
            VelocityIterations = velocityIterations,
            RenderCollisionGeometry = true
        };

        // Scenario: Kinematic body is moving towards two other kinematic bodies and one static body.
        // Once first body hits another one, all three kinematic bodies should eventually hit the static body and stop moving.
        // Perfect solution for first kinematic body is velocity of 0.
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);
        var kinematicBody = CreateRectangleKinematicBody(-10, 5, 10, 10);
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity = new Vector2(100, 0);
        CreateRectangleKinematicBody(5, 5, 10, 10).GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
        CreateRectangleKinematicBody(20, 5, 10, 10).GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
        CreateRectangleStaticBody(31, 5, 10, 10);

        // Act
        SaveVisualOutput(physicsSystem, 0, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 1, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 2, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 3, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 4, 10);

        // Assert
        Assert.That(kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity.X, Is.EqualTo(expectedVelocityX));
    }

    [Test]
    public void PositionIterationsTest_Constructor_ShouldThrowException_GivenPositionIterationsBelow_1()
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            PositionIterations = 0
        };

        // Act & Assert
        Assert.That(() => GetPhysicsSystem(physicsConfiguration), Throws.ArgumentException);
    }

    [Test]
    public void PositionIterationsTest_Constructor_ShouldNotThrowException_GivenPositionIterationsAbove_0()
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            PositionIterations = 1
        };

        // Act & Assert
        Assert.That(() => GetPhysicsSystem(physicsConfiguration), Throws.Nothing);
    }

    [TestCase(1, -2.064582, 7.959936)]
    [TestCase(2, -0.516145, 10.641907)]
    [TestCase(3, -0.129036, 11.312400)]
    public void PositionIterationsTest_IncreasingPositionIterations_MakesPositionOfBodiesMoreAccurate(int positionIterations, double expectedPositionX,
        double expectedPositionY)
    {
        // Arrange
        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
        var physicsConfiguration = new PhysicsConfiguration
        {
            PositionIterations = positionIterations,
            RenderCollisionGeometry = true
        };

        var physicsSystem = GetPhysicsSystem(physicsConfiguration);
        var kinematicBody = CreateRectangleKinematicBody(0, 2, 10, 10);
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
        CreateRectangleStaticBody(-7, 0, 10, 10, Angle.Deg2Rad(30));
        CreateRectangleStaticBody(7, 0, 10, 10, Angle.Deg2Rad(-30));

        // Act
        SaveVisualOutput(physicsSystem, 0, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 1, 10);

        // Assert
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Translation.X, Is.EqualTo(expectedPositionX).Within(Epsilon));
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Translation.Y, Is.EqualTo(expectedPositionY).Within(Epsilon));
    }

    [Test]
    public void CollisionToleranceTest_Constructor_ShouldThrowException_GivenCollisionToleranceBelow_0()
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            CollisionTolerance = -0.1
        };

        // Act & Assert
        Assert.That(() => GetPhysicsSystem(physicsConfiguration), Throws.ArgumentException);
    }

    [Test]
    public void CollisionToleranceTest_Constructor_ShouldNotThrowException_GivenCollisionToleranceEqualOrAbove_0()
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            CollisionTolerance = 0d
        };

        // Act & Assert
        Assert.That(() => GetPhysicsSystem(physicsConfiguration), Throws.Nothing);
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    public void CollisionToleranceTest_IncreasingCollisionTolerance_PermitsBiggerPenetration(double collisionTolerance)
    {
        // Arrange
        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
        var physicsConfiguration = new PhysicsConfiguration
        {
            CollisionTolerance = collisionTolerance,
            RenderCollisionGeometry = true
        };

        var physicsSystem = GetPhysicsSystem(physicsConfiguration);
        var kinematicBody = CreateRectangleKinematicBody(0, 5, 10, 10);
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
        CreateRectangleStaticBody(5, 5, 10, 10);

        // Act
        SaveVisualOutput(physicsSystem, 0, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 1, 10);

        // Assert
        const double zeroToleranceSolution = -5d;
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Translation.X, Is.EqualTo(zeroToleranceSolution + collisionTolerance));
    }
}