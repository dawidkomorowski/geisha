using System.Collections.Generic;

namespace Geisha.Engine.Core.SceneModel.Definition
{
    public sealed class AutomaticComponentDefinition : IComponentDefinition
    {
        public string ComponentTypeFullName { get; set; }
        public Dictionary<string, object> Properties { get; set; }
    }
}