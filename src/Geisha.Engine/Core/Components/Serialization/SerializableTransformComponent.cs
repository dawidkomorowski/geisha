using Geisha.Common.Math.Serialization;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Core.Components.Serialization
{
    /// <summary>
    ///     Represents serializable <see cref="TransformComponent" /> that is used in a scene file content.
    /// </summary>
    public sealed class SerializableTransformComponent : ISerializableComponent
    {
        /// <summary>
        ///     Represents <see cref="TransformComponent.Translation" /> property of <see cref="TransformComponent" />.
        /// </summary>
        public SerializableVector3 Translation { get; set; }

        /// <summary>
        ///     Represents <see cref="TransformComponent.Rotation" /> property of <see cref="TransformComponent" />.
        /// </summary>
        public SerializableVector3 Rotation { get; set; }

        /// <summary>
        ///     Represents <see cref="TransformComponent.Scale" /> property of <see cref="TransformComponent" />.
        /// </summary>
        public SerializableVector3 Scale { get; set; }
    }
}