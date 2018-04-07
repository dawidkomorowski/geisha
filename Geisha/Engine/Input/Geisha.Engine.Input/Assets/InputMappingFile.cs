using System.Collections.Generic;
using Geisha.Engine.Input.Mapping;
using Geisha.Framework.Input;

namespace Geisha.Engine.Input.Assets
{
    /// <summary>
    ///     Represents input mapping file content to be used to load <see cref="InputMapping" /> from a file into memory.
    /// </summary>
    public class InputMappingFile
    {
        /// <summary>
        ///     Action mappings dictionary. Dictionary key is an action name and value is a list of hardware actions.
        /// </summary>
        public Dictionary<string, HardwareActionDefinition[]> ActionMappings { get; set; }

        /// <summary>
        ///     Axis mappings dictionary. Dictionary key is an axis name and value is a list of hardware axes.
        /// </summary>
        public Dictionary<string, HardwareAxisDefinition[]> AxisMappings { get; set; }
    }

    public class HardwareActionDefinition
    {
        public Key Key { get; set; }
    }

    public class HardwareAxisDefinition
    {
        public Key Key { get; set; }
        public double Scale { get; set; }
    }
}