using System;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Physics.Systems;

internal static class KinematicIntegrator
{
    public static void IntegrateKinematicMotion(PhysicsState physicsState, double deltaTimeSeconds)
    {
        var kinematicBodies = physicsState.GetKinematicBodies();

        for (var i = 0; i < kinematicBodies.Count; i++)
        {
            var kinematicBody = kinematicBodies[i];

            kinematicBody.Position += kinematicBody.LinearVelocity * deltaTimeSeconds;
            kinematicBody.Rotation += kinematicBody.AngularVelocity * deltaTimeSeconds;
        }
    }

    // TODO Research it further.
    public static void IntegrateKinematicMotionWithBasicVelocityConstraint(PhysicsState physicsState, double deltaTimeSeconds)
    {
        var kinematicBodies = physicsState.GetKinematicBodies();

        for (var i = 0; i < kinematicBodies.Count; i++)
        {
            var kinematicBody = kinematicBodies[i];

            var velocity = kinematicBody.LinearVelocity * deltaTimeSeconds;
            var constraintVelocity = Vector2.Zero;

            for (int j = 0; j < kinematicBody.Contacts.Count; j++)
            {
                var contact = kinematicBody.Contacts[j];
                var velocityAlongNormal = velocity.Dot(contact.Point1.CollisionNormal.Opposite);
                constraintVelocity += contact.Point1.CollisionNormal * Math.Max(velocityAlongNormal, 0);
            }

            var velocityOpposite = velocity.Opposite;
            var velocityNegation = new Vector2(Math.MinMagnitude(constraintVelocity.X, velocityOpposite.X),
                Math.MinMagnitude(constraintVelocity.Y, velocityOpposite.Y));
            var finalVelocity = velocity + velocityNegation * 0.95;

            kinematicBody.Position += finalVelocity;
            kinematicBody.Rotation += kinematicBody.AngularVelocity * deltaTimeSeconds;
        }
    }
}