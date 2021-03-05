using System.Collections.Generic;

namespace Geisha.Engine.Core.SceneModel.Serialization
{
    /// <summary>
    ///     Represents serializable <see cref="Scene" /> that is used as a scene file content.
    /// </summary>
    public sealed class SerializableScene
    {
        /// <summary>
        ///     Represents <see cref="Scene.RootEntities" /> property of <see cref="Scene" />.
        /// </summary>
        public List<SerializableEntity> RootEntities { get; set; } = new List<SerializableEntity>();

        /// <summary>
        ///     Represents <see cref="Scene.SceneBehaviorName" /> property of <see cref="Scene" />.
        /// </summary>
        public string? SceneBehaviorName { get; set; }
    }
}