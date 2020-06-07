using Geisha.Common.Math;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Components
{
    public static class TransformHierarchy
    {
        public static Matrix3x3 Calculate2DTransformationMatrix(Entity entity)
        {
            // TODO If entity does not have transform should it crash or should identity be used instead?
            var transform = entity.GetComponent<Transform2DComponent>().ToMatrix();
            if (entity.IsRoot)
            {
                return transform;
            }
            else
            {
                return Calculate2DTransformationMatrix(entity.Parent!) * transform;
            }
        }
    }
}