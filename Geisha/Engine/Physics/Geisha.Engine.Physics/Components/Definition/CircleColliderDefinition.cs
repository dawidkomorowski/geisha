using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Physics.Components.Definition
{
    /// <summary>
    ///     Represents serializable <see cref="CircleCollider" /> that is used in a scene file content.
    /// </summary>
    public sealed class CircleColliderDefinition : IComponentDefinition
    {
        /// <summary>
        ///     Defines <see cref="CircleCollider.Radius" /> property of <see cref="CircleCollider" />.
        /// </summary>
        public double Radius { get; set; }
    }
}