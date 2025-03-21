using System;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Rendering.Systems;

namespace Geisha.Engine.Rendering.Components
{
    /// <summary>
    ///     Base class for 2D rendering components that provides common features.
    /// </summary>
    public abstract class Renderer2DComponent : Component
    {
        private protected Renderer2DComponent(Entity entity, IRenderNode renderNode) : base(entity)
        {
            foreach (var component in entity.Components)
            {
                if (component is Renderer2DComponent)
                {
                    throw new ArgumentException($"{nameof(Renderer2DComponent)} is already added to entity.");
                }
            }

            RenderNode = renderNode;
            Visible = true;
            SortingLayerName = RenderingConfiguration.DefaultSortingLayerName;
            OrderInLayer = 0;
        }

        private protected IRenderNode RenderNode { get; set; }

        /// <summary>
        ///     Indicates whether this <see cref="Renderer2DComponent" /> is managed by rendering system.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         <see cref="Renderer2DComponent" /> is managed by rendering system when it belongs to <see cref="Scene" /> that
        ///         is managed by rendering system. It is true for components that are part of currently processed scene at
        ///         runtime, but it may not be true during serialization or in context of some tools.
        ///     </para>
        ///     <para>
        ///         <see cref="Renderer2DComponent" /> has limited functionality when it is not managed by rendering system. For
        ///         example some APIs may return default values instead of being actually computed.
        ///     </para>
        /// </remarks>
        public bool IsManagedByRenderingSystem => RenderNode.IsManagedByRenderingSystem;

        /// <summary>
        ///     Indicates whether result of rendering is visible. Default value is <c>true</c>.
        /// </summary>
        public bool Visible
        {
            get => RenderNode.Visible;
            set => RenderNode.Visible = value;
        }

        /// <summary>
        ///     Name of sorting layer the rendered object will be placed in. Sorting layers are used to define order of objects
        ///     rendering. Order of layers is defined in <see cref="RenderingConfiguration" />. Default value is defined by
        ///     <see cref="RenderingConfiguration.DefaultSortingLayerName" />.
        /// </summary>
        public string SortingLayerName
        {
            get => RenderNode.SortingLayerName;
            set => RenderNode.SortingLayerName = value;
        }

        /// <summary>
        ///     Defines order of objects rendering in the same layer. Rendering order is from lower to higher. Default value is
        ///     zero.
        /// </summary>
        public int OrderInLayer
        {
            get => RenderNode.OrderInLayer;
            set => RenderNode.OrderInLayer = value;
        }


        /// <summary>
        ///     Gets axis aligned bounding rectangle in global coordinates.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         <see cref="BoundingRectangle" /> is axis aligned rectangle that fully encloses geometry represented by this
        ///         instance of <see cref="Renderer2DComponent" />.
        ///     </para>
        ///     <para>
        ///         This property returns default value of <see cref="AxisAlignedRectangle" /> when
        ///         <see cref="Renderer2DComponent" /> is not managed by rendering system.
        ///     </para>
        /// </remarks>
        /// <seealso cref="IsManagedByRenderingSystem" />
        public AxisAlignedRectangle BoundingRectangle => RenderNode.GetBoundingRectangle();

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