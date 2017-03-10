using System.Collections.Generic;

namespace Geisha.Engine.Input.Mapping
{
    public class AxisMappingGroup
    {
        public string AxisName { get; set; }
        public IList<AxisMapping> AxisMappings { get; } = new List<AxisMapping>();
    }
}