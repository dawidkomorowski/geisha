using System;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Rendering.Components
{
    /// <summary>
    ///     Base class for 2D rendering components that provides common features.
    /// </summary>
    public abstract class Renderer2DComponent : Component
    {
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

        protected internal override void Serialize(IComponentDataWriter componentDataWriter, IAssetStore assetStore)
        {
            componentDataWriter.WriteBool("Visible", Visible);
            componentDataWriter.WriteString("SortingLayerName", SortingLayerName);
            componentDataWriter.WriteInt("OrderInLayer", OrderInLayer);
        }

        protected internal override void Deserialize(IComponentDataReader componentDataReader, IAssetStore assetStore)
        {
            Visible = componentDataReader.ReadBool("Visible");
            SortingLayerName = componentDataReader.ReadString("SortingLayerName") ??
                               throw new InvalidOperationException("SortingLayerName cannot be null.");
            OrderInLayer = componentDataReader.ReadInt("OrderInLayer");
        }
    }
}