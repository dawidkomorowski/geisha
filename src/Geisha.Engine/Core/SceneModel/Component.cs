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
        ///     Initializes a new instance of the <see cref="Component" /> class attached to the specified entity.
        /// </summary>
        /// <param name="entity">The entity to which the new component is attached.</param>
        protected Component(Entity entity)
        {
            ComponentId = ComponentId.Of(GetType());
            Entity = entity;
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
        ///     <see cref="SceneModel.Entity" /> to which this component is attached.
        /// </summary>
        public Entity Entity { get; }

        /// <summary>
        ///     <see cref="SceneModel.Scene" /> that this component is part of.
        /// </summary>
        public Scene Scene => Entity.Scene;

        /// <summary>
        ///     Serializes the persistent state of this <see cref="Component" /> instance.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This method is invoked by the scene serialization infrastructure. The data written here must be readable by
        ///         the corresponding <see cref="Deserialize" /> implementation.
        ///     </para>
        ///     <para>
        ///         Only serialize state that is required to recreate the component when a scene is loaded. Avoid serializing
        ///         transient/runtime-only state (for example cached values, handles/proxies, derived values) unless it is
        ///         explicitly part of the persisted scene.
        ///     </para>
        ///     <para>
        ///         Property names passed to <paramref name="writer" /> are part of the serialized format and are
        ///         case-sensitive. Keep them stable to preserve backward compatibility. When introducing new properties, make
        ///         <see cref="Deserialize" /> tolerant to older data by using <see cref="IObjectReader.IsDefined" /> and falling
        ///         back to defaults.
        ///     </para>
        ///     <para>
        ///         When overriding this method in derived class, be sure to call the base class's <see cref="Serialize" />
        ///         method so data of base class is serialized as well.
        ///     </para>
        /// </remarks>
        /// <param name="writer">Instance of <see cref="IComponentDataWriter" /> provided for writing component data.</param>
        /// <param name="assetStore">
        ///     Instance of <see cref="IAssetStore" /> provided to convert asset references to/from <see cref="AssetId" /> or
        ///     access assets.
        /// </param>
        protected internal virtual void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
        {
        }

        /// <summary>
        ///     Deserializes the persistent state of this <see cref="Component" /> instance.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This method is invoked by the scene deserialization infrastructure after the component instance has been
        ///         created and attached to an <see cref="Entity" />.
        ///     </para>
        ///     <para>
        ///         Implementations should be tolerant to missing properties to preserve compatibility with older serialized
        ///         scenes (use <see cref="IObjectReader.IsDefined" /> and fall back to defaults). Use
        ///         <see cref="IObjectReader.IsNull" /> when supporting nullable properties.
        ///     </para>
        ///     <para>
        ///         When overriding this method in derived class, be sure to call the base class's <see cref="Deserialize" />
        ///         method so data of base class is deserialized as well.
        ///     </para>
        /// </remarks>
        /// <param name="reader">Instance of <see cref="IComponentDataReader" /> provided for reading component data.</param>
        /// <param name="assetStore">
        ///     Instance of <see cref="IAssetStore" /> provided to resolve <see cref="AssetId" /> values or access assets.
        /// </param>
        protected internal virtual void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
        {
        }
    }
}