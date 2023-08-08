﻿using System.Collections.Generic;

namespace Geisha.Engine.Rendering
{
    /// <summary>
    ///     Configuration of engine rendering subsystem.
    /// </summary>
    public sealed record RenderingConfiguration
    {
        /// <summary>
        ///     Provides name of default sorting layer.
        /// </summary>
        public const string DefaultSortingLayerName = "Default";

        /// <summary>
        ///     If true, enables VSync. This makes rendered frames wait for vertical synchronization in order to be presented.
        ///     Therefore frame rate is limited to refresh rate of display.
        /// </summary>
        public bool EnableVSync { get; init; } = false;

        /// <summary>
        ///     Height of the screen (full screen) or client area in the window (excluding window frame) in pixels.
        /// </summary>
        public int ScreenHeight { get; init; } = 720;

        /// <summary>
        ///     Width of the screen (full screen) or client area in the window (excluding window frame) in pixels.
        /// </summary>
        public int ScreenWidth { get; init; } = 1280;

        // TODO Add xml documentation.
        public bool ShowRenderingStatistics { get; init; } = false;

        /// <summary>
        ///     List of sorting layers in order of rendering that is first layer in the list is rendered first, last layer in the
        ///     list is rendered last (on top of previous layers). Default is <c>["Default"]</c>.
        /// </summary>
        public IReadOnlyList<string> SortingLayersOrder { get; init; } = new List<string> { DefaultSortingLayerName }.AsReadOnly();
    }
}