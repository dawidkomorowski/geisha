using Geisha.Common.Math.Serialization;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Physics.Components.Serialization
{
    /// <summary>
    ///     Represents serializable <see cref="RectangleColliderComponent" /> that is used in a scene file content.
    /// </summary>
    public sealed class SerializableRectangleColliderComponent : ISerializableComponent
    {
        /// <summary>
        ///     Represents <see cref="RectangleColliderComponent.Dimension" /> property of
        ///     <see cref="RectangleColliderComponent" />.
        /// </summary>
        public SerializableVector2 Dimension { get; set; }
    }
}