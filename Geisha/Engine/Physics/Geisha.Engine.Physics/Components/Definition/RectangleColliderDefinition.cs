using Geisha.Common.Math.Serialization;
using Geisha.Engine.Core.SceneModel.Serialization;

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
        public SerializableVector2 Dimension { get; set; }
    }
}