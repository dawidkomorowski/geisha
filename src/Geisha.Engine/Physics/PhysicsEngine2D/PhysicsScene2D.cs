using System;
using System.Collections.Generic;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal sealed class PhysicsScene2D
{
    private readonly List<RigidBody2D> _bodies = new();
    private readonly List<RigidBody2D> _staticBodies = new();
    private readonly List<RigidBody2D> _kinematicBodies = new();

    public IReadOnlyList<RigidBody2D> Bodies => _bodies;

    public RigidBody2D CreateBody(BodyType bodyType, Circle circleCollider)
    {
        var body = new RigidBody2D(this, bodyType, circleCollider);
        AddBodyToScene(body);
        return body;
    }

    public RigidBody2D CreateBody(BodyType bodyType, AxisAlignedRectangle rectangleCollider)
    {
        var body = new RigidBody2D(this, bodyType, rectangleCollider);
        AddBodyToScene(body);
        return body;
    }

    public void RemoveBody(RigidBody2D body)
    {
        switch (body.Type)
        {
            case BodyType.Static:
                _bodies.Remove(body);
                _staticBodies.Remove(body);
                break;
            case BodyType.Kinematic:
                _bodies.Remove(body);
                _kinematicBodies.Remove(body);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Simulate(TimeSpan timeStep)
    {
        // TODO Make this configurable.
        const int subSteps = 1;
        var subStepTime = timeStep.TotalSeconds / subSteps;

        for (var s = 0; s < subSteps; s++)
        {
            // TODO Consider adding minimum velocity threshold to avoid solving constraints for very small velocities.
            ContactSolver.SolveVelocityConstraints(_kinematicBodies);

            KinematicIntegration.IntegrateKinematicMotion(_kinematicBodies, subStepTime);

            foreach (var kinematicBody in _kinematicBodies)
            {
                kinematicBody.RecomputeCollider();
            }

            CollisionDetection.DetectCollisions(_staticBodies, _kinematicBodies);

            // TODO Make this more general and configurable.
            FilterTileGhostCollisions(_kinematicBodies);

            // TODO Constant of 6 is how many times position constraints are iteratively solved. It is arbitrary value.
            // TODO It may require further research to find optimal value. Also, it may require to be configurable.
            // TODO SolvePositionConstraints could return a boolean value indicating whether the position constraints were solved. Then further iterations could be stopped.
            // TODO Research it further when working on https://github.com/dawidkomorowski/geisha/issues/324.
            for (var i = 0; i < 6; i++)
            {
                ContactSolver.SolvePositionConstraints(_kinematicBodies);
            }
        }
    }

    private void AddBodyToScene(RigidBody2D body)
    {
        switch (body.Type)
        {
            case BodyType.Static:
                _bodies.Add(body);
                _staticBodies.Add(body);
                break;
            case BodyType.Kinematic:
                _bodies.Add(body);
                _kinematicBodies.Add(body);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static void FilterTileGhostCollisions(IReadOnlyList<RigidBody2D> kinematicBodies)
    {
        for (var i = 0; i < kinematicBodies.Count; i++)
        {
            var kinematicBody = kinematicBodies[i];

            if (kinematicBody.EnableCollisionResponse is false)
            {
                continue;
            }

            if (kinematicBody.Contacts.Count <= 1)
            {
                continue;
            }

            FilterContacts(kinematicBody);
        }
    }

    private static void FilterContacts(RigidBody2D kinematicBody)
    {
        var tileSize = 100d; // TODO This is a placeholder value. It should be replaced with the actual tile size from configuration.

        Span<int> contactsToRemove = stackalloc int[kinematicBody.Contacts.Count];
        var contactsToRemoveCount = 0;

        for (var j = 0; j < kinematicBody.Contacts.Count; j++)
        {
            var contact1 = kinematicBody.Contacts[j];
            if (contact1.Body1.Type is BodyType.Kinematic && contact1.Body2.Type is BodyType.Kinematic)
            {
                continue;
            }

            var otherBody1 = contact1.Body1 == kinematicBody ? contact1.Body2 : contact1.Body1;

            for (var k = j + 1; k < kinematicBody.Contacts.Count; k++)
            {
                var contact2 = kinematicBody.Contacts[k];
                var otherBody2 = contact2.Body1 == kinematicBody ? contact2.Body2 : contact2.Body1;

                if (otherBody2.Position == otherBody1.Position + new Vector2(tileSize, 0))
                {
                    var dot1 = contact1.CollisionNormal.Dot(Vector2.UnitY);
                    var dot2 = contact2.CollisionNormal.Dot(Vector2.UnitY);

                    if (dot1 == 0 && dot2 != 0)
                    {
                        contactsToRemove[contactsToRemoveCount++] = j;
                    }

                    if (dot1 != 0 && dot2 == 0)
                    {
                        contactsToRemove[contactsToRemoveCount++] = k;
                    }
                }

                if (otherBody2.Position == otherBody1.Position + new Vector2(0, tileSize))
                {
                    var dot1 = contact1.CollisionNormal.Dot(Vector2.UnitX);
                    var dot2 = contact2.CollisionNormal.Dot(Vector2.UnitX);
                    if (dot1 == 0 && dot2 != 0)
                    {
                        contactsToRemove[contactsToRemoveCount++] = j;
                    }

                    if (dot1 != 0 && dot2 == 0)
                    {
                        contactsToRemove[contactsToRemoveCount++] = k;
                    }
                }
            }
        }

        for (var j = contactsToRemoveCount - 1; j >= 0; j--)
        {
            var indexToRemove = contactsToRemove[j];
            kinematicBody.Contacts.RemoveAt(indexToRemove);
        }
    }
}