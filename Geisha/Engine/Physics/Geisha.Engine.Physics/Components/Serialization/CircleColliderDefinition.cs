using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Physics.Components.Serialization
{
    /// <summary>
    ///     Represents serializable <see cref="CircleColliderComponent" /> that is used in a scene file content.
    /// </summary>
    public sealed class CircleColliderDefinition : ISerializableComponent
    {
        /// <summary>
        ///     Defines <see cref="CircleColliderComponent.Radius" /> property of <see cref="CircleColliderComponent" />.
        /// </summary>
        public double Radius { get; set; }
    }
}