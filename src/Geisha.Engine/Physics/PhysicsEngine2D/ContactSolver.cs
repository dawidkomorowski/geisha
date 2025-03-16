using System;
using System.Collections.Generic;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal static class ContactSolver
{
    public static void SolvePositionConstraints(IReadOnlyList<RigidBody2D> kinematicBodies)
    {
        for (var i = 0; i < kinematicBodies.Count; i++)
        {
            var kinematicBody = kinematicBodies[i];

            var minimumTranslationVector = Vector2.Zero;

            for (int j = 0; j < kinematicBody.Contacts.Count; j++)
            {
                var contact = kinematicBody.Contacts[j];

                var mtv = PositionConstraint.GetMinimumTranslationVector(contact);

                if (mtv.Length <= 0.5)
                {
                    continue;
                }

                var localMtv = mtv.Direction * mtv.Length;
                if (localMtv.X * minimumTranslationVector.X > 0)
                {
                    if (Math.Abs(localMtv.X) > Math.Abs(minimumTranslationVector.X))
                    {
                        minimumTranslationVector = minimumTranslationVector.WithX(localMtv.X);
                    }
                }
                else
                {
                    minimumTranslationVector = minimumTranslationVector.WithX(minimumTranslationVector.X + localMtv.X);
                }

                if (localMtv.Y * minimumTranslationVector.Y > 0)
                {
                    if (Math.Abs(localMtv.Y) > Math.Abs(minimumTranslationVector.Y))
                    {
                        minimumTranslationVector = minimumTranslationVector.WithY(localMtv.Y);
                    }
                }
                else
                {
                    minimumTranslationVector = minimumTranslationVector.WithY(minimumTranslationVector.Y + localMtv.Y);
                }
            }

            kinematicBody.Position += minimumTranslationVector * 0.9;
            kinematicBody.RecomputeCollider();
        }
    }
}