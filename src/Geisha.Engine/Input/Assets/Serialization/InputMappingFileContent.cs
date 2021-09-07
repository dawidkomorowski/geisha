using System.Collections.Generic;
using System.Text.Json.Serialization;
using Geisha.Engine.Input.Mapping;

namespace Geisha.Engine.Input.Assets.Serialization
{
    /// <summary>
    ///     Represents input mapping file content to be used to load <see cref="InputMapping" /> from a file into memory.
    /// </summary>
    public sealed class InputMappingFileContent
    {
        /// <summary>
        ///     Action mappings dictionary. Dictionary key is an action name and value is a list of hardware actions.
        /// </summary>
        public Dictionary<string, SerializableHardwareAction[]>? ActionMappings { get; set; }

        /// <summary>
        ///     Axis mappings dictionary. Dictionary key is an axis name and value is a list of hardware axes.
        /// </summary>
        public Dictionary<string, SerializableHardwareAxis[]>? AxisMappings { get; set; }
    }

    /// <summary>
    ///     Represents <see cref="HardwareAction" /> in input mapping file content.
    /// </summary>
    public sealed class SerializableHardwareAction
    {
        /// <summary>
        ///     Keyboard key mapped to action.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Key? Key { get; set; }

        /// <summary>
        ///     Mouse button mapped to action.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MouseButton? MouseButton { get; set; }
    }

    /// <summary>
    ///     Represents <see cref="HardwareAxis" /> in input mapping file content.
    /// </summary>
    public sealed class SerializableHardwareAxis
    {
        /// <summary>
        ///     Keyboard key mapped to axis.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Key? Key { get; set; }

        /// <summary>
        ///     Mouse axis mapped to axis.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MouseAxis? MouseAxis { get; set; }

        /// <summary>
        ///     Scaling factor of hardware axis state to logical axis state.
        /// </summary>
        public double Scale { get; set; }
    }

    /// <summary>
    ///     Enumerates mouse buttons supported as hardware actions in input mapping file content.
    /// </summary>
    public enum MouseButton
    {
        /// <summary>
        ///     Left mouse button.
        /// </summary>
        LeftButton,

        /// <summary>
        ///     Middle mouse button.
        /// </summary>
        MiddleButton,

        /// <summary>
        ///     Right mouse button.
        /// </summary>
        RightButton,

        /// <summary>
        ///     First extended mouse button.
        /// </summary>
        XButton1,

        /// <summary>
        ///     Second extended mouse button.
        /// </summary>
        XButton2
    }

    /// <summary>
    ///     Enumerates mouse axes supported as hardware axes in input mapping file content.
    /// </summary>
    public enum MouseAxis
    {
        /// <summary>
        ///     Horizontal axis of mouse movement.
        /// </summary>
        AxisX,

        /// <summary>
        ///     Vertical axis of mouse movement.
        /// </summary>
        AxisY
    }
}