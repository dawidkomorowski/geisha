using System.Collections.Generic;

namespace Geisha.Engine.Core.SceneModel
{
    /// <summary>
    ///     Represents serializable <see cref="Entity" /> definition that is used in a scene file content.
    /// </summary>
    public class EntityDefinition
    {
        public string Name { get; set; }
        public List<EntityDefinition> Children { get; set; }
    }
}