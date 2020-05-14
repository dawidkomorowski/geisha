using System.Collections.Generic;

namespace Geisha.Engine.Core.SceneModel.Serialization
{
    /// <summary>
    ///     Represents serializable <see cref="Entity" /> that is used in a scene file content.
    /// </summary>
    public sealed class SerializableEntity
    {
        /// <summary>
        ///     Represents <see cref="Entity.Name" /> property of <see cref="Entity" />.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        ///     Represents <see cref="Entity.Children" /> property of <see cref="Entity" />.
        /// </summary>
        public List<SerializableEntity> Children { get; set; } = new List<SerializableEntity>();

        /// <summary>
        ///     Represents <see cref="Entity.Components" /> property of <see cref="Entity" />.
        /// </summary>
        public List<ISerializableComponent> Components { get; set; } = new List<ISerializableComponent>();
    }
}