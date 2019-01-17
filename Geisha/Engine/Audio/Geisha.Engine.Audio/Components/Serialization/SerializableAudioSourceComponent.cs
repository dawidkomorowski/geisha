using System;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Framework.Audio;

namespace Geisha.Engine.Audio.Components.Serialization
{
    /// <summary>
    ///     Represents serializable <see cref="AudioSourceComponent" /> that is used in a scene file content.
    /// </summary>
    public sealed class SerializableAudioSourceComponent : ISerializableComponent
    {
        /// <summary>
        ///     Asset id of <see cref="ISound" /> asset.
        /// </summary>
        public Guid SoundAssetId { get; set; }

        /// <summary>
        ///     Defines <see cref="AudioSourceComponent.IsPlaying" /> property of <see cref="AudioSourceComponent" />.
        /// </summary>
        public bool IsPlaying { get; set; }
    }
}