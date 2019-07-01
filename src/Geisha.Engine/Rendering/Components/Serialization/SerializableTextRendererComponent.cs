﻿using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Rendering.Components.Serialization
{
    /// <summary>
    ///     Represents serializable <see cref="TextRendererComponent" /> that is used in a scene file content.
    /// </summary>
    public sealed class SerializableTextRendererComponent : ISerializableComponent
    {
        /// <summary>
        ///     Represents <see cref="TextRendererComponent.Visible" /> property of <see cref="TextRendererComponent" />.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        ///     Represents <see cref="TextRendererComponent.SortingLayerName" /> property of <see cref="TextRendererComponent" />.
        /// </summary>
        public string SortingLayerName { get; set; }

        /// <summary>
        ///     Represents <see cref="TextRendererComponent.OrderInLayer" /> property of <see cref="TextRendererComponent" />.
        /// </summary>
        public int OrderInLayer { get; set; }

        /// <summary>
        ///     Represents <see cref="TextRendererComponent.Text" /> property of <see cref="TextRendererComponent" />.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///     Represents <see cref="TextRendererComponent.FontSize" /> property of <see cref="TextRendererComponent" /> in points
        ///     (unit).
        /// </summary>
        public double FontSize { get; set; }

        /// <summary>
        ///     Represents <see cref="TextRendererComponent.Color" /> property of <see cref="TextRendererComponent" />.
        /// </summary>
        public int ColorArgb { get; set; }
    }
}