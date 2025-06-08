using System;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics.Components;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

[TestFixture]
public class KinematicBodyMovementTests : PhysicsSystemTestsBase
{
    [TestCase(20, 0, 0.1, 2, 0)]
    [TestCase(0, 20, 0.1, 0, 2)]
    [TestCase(20, 20, 0.1, 2, 2)]
    [TestCase(20, 20, 0.2, 4, 4)]
    public void ProcessPhysics_ShouldUpdatePosition_WhenKinematicBodyHasLinearVelocity(double vx, double vy, double dt, double tx, double ty)
    {
        // Arrange
        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(dt);

        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(0, 0, 10, 10);
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity = new Vector2(vx, vy);

        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(Vector2.Zero));

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(tx, ty)));
    }

    [TestCase(Math.PI, 0.1, Math.PI / 10)]
    [TestCase(-Math.PI, 0.1, -Math.PI / 10)]
    [TestCase(Math.PI, 0.2, Math.PI / 5)]
    public void ProcessPhysics_ShouldUpdateRotation_WhenKinematicBodyHasAngularVelocity(double vr, double dt, double r)
    {
        // Arrange
        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(dt);

        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(0, 0, 10, 10);
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().AngularVelocity = vr;

        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Rotation, Is.Zero);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(r));
    }

    [Test]
    public void ProcessPhysics_ShouldUpdatePositionAndRotation_WhenKinematicBodyHasLinearVelocityAndAngularVelocity()
    {
        // Arrange
        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);

        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(0, 0, 10, 10);
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity = new Vector2(20, 10);
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().AngularVelocity = Math.PI / 2;

        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(Vector2.Zero));
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Rotation, Is.Zero);

        // Act
        physicsSystem.ProcessPhysics();
        physicsSystem.ProcessPhysics();
        physicsSystem.ProcessPhysics();
        physicsSystem.ProcessPhysics();
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(10, 5)));
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(Math.PI / 4));
    }
}