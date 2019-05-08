using System.Collections.Generic;

namespace Geisha.Engine.Rendering.Configuration
{
    /// <summary>
    ///     Represents type safe rendering engine configuration.
    /// </summary>
    public sealed class RenderingConfiguration
    {
        /// <summary>
        ///     Provides name of default sorting layer.
        /// </summary>
        public const string DefaultSortingLayerName = "Default";

        /// <summary>
        ///     If true, enables VSync. That is rendered frames wait for vertical synchronization in order to be presented
        ///     therefore frame rate is limited to refresh rate of display.
        /// </summary>
        public bool EnableVSync { get; set; } = false;

        /// <summary>
        ///     List of sorting layers in order of rendering that is first layer in the list is rendered first, last layer in the
        ///     list is rendered last (on top of previous layers). Default is <c>["Default"]</c>.
        /// </summary>
        public List<string> SortingLayersOrder { get; set; } = new List<string> {DefaultSortingLayerName};
    }
}