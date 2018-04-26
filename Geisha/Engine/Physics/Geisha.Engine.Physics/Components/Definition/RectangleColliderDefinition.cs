using Geisha.Common.Math.Definition;
using Geisha.Engine.Core.SceneModel.Definition;

namespace Geisha.Engine.Physics.Components.Definition
{
    /// <summary>
    ///     Represents serializable <see cref="RectangleCollider" /> that is used in a scene file content.
    /// </summary>
    public sealed class RectangleColliderDefinition : IComponentDefinition
    {
        /// <summary>
        ///     Defines <see cref="RectangleCollider.Dimension" /> property of <see cref="RectangleCollider" />.
        /// </summary>
        public Vector2Definition Dimension { get; set; }
    }
}