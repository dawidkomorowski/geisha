using System;
using Geisha.Engine.Core.SceneModel.Definition;

namespace Geisha.Engine.Rendering.Components.Definition
{
    /// <inheritdoc />
    /// <summary>
    ///     Represents serializable <see cref="SpriteRenderer" /> that is used in a scene file content.
    /// </summary>
    public class SpriteRendererDefinition : IComponentDefinition
    {
        public bool Visible { get; set; }
        public string SortingLayerName { get; set; }
        public int OrderInLayer { get; set; } = 0;
        public Guid SpriteAssetId { get; set; }
    }
}