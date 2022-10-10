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
        ///     Initializes new instance of <see cref="Renderer2DComponent" /> class which is attached to specified entity.
        /// </summary>
        /// <param name="entity">Entity to which new component is attached.</param>
        protected Renderer2DComponent(Entity entity) : base(entity)
        {
            if (entity.HasComponent<Renderer2DComponent>())
            {
                throw new ArgumentException($"{nameof(Renderer2DComponent)} is already added to entity.");
            }
        }

        /// <summary>
        ///     Indicates whether result of rendering is visible. Default value is <c>true</c>.
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        ///     Name of sorting layer the rendered object will be placed in. Sorting layers are used to define order of objects
        ///     rendering. Order of layers is defined in <see cref="RenderingConfiguration" />. Default value is defined by
        ///     <see cref="RenderingConfiguration.DefaultSortingLayerName" />.
        /// </summary>
        public string SortingLayerName { get; set; } = RenderingConfiguration.DefaultSortingLayerName;

        /// <summary>
        ///     Defines order of objects rendering in the same layer. Rendering order is from lower to higher. Default value is
        ///     zero.
        /// </summary>
        public int OrderInLayer { get; set; }

        /// <inheritdoc />
        protected internal override void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
        {
            base.Serialize(writer, assetStore);
            writer.WriteBool("Visible", Visible);
            writer.WriteString("SortingLayerName", SortingLayerName);
            writer.WriteInt("OrderInLayer", OrderInLayer);
        }

        /// <inheritdoc />
        protected internal override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
        {
            base.Deserialize(reader, assetStore);
            Visible = reader.ReadBool("Visible");
            SortingLayerName = reader.ReadString("SortingLayerName") ??
                               throw new InvalidOperationException("SortingLayerName cannot be null.");
            OrderInLayer = reader.ReadInt("OrderInLayer");
        }
    }
}