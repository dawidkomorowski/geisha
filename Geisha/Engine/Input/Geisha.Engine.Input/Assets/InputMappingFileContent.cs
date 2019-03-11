using System;
using System.Collections.Generic;
using Geisha.Engine.Input.Mapping;
using Geisha.Framework.Input;

namespace Geisha.Engine.Input.Assets
{
    /// <summary>
    ///     Represents input mapping file content to be used to load <see cref="InputMapping" /> from a file into memory.
    /// </summary>
    public class InputMappingFileContent
    {
        /// <summary>
        ///     Asset id.
        /// </summary>
        public Guid AssetId { get; set; }

        /// <summary>
        ///     Action mappings dictionary. Dictionary key is an action name and value is a list of hardware actions.
        /// </summary>
        public Dictionary<string, SerializableHardwareAction[]> ActionMappings { get; set; }

        /// <summary>
        ///     Axis mappings dictionary. Dictionary key is an axis name and value is a list of hardware axes.
        /// </summary>
        public Dictionary<string, SerializableHardwareAxis[]> AxisMappings { get; set; }
    }

    /// <summary>
    ///     Represents <see cref="HardwareAction" /> in input mapping file content.
    /// </summary>
    public class SerializableHardwareAction
    {
        /// <summary>
        ///     Keyboard key mapped to action.
        /// </summary>
        public Key Key { get; set; }
    }

    /// <summary>
    ///     Represents <see cref="HardwareAxis" /> in input mapping file content.
    /// </summary>
    public class SerializableHardwareAxis
    {
        /// <summary>
        ///     Keyboard key mapped to axis.
        /// </summary>
        public Key Key { get; set; }

        /// <summary>
        ///     Scaling factor of hardware axis state to logical axis state.
        /// </summary>
        public double Scale { get; set; }
    }
}