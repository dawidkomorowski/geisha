using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Engine.Rendering.Systems
{
    internal sealed class CameraNode
    {
        public CameraNode(Transform2DComponent transform, CameraComponent camera)
        {
            Transform = transform;
            Camera = camera;
        }

        public Entity Entity => Transform.Entity;
        public Transform2DComponent Transform { get; }
        public CameraComponent Camera { get; }

        public AxisAlignedRectangle GetBoundingRectangleOfView()
        {
            var transform = TransformHierarchy.Calculate2DTransformationMatrix(Entity);
            var quad = new AxisAlignedRectangle(Camera.ViewRectangle).ToQuad();
            return quad.Transform(transform).GetBoundingRectangle();
        }
    }
}