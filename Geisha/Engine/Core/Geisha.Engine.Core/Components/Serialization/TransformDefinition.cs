using Geisha.Common.Math.Serialization;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Core.Components.Serialization
{
    /// <summary>
    ///     Represents serializable <see cref="Transform" /> that is used in a scene file content.
    /// </summary>
    public sealed class TransformDefinition : ISerializableComponent
    {
        /// <summary>
        ///     Defines <see cref="Transform.Translation" /> property of <see cref="Transform" />.
        /// </summary>
        public SerializableVector3 Translation { get; set; }

        /// <summary>
        ///     Defines <see cref="Transform.Rotation" /> property of <see cref="Transform" />.
        /// </summary>
        public SerializableVector3 Rotation { get; set; }

        /// <summary>
        ///     Defines <see cref="Transform.Scale" /> property of <see cref="Transform" />.
        /// </summary>
        public SerializableVector3 Scale { get; set; }
    }
}