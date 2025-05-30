﻿using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;

namespace Sandbox.Physics;

public static class Layout
{
    public static void RectangleColliders(Scene scene)
    {
        PhysicsEntityFactory.CreateRectangleStaticBody(scene, 0, -200, 100, 100);
        PhysicsEntityFactory.CreateRectangleStaticBody(scene, -300, -300, 200, 100);
        PhysicsEntityFactory.CreateRectangleStaticBody(scene, -600, -300, 50, 100);

        PhysicsEntityFactory.CreateRectangleStaticBody(scene, -500, 200, 100, 100);
        PhysicsEntityFactory.CreateRectangleStaticBody(scene, -600, 100, 100, 100);
        PhysicsEntityFactory.CreateRectangleStaticBody(scene, -300, 200, 100, 100);


        PhysicsEntityFactory.CreateRectangleStaticBody(scene, 300, -300, 100, 100);
        PhysicsEntityFactory.CreateRectangleStaticBody(scene, 400, -300, 100, 100);
        PhysicsEntityFactory.CreateRectangleStaticBody(scene, 500, -300, 100, 100);
        PhysicsEntityFactory.CreateRectangleStaticBody(scene, 600, -300, 100, 100);

        PhysicsEntityFactory.CreateRectangleStaticBody(scene, -100, 200, 12.5, 12.5);
        PhysicsEntityFactory.CreateRectangleStaticBody(scene, 100, 200, 25, 25);
        PhysicsEntityFactory.CreateRectangleStaticBody(scene, 300, 200, 50, 50);
        PhysicsEntityFactory.CreateRectangleStaticBody(scene, 600, 200, 200, 200);
    }

    public static void CircleColliders(Scene scene)
    {
        PhysicsEntityFactory.CreateCircleStaticBody(scene, 0, -200, 50);
        PhysicsEntityFactory.CreateCircleStaticBody(scene, 200, -200, 50);
        PhysicsEntityFactory.CreateCircleStaticBody(scene, 350, -200, 50);
        PhysicsEntityFactory.CreateCircleStaticBody(scene, 450, -200, 50);

        PhysicsEntityFactory.CreateCircleStaticBody(scene, -100, 200, 6.25);
        PhysicsEntityFactory.CreateCircleStaticBody(scene, 100, 200, 12.5);
        PhysicsEntityFactory.CreateCircleStaticBody(scene, 300, 200, 25);
        PhysicsEntityFactory.CreateCircleStaticBody(scene, 600, 200, 100);
    }

    public static void KinematicBodies(Scene scene)
    {
        PhysicsEntityFactory.CreateRectangleKinematicBody(scene, 300, 200, 100, 100);
        PhysicsEntityFactory.CreateCircleKinematicBody(scene, -300, 200, 50);

        PhysicsEntityFactory.CreateRectangleKinematicBody(scene, 300, -200, 100, 100)
            .GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
        PhysicsEntityFactory.CreateCircleKinematicBody(scene, -300, -200, 50)
            .GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
    }

    public static void PlatformLevel(Scene scene)
    {
        for (var x = 0; x < 17; x++)
        {
            for (var y = 0; y < 10; y++)
            {
                if (x == 0 || x == 16 || y == 0 || y == 9)
                {
                    PhysicsEntityFactory.CreateRectangleStaticBody(scene, x * 100 - 800, y * 100 - 450, 100, 100);
                }
            }
        }

        PhysicsEntityFactory.CreateRectangleStaticBody(scene, -500, -200, 200, 50);
        PhysicsEntityFactory.CreateRectangleStaticBody(scene, -500, 0, 200, 50);
        PhysicsEntityFactory.CreateRectangleStaticBody(scene, -100, -100, 100, 50);
        PhysicsEntityFactory.CreateRectangleStaticBody(scene, 200, 200, 200, 50);
        PhysicsEntityFactory.CreateRectangleStaticBody(scene, 400, 0, 50, 50);
        PhysicsEntityFactory.CreateRectangleStaticBody(scene, 500, -100, 50, 50);
        PhysicsEntityFactory.CreateRectangleStaticBody(scene, 600, 300, 50, 50);
    }
}