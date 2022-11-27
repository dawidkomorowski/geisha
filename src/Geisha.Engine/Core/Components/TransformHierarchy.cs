using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Components
{
    /// <summary>
    ///     Provides common methods for handling hierarchy of transform components. Hierarchy of transform components is
    ///     achieved by attaching transform components to entities organized hierarchically.
    /// </summary>
    public static class TransformHierarchy
    {
        /// <summary>
        ///     Calculates 2D transformation matrix in global coordinate space for specified <paramref name="entity" /> with
        ///     <see cref="Transform2DComponent" /> attached.
        /// </summary>
        /// <param name="entity">Entity with <see cref="Transform2DComponent" /> attached.</param>
        /// <returns>
        ///     2D transformation matrix in global coordinate space as defined by <see cref="Transform2DComponent" />
        ///     hierarchy.
        /// </returns>
        /// <remarks>
        ///     The resulting 2D transformation matrix defines transform in global coordinate space equivalent to application
        ///     of whole hierarchy of <see cref="Transform2DComponent" /> from root entity down the hierarchy to specified
        ///     <paramref name="entity" />. If specified <paramref name="entity" /> or any entity in hierarchy does not have
        ///     <see cref="Transform2DComponent" /> attached then identity transform is used for such entities.
        /// </remarks>
        public static Matrix3x3 Calculate2DTransformationMatrix(Entity entity)
        {
            var transform = entity.HasComponent<Transform2DComponent>()
                ? entity.GetComponent<Transform2DComponent>().ToMatrix()
                : Matrix3x3.Identity;

            if (entity.IsRoot)
                return transform;

            return Calculate2DTransformationMatrix(entity.Parent) * transform;
        }
    }
}