using Geisha.Common.Math.Serialization;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Core.Components.Serialization
{
    /// <summary>
    ///     Represents serializable <see cref="Transform3DComponent" /> that is used in a scene file content.
    /// </summary>
    public sealed class SerializableTransform3DComponent : ISerializableComponent
    {
        /// <summary>
        ///     Represents <see cref="Transform3DComponent.Translation" /> property of <see cref="Transform3DComponent" />.
        /// </summary>
        public SerializableVector3 Translation { get; set; }

        /// <summary>
        ///     Represents <see cref="Transform3DComponent.Rotation" /> property of <see cref="Transform3DComponent" />.
        /// </summary>
        public SerializableVector3 Rotation { get; set; }

        /// <summary>
        ///     Represents <see cref="Transform3DComponent.Scale" /> property of <see cref="Transform3DComponent" />.
        /// </summary>
        public SerializableVector3 Scale { get; set; }
    }
}