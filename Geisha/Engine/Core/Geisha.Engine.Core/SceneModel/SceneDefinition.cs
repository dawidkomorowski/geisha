using System.Collections.Generic;

namespace Geisha.Engine.Core.SceneModel
{
    /// <summary>
    ///     Represents serializable <see cref="Scene" /> definition that is used as a scene file content.
    /// </summary>
    public class SceneDefinition
    {
        public List<EntityDefinition> RootEntities { get; set; }
    }
}