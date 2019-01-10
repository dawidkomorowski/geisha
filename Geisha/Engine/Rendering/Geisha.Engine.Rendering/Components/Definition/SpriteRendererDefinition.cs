using System;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Components.Definition
{
    /// <summary>
    ///     Represents serializable <see cref="SpriteRenderer" /> that is used in a scene file content.
    /// </summary>
    public sealed class SpriteRendererDefinition : IComponentDefinition
    {
        /// <summary>
        ///     Defines <see cref="SpriteRenderer.Visible" /> property of <see cref="SpriteRenderer" />.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        ///     Defines <see cref="SpriteRenderer.SortingLayerName" /> property of <see cref="SpriteRenderer" />.
        /// </summary>
        public string SortingLayerName { get; set; }

        /// <summary>
        ///     Defines <see cref="SpriteRenderer.OrderInLayer" /> property of <see cref="SpriteRenderer" />.
        /// </summary>
        public int OrderInLayer { get; set; }

        /// <summary>
        ///     Asset id of <see cref="Sprite" /> asset.
        /// </summary>
        public Guid SpriteAssetId { get; set; }
    }
}