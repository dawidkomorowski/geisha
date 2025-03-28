using Geisha.Engine.Core.SceneModel;

namespace Sandbox.Physics;

public static class Layout
{
    public static void RectangleColliders(Scene scene)
    {
        PhysicsEntityFactory.CreateRectangleStaticBody(scene, 0, -200, 100, 100);
        PhysicsEntityFactory.CreateRectangleStaticBody(scene, -300, -300, 200, 100);
        PhysicsEntityFactory.CreateRectangleStaticBody(scene, -600, -300, 50, 100);
        PhysicsEntityFactory.CreateRectangleStaticBody(scene, -200, 300, 100, 100);
        PhysicsEntityFactory.CreateRectangleStaticBody(scene, -300, 200, 100, 100);
        PhysicsEntityFactory.CreateRectangleStaticBody(scene, 200, -300, 100, 100);
        PhysicsEntityFactory.CreateRectangleStaticBody(scene, 300, -300, 100, 100);
        PhysicsEntityFactory.CreateRectangleStaticBody(scene, 400, -300, 100, 100);
    }

    public static void CircleColliders(Scene scene)
    {
        PhysicsEntityFactory.CreateCircleStaticBody(scene, 200, 0, 50);
        PhysicsEntityFactory.CreateCircleStaticBody(scene, 350, 0, 50);
        PhysicsEntityFactory.CreateCircleStaticBody(scene, 450, 0, 50);
    }

    public static void KinematicBodies(Scene scene)
    {
        PhysicsEntityFactory.CreateRectangleKinematicBody(scene, 0, 300, 100, 100);
    }
}