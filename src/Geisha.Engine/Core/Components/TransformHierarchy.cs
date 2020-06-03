using Geisha.Common.Math;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Components
{
    internal static class TransformHierarchy
    {
        public static Matrix3x3 CalculateTransformationMatrix(Entity entity)
        {
            // TODO If entity does not have transform should it crash or should identity be used instead?
            var transform = entity.GetComponent<TransformComponent>().Create2DTransformationMatrix();
            if (entity.IsRoot)
            {
                return transform;
            }
            else
            {
                return CalculateTransformationMatrix(entity.Parent!) * transform;
            }
        }
    }
}