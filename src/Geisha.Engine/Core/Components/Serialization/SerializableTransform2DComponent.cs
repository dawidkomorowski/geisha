using Geisha.Common.Math.Serialization;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Core.Components.Serialization
{
    /// <summary>
    ///     Represents serializable <see cref="Transform2DComponent" /> that is used in a scene file content.
    /// </summary>
    public sealed class SerializableTransform2DComponent : ISerializableComponent
    {
        /// <summary>
        ///     Represents <see cref="Transform2DComponent.Translation" /> property of <see cref="Transform2DComponent" />.
        /// </summary>
        public SerializableVector2 Translation { get; set; }

        /// <summary>
        ///     Represents <see cref="Transform2DComponent.Rotation" /> property of <see cref="Transform2DComponent" />.
        /// </summary>
        public double Rotation { get; set; }

        /// <summary>
        ///     Represents <see cref="Transform2DComponent.Scale" /> property of <see cref="Transform2DComponent" />.
        /// </summary>
        public SerializableVector2 Scale { get; set; }
    }
}