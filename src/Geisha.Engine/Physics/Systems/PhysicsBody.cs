using Geisha.Common.Math;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;

namespace Geisha.Engine.Physics.Systems
{
    internal sealed class PhysicsBody
    {
        public PhysicsBody(Transform2DComponent transform, Collider2DComponent collider)
        {
            Transform = transform;
            Collider = collider;
        }

        public Entity Entity => Transform.Entity;
        public Transform2DComponent Transform { get; }
        public Collider2DComponent Collider { get; }
        public Matrix3x3 FinalTransform { get; private set; }

        public void UpdateFinalTransform()
        {
            FinalTransform = TransformHierarchy.Calculate2DTransformationMatrix(Entity);
        }
    }
}