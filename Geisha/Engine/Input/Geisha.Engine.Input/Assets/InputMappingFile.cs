using System.Collections.Generic;
using Geisha.Framework.Input;

namespace Geisha.Engine.Input.Assets
{
    // TODO Add docs to all Mapping classes and all Assets classes
    public class InputMappingFile
    {
        public Dictionary<string, ActionMappingDefinition[]> ActionMappings { get; set; }
        public Dictionary<string, AxisMappingDefinition[]> AxisMappings { get; set; }
    }

    public class ActionMappingDefinition
    {
        public Key Key { get; set; }
    }

    public class AxisMappingDefinition
    {
        public Key Key { get; set; }
        public double Scale { get; set; }
    }
}