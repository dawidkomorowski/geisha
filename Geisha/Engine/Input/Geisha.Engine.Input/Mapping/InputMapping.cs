using System.Collections.Generic;

namespace Geisha.Engine.Input.Mapping
{
    public class InputMapping
    {
        public IList<ActionMappingGroup> ActionMappingGroups { get; } = new List<ActionMappingGroup>();
        public IList<AxisMappingGroup> AxisMappingGroups { get; } = new List<AxisMappingGroup>();
    }
}