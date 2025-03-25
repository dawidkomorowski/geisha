using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;

namespace Sandbox.Physics;

public static class PhysicsEntityFactory
{
    public static Entity CreateRectangleStaticBody(Scene scene, double x, double y, double w, double h)
    {
        var entity = scene.CreateEntity();
        var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
        transform2DComponent.Translation = new Vector2(x, y);
        var rectangleColliderComponent = entity.CreateComponent<RectangleColliderComponent>();
        rectangleColliderComponent.Dimensions = new Vector2(w, h);

        return entity;
    }

    public static Entity CreateRectangleKinematicBody(Scene scene, double x, double y, double w, double h)
    {
        var entity = CreateRectangleStaticBody(scene, x, y, w, h);
        entity.CreateComponent<KinematicRigidBody2DComponent>();

        return entity;
    }

    public static Entity CreateCircleStaticBody(Scene scene, double x, double y, double r)
    {
        var entity = scene.CreateEntity();
        var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
        transform2DComponent.Translation = new Vector2(x, y);
        var rectangleColliderComponent = entity.CreateComponent<CircleColliderComponent>();
        rectangleColliderComponent.Radius = r;

        return entity;
    }

    public static Entity CreateCircleKinematicBody(Scene scene, double x, double y, double r)
    {
        var entity = CreateCircleStaticBody(scene, x, y, r);
        entity.CreateComponent<KinematicRigidBody2DComponent>();

        return entity;
    }
}