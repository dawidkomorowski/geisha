using System.Collections.Generic;

namespace Geisha.Engine.Core.SceneModel.Serialization
{
    /// <summary>
    ///     Represents serializable <see cref="Entity" /> that is used in a scene file content.
    /// </summary>
    public sealed class SerializableEntity
    {
        public string Name { get; set; }
        public List<SerializableEntity> Children { get; set; }
        public List<ISerializableComponent> Components { get; set; }
    }
}