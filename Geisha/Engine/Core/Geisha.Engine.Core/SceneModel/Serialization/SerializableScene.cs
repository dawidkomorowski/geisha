using System.Collections.Generic;

namespace Geisha.Engine.Core.SceneModel.Serialization
{
    /// <summary>
    ///     Represents serializable <see cref="Scene" /> that is used as a scene file content.
    /// </summary>
    public sealed class SerializableScene
    {
        public List<SerializableEntity> RootEntities { get; set; }
        // TODO Add documentation.
        public string ConstructionScript { get; set; }
    }
}