using System.Collections.Generic;

namespace Geisha.Engine.Core.SceneModel.Definition
{
    /// <summary>
    ///     Represents serializable <see cref="Scene" /> that is used as a scene file content.
    /// </summary>
    public class SceneDefinition
    {
        public List<EntityDefinition> RootEntities { get; set; }
    }
}