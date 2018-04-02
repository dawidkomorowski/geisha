using System.Collections.Generic;

namespace Geisha.Engine.Input.Mapping
{
    public class InputMapping
    {
        public IList<ActionMappingGroup> ActionMappingGroups { get; } = new List<ActionMappingGroup>();
        public IList<AxisMappingGroup> AxisMappingGroups { get; } = new List<AxisMappingGroup>();
    }

    public class ActionMappingGroup
    {
        public string ActionName { get; set; }
        public IList<ActionMapping> ActionMappings { get; } = new List<ActionMapping>();
    }

    public class AxisMappingGroup
    {
        public string AxisName { get; set; }
        public IList<AxisMapping> AxisMappings { get; } = new List<AxisMapping>();
    }

    public class ActionMapping
    {
        public HardwareInputVariant HardwareInputVariant { get; set; }
    }

    public class AxisMapping
    {
        public HardwareInputVariant HardwareInputVariant { get; set; }
        public double Scale { get; set; }
    }
}