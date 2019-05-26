namespace Geisha.Engine.Core.SceneModel.Serialization
{
    /// <inheritdoc />
    /// <summary>
    ///     Adapter class simplifying implementation of <see cref="ISerializableComponentMapper" />.
    /// </summary>
    /// <typeparam name="TComponent">Component type the mapper supports.</typeparam>
    /// <typeparam name="TSerializableComponent">Serializable component type the mapper supports.</typeparam>
    public abstract class SerializableComponentMapperAdapter<TComponent, TSerializableComponent> : ISerializableComponentMapper
        where TComponent : IComponent where TSerializableComponent : ISerializableComponent
    {
        /// <inheritdoc />
        /// <summary>
        ///     Tests applicability of this mapper for given component.
        /// </summary>
        public bool IsApplicableForComponent(IComponent component)
        {
            return component is TComponent;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Tests applicability of this mapper for given serializable component.
        /// </summary>
        public bool IsApplicableForSerializableComponent(ISerializableComponent serializableComponent)
        {
            return serializableComponent is TSerializableComponent;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Maps <see cref="IComponent" /> to <see cref="ISerializableComponent" />.
        /// </summary>
        public ISerializableComponent MapToSerializable(IComponent component)
        {
            return MapToSerializable((TComponent) component);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Maps <see cref="ISerializableComponent" /> to <see cref="IComponent" />.
        /// </summary>
        public IComponent MapFromSerializable(ISerializableComponent serializableComponent)
        {
            return MapFromSerializable((TSerializableComponent) serializableComponent);
        }

        /// <summary>
        ///     Maps <typeparamref name="TComponent" /> to <typeparamref name="TSerializableComponent" />.
        /// </summary>
        /// <param name="component">Component to be mapped.</param>
        /// <returns>Serializable component that is equivalent of given component.</returns>
        protected abstract TSerializableComponent MapToSerializable(TComponent component);

        /// <summary>
        ///     Maps <typeparamref name="TSerializableComponent" /> to <typeparamref name="TComponent" />.
        /// </summary>
        /// <param name="serializableComponent">Serializable component to be mapped.</param>
        /// <returns>Component that is equivalent of given serializable component.</returns>
        protected abstract TComponent MapFromSerializable(TSerializableComponent serializableComponent);
    }
}