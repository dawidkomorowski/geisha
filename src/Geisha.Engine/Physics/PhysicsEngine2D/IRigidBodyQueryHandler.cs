namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal interface IRigidBodyQueryHandler
{
    // TODO: Document contract - especially the return value meaning.
    bool Handle(RigidBody2D body);
}