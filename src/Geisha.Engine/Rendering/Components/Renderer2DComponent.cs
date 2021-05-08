using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Rendering.Components
{
    /// <summary>
    ///     Base class for 2D rendering components that provides common features.
    /// </summary>
    public abstract class Renderer2DComponent : IComponent
    {
        public abstract ComponentId ComponentId { get; }

        /// <summary>
        ///     Indicates whether result of rendering is visible.
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        ///     Name of sorting layer the rendered object will be placed in. Sorting layers are used to define order of objects
        ///     rendering. Order of layers is defined in <see cref="RenderingConfiguration" />.
        /// </summary>
        public string SortingLayerName { get; set; } = RenderingConfiguration.DefaultSortingLayerName;

        /// <summary>
        ///     Defines order of objects rendering in the same layer. Rendering order is from smaller to higher.
        /// </summary>
        public int OrderInLayer { get; set; }

    }
}