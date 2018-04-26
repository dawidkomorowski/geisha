using System.Collections.Generic;

namespace Geisha.Engine.Core.SceneModel.Definition
{
    /// <summary>
    ///     Represents serializable <see cref="Entity" /> that is used in a scene file content.
    /// </summary>
    public sealed class EntityDefinition
    {
        public string Name { get; set; }
        public List<EntityDefinition> Children { get; set; }
        public List<IComponentDefinition> Components { get; set; }
    }
}