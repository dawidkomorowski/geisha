﻿using System;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Input.Mapping;

namespace Geisha.Engine.Input.Components.Definition
{
    /// <inheritdoc />
    /// <summary>
    ///     Represents serializable <see cref="T:Geisha.Engine.Input.Components.InputComponent" /> that is used in a scene file
    ///     content.
    /// </summary>
    public sealed class InputComponentDefinition : ISerializableComponent
    {
        /// <summary>
        ///     Asset id of <see cref="InputMapping"/> asset.
        /// </summary>
        public Guid? InputMappingAssetId { get; set; }
    }
}