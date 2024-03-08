using System;
using System.Collections.Generic;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal static class KinematicIntegrator
{
    public static void IntegrateKinematicMotion(IReadOnlyList<RigidBody2D> bodies, double deltaTimeSeconds)
    {
        for (var i = 0; i < bodies.Count; i++)
        {
            var body = bodies[i];

            body.Position += body.LinearVelocity * deltaTimeSeconds;
            body.Rotation += body.AngularVelocity * deltaTimeSeconds;
        }
    }

    // TODO Research it further.
    public static void IntegrateKinematicMotionWithBasicVelocityConstraint(IReadOnlyList<RigidBody2D> bodies, double deltaTimeSeconds)
    {
        for (var i = 0; i < bodies.Count; i++)
        {
            var body = bodies[i];

            var velocity = body.LinearVelocity * deltaTimeSeconds;
            var constraintVelocity = Vector2.Zero;

            for (int j = 0; j < body.Contacts.Count; j++)
            {
                var contact = body.Contacts[j];
                var velocityAlongNormal = velocity.Dot(contact.Point1.CollisionNormal.Opposite);
                constraintVelocity += contact.Point1.CollisionNormal * Math.Max(velocityAlongNormal, 0);
            }

            var velocityOpposite = velocity.Opposite;
            var velocityNegation = new Vector2(Math.MinMagnitude(constraintVelocity.X, velocityOpposite.X),
                Math.MinMagnitude(constraintVelocity.Y, velocityOpposite.Y));
            var finalVelocity = velocity + velocityNegation * 0.95;

            body.Position += finalVelocity;
            body.Rotation += body.AngularVelocity * deltaTimeSeconds;
        }
    }
}