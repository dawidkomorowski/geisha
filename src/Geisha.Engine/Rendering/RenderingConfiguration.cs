using System.Collections.Generic;

namespace Geisha.Engine.Rendering
{
    /// <summary>
    ///     Configuration of engine rendering subsystem.
    /// </summary>
    public sealed class RenderingConfiguration
    {
        /// <summary>
        ///     Provides name of default sorting layer.
        /// </summary>
        public const string DefaultSortingLayerName = "Default";

        private RenderingConfiguration(
            bool enableVSync,
            int screenHeight,
            int screenWidth,
            IReadOnlyList<string> sortingLayersOrder)
        {
            EnableVSync = enableVSync;
            ScreenHeight = screenHeight;
            ScreenWidth = screenWidth;
            SortingLayersOrder = sortingLayersOrder;
        }

        /// <summary>
        ///     If true, enables VSync. This makes rendered frames wait for vertical synchronization in order to be presented.
        ///     Therefore frame rate is limited to refresh rate of display.
        /// </summary>
        public bool EnableVSync { get; }

        /// <summary>
        ///     Height of the screen (full screen) or client area in the window (excluding window frame) in pixels.
        /// </summary>
        public int ScreenHeight { get; }

        /// <summary>
        ///     Width of the screen (full screen) or client area in the window (excluding window frame) in pixels.
        /// </summary>
        public int ScreenWidth { get; }

        /// <summary>
        ///     List of sorting layers in order of rendering that is first layer in the list is rendered first, last layer in the
        ///     list is rendered last (on top of previous layers). Default is <c>["Default"]</c>.
        /// </summary>
        public IReadOnlyList<string> SortingLayersOrder { get; }

        public static IBuilder CreateBuilder() => new Builder();

        public interface IBuilder
        {
            IBuilder WithEnableVSync(bool enableVSync);
            IBuilder WithScreenHeight(int screenHeight);
            IBuilder WithScreenWidth(int screenWidth);
            IBuilder WithSortingLayersOrder(IReadOnlyList<string> sortingLayersOrder);
            RenderingConfiguration Build();
        }

        private sealed class Builder : IBuilder
        {
            private bool _enableVSync;
            private int _screenHeight = 720;
            private int _screenWidth = 1280;
            private IReadOnlyList<string> _sortingLayersOrder = new List<string> {DefaultSortingLayerName}.AsReadOnly();

            public IBuilder WithEnableVSync(bool enableVSync)
            {
                _enableVSync = enableVSync;
                return this;
            }

            public IBuilder WithScreenHeight(int screenHeight)
            {
                _screenHeight = screenHeight;
                return this;
            }

            public IBuilder WithScreenWidth(int screenWidth)
            {
                _screenWidth = screenWidth;
                return this;
            }

            public IBuilder WithSortingLayersOrder(IReadOnlyList<string> sortingLayersOrder)
            {
                _sortingLayersOrder = sortingLayersOrder;
                return this;
            }

            public RenderingConfiguration Build() => new RenderingConfiguration(
                _enableVSync,
                _screenHeight,
                _screenWidth,
                _sortingLayersOrder);
        }
    }
}