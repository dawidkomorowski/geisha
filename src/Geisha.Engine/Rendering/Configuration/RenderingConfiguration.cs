using System.Collections.Generic;

namespace Geisha.Engine.Rendering.Configuration
{
    /// <summary>
    ///     Rendering engine configuration.
    /// </summary>
    public sealed class RenderingConfiguration
    {
        public static IBuilder CreateBuilder() => new Builder();

        private RenderingConfiguration(
            bool enableVSync,
            IReadOnlyList<string> sortingLayersOrder)
        {
            EnableVSync = enableVSync;
            SortingLayersOrder = sortingLayersOrder;
        }

        /// <summary>
        ///     Provides name of default sorting layer.
        /// </summary>
        public const string DefaultSortingLayerName = "Default";

        /// <summary>
        ///     If true, enables VSync. That is rendered frames wait for vertical synchronization in order to be presented
        ///     therefore frame rate is limited to refresh rate of display.
        /// </summary>
        public bool EnableVSync { get; }

        /// <summary>
        ///     List of sorting layers in order of rendering that is first layer in the list is rendered first, last layer in the
        ///     list is rendered last (on top of previous layers). Default is <c>["Default"]</c>.
        /// </summary>
        public IReadOnlyList<string> SortingLayersOrder { get; }

        public interface IBuilder
        {
            IBuilder WithEnableVSync(bool enableVSync);
            IBuilder WithSortingLayersOrder(IReadOnlyList<string> sortingLayersOrder);
            RenderingConfiguration Build();
        }

        private sealed class Builder : IBuilder
        {
            private bool _enableVSync = false;
            private IReadOnlyList<string> _sortingLayersOrder = new List<string> {DefaultSortingLayerName}.AsReadOnly();

            public IBuilder WithEnableVSync(bool enableVSync)
            {
                _enableVSync = enableVSync;
                return this;
            }

            public IBuilder WithSortingLayersOrder(IReadOnlyList<string> sortingLayersOrder)
            {
                _sortingLayersOrder = sortingLayersOrder;
                return this;
            }

            public RenderingConfiguration Build() => new RenderingConfiguration(_enableVSync, _sortingLayersOrder);
        }
    }
}