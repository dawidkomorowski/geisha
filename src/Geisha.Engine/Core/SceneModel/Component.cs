using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Core.SceneModel
{
    /// <summary>
    ///     Base class of all components to be supported by engine.
    /// </summary>
    public abstract class Component
    {
        /// <summary>
        ///     Initializes new instance of <see cref="Component" /> class.
        /// </summary>
        protected Component()
        {
            ComponentId = ComponentId.Of(GetType());
        }

        /// <summary>
        ///     <see cref="SceneModel.ComponentId" /> of this <see cref="Component" /> instance.
        /// </summary>
        /// <remarks>
        ///     Default value of <see cref="ComponentId" /> is full name of <see cref="Component" /> class. In order to
        ///     specify custom <see cref="SceneModel.ComponentId" /> for <see cref="Component" /> class mark such class with
        ///     <see cref="ComponentIdAttribute" />. It is useful to keep serialization compatibility while renaming the class or
        ///     moving it to different namespace.
        /// </remarks>
        public ComponentId ComponentId { get; }

        /// <summary>
        ///     Serializes data of this instance of <see cref="Component" />. Override in derived classes to provide serialization
        ///     functionality to a derived class.
        /// </summary>
        /// <remarks>
        ///     When overriding this method in derived class, be sure to call the base class's <see cref="Serialize" /> method
        ///     so data of base class is serialized as well.
        /// </remarks>
        /// <param name="writer">Instance of <see cref="IComponentDataWriter" /> provided for writing component data.</param>
        /// <param name="assetStore">
        ///     Instance of <see cref="IAssetStore" /> provided to lookup <see cref="AssetId" /> or access assets.
        /// </param>
        protected internal virtual void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
        {
        }

        /// <summary>
        ///     Deserializes data of this instance of <see cref="Component" />. Override in derived classes to provide
        ///     deserialization functionality to a derived class.
        /// </summary>
        /// <remarks>
        ///     When overriding this method in derived class, be sure to call the base class's <see cref="Deserialize" />
        ///     method so data of base class is deserialized as well.
        /// </remarks>
        /// <param name="reader">Instance of <see cref="IComponentDataReader" /> provided for reading component data.</param>
        /// <param name="assetStore">
        ///     Instance of <see cref="IAssetStore" /> provided to lookup <see cref="AssetId" /> or access
        ///     assets.
        /// </param>
        protected internal virtual void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
        {
        }
    }
}