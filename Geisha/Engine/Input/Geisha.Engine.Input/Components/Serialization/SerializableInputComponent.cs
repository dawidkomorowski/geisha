using System;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Input.Mapping;

namespace Geisha.Engine.Input.Components.Serialization
{
    /// <inheritdoc />
    /// <summary>
    ///     Represents serializable <see cref="InputComponent" /> that is used in a scene file content.
    /// </summary>
    public sealed class SerializableInputComponent : ISerializableComponent
    {
        /// <summary>
        ///     Asset id of <see cref="InputMapping" /> asset.
        /// </summary>
        public Guid? InputMappingAssetId { get; set; }
    }
}