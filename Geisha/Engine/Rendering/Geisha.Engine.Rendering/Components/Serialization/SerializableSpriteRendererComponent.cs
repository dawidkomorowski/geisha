using System;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Components.Serialization
{
    /// <summary>
    ///     Represents serializable <see cref="SpriteRendererComponent" /> that is used in a scene file content.
    /// </summary>
    public sealed class SerializableSpriteRendererComponent : ISerializableComponent
    {
        /// <summary>
        ///     Defines <see cref="SpriteRendererComponent.Visible" /> property of <see cref="SpriteRendererComponent" />.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        ///     Defines <see cref="SpriteRendererComponent.SortingLayerName" /> property of <see cref="SpriteRendererComponent" />.
        /// </summary>
        public string SortingLayerName { get; set; }

        /// <summary>
        ///     Defines <see cref="SpriteRendererComponent.OrderInLayer" /> property of <see cref="SpriteRendererComponent" />.
        /// </summary>
        public int OrderInLayer { get; set; }

        /// <summary>
        ///     Asset id of <see cref="Sprite" /> asset.
        /// </summary>
        public Guid SpriteAssetId { get; set; }
    }
}