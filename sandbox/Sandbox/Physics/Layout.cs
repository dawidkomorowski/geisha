using Geisha.Engine.Core.SceneModel;
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
        const int horizontalBlocks = 33;
        const int verticalBlocks = 19;
        for (var x = 0; x < horizontalBlocks; x++)
        {
            for (var y = 0; y < verticalBlocks; y++)
            {
                if (x == 0 || x == horizontalBlocks - 1 || y == 0 || y == verticalBlocks - 1)
                {
                    PhysicsEntityFactory.CreateTileStaticBody(scene, x * 50 - 800, y * 50 - 450);
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

        // Tile cluster with central tile with CollisionNormalFilter set to None.
        PhysicsEntityFactory.CreateTileStaticBody(scene, -550, 350);
        PhysicsEntityFactory.CreateTileStaticBody(scene, -500, 350);
        PhysicsEntityFactory.CreateTileStaticBody(scene, -450, 350);
        PhysicsEntityFactory.CreateTileStaticBody(scene, -550, 300);
        PhysicsEntityFactory.CreateTileStaticBody(scene, -500, 300);
        PhysicsEntityFactory.CreateTileStaticBody(scene, -450, 300);
        PhysicsEntityFactory.CreateTileStaticBody(scene, -550, 250);
        PhysicsEntityFactory.CreateTileStaticBody(scene, -500, 250);
        PhysicsEntityFactory.CreateTileStaticBody(scene, -450, 250);
    }
}