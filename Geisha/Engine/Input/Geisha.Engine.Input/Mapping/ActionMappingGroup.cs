using System.Collections.Generic;

namespace Geisha.Engine.Input.Mapping
{
    public class ActionMappingGroup
    {
        public string ActionName { get; set; }
        public IList<ActionMapping> ActionMappings { get; } = new List<ActionMapping>();
    }
}