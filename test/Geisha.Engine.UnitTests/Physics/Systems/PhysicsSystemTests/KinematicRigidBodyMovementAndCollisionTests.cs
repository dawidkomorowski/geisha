using System;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics.Components;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

[TestFixture]
public class KinematicRigidBodyMovementAndCollisionTests : PhysicsSystemTestsBase
{
    [Test]
    public void ProcessPhysics_KinematicBodyShouldMoveWithLinearVelocityAndStartCollidingWithStaticBody()
    {
        // Arrange
        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);

        var physicsSystem = GetPhysicsSystem();
        var staticBody = CreateRectangleStaticBody(50, 0, 10, 10);
        var kinematicBody = CreateRectangleKinematicBody(0, 0, 10, 10);
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity = new Vector2(20, 0);

        Assert.That(staticBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assume.That(kinematicBody.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(Vector2.Zero));
        Assert.That(kinematicBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

        // Act
        for (var i = 0; i < 19; i++)
        {
            physicsSystem.ProcessPhysics();

            Assert.That(staticBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
            Assert.That(kinematicBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        }

        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(staticBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(40, 0)));
        Assert.That(kinematicBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);
    }
}