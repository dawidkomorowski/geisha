using System.Collections.Generic;

namespace Geisha.Engine.Input.Mapping
{
    /// <summary>
    ///     Defines mapping of hardware input to actions and axes.
    /// </summary>
    public class InputMapping
    {
        /// <summary>
        ///     List of action mappings.
        /// </summary>
        public IList<ActionMapping> ActionMappings { get; } = new List<ActionMapping>();

        /// <summary>
        ///     List of axis mappings.
        /// </summary>
        public IList<AxisMapping> AxisMappings { get; } = new List<AxisMapping>();
    }

    /// <summary>
    ///     Defines mapping of list of hardware actions to a single logical action.
    /// </summary>
    public class ActionMapping
    {
        /// <summary>
        ///     Name of action.
        /// </summary>
        public string ActionName { get; set; } = string.Empty;

        /// <summary>
        ///     List of hardware actions this logical action is based on.
        /// </summary>
        public IList<HardwareAction> HardwareActions { get; } = new List<HardwareAction>();
    }

    /// <summary>
    ///     Defines mapping of list of hardware axes to a single logical axis.
    /// </summary>
    public class AxisMapping
    {
        /// <summary>
        ///     Name of axis.
        /// </summary>
        public string AxisName { get; set; } = string.Empty;

        /// <summary>
        ///     List of hardware axes this logical axis is based on.
        /// </summary>
        public IList<HardwareAxis> HardwareAxes { get; } = new List<HardwareAxis>();
    }

    /// <summary>
    ///     Represents a single hardware action that is defined by hardware input variant i.e. a particular keyboard key press.
    /// </summary>
    public class HardwareAction
    {
        /// <summary>
        ///     Hardware input variant this hardware action is based on.
        /// </summary>
        public HardwareInputVariant HardwareInputVariant { get; set; }
    }

    /// <summary>
    ///     Represents a single hardware axis that is defined by hardware input variant i.e. a particular keyboard key press.
    /// </summary>
    public class HardwareAxis
    {
        /// <summary>
        ///     Hardware input variant this hardware axis is based on.
        /// </summary>
        public HardwareInputVariant HardwareInputVariant { get; set; }

        /// <summary>
        ///     Scaling factor of hardware axis state to logical axis state. For discrete input +1 and -1 is mostly used. For
        ///     analog input it can be used as sensitivity.
        /// </summary>
        public double Scale { get; set; }
    }
}