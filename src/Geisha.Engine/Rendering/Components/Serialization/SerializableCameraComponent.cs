using Geisha.Common.Math.Serialization;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Rendering.Components.Serialization
{
    /// <summary>
    ///     Represents serializable <see cref="CameraComponent" /> that is used in a scene file content.
    /// </summary>
    public sealed class SerializableCameraComponent : ISerializableComponent
    {
        /// <summary>
        ///     Represents <see cref="CameraComponent.AspectRatioBehavior" /> property of <see cref="CameraComponent" />.
        /// </summary>
        public string? AspectRatioBehavior { get; set; }

        /// <summary>
        ///     Represents <see cref="CameraComponent.ViewRectangle" /> property of <see cref="CameraComponent" />.
        /// </summary>
        public SerializableVector2 ViewRectangle { get; set; }
    }
}