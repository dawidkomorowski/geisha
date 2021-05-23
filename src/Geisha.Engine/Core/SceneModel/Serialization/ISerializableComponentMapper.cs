namespace Geisha.Engine.Core.SceneModel.Serialization
{
    /// <summary>
    ///     Provides functionality to map between <see cref="Component" /> and <see cref="ISerializableComponent" /> in both
    ///     directions.
    /// </summary>
    /// <remarks>
    ///     For each component that requires custom serializable component the implementation of this interface must be
    ///     provided.
    ///     There can be only single implementation per component type.
    /// </remarks>
    public interface ISerializableComponentMapper
    {
        /// <summary>
        ///     Tests applicability of this mapper for given component.
        /// </summary>
        /// <param name="component">Component instance.</param>
        /// <returns>True, if given component is supported by mapper; false otherwise.</returns>
        bool IsApplicableForComponent(Component component);

        /// <summary>
        ///     Tests applicability of this mapper for given serializable component.
        /// </summary>
        /// <param name="serializableComponent">Serializable component instance.</param>
        /// <returns>True, if given serializable component is supported by mapper; false otherwise.</returns>
        bool IsApplicableForSerializableComponent(ISerializableComponent serializableComponent);

        /// <summary>
        ///     Maps <see cref="Component" /> to <see cref="ISerializableComponent" />.
        /// </summary>
        /// <param name="component">Component to be mapped.</param>
        /// <returns>Serializable component that is equivalent of given component.</returns>
        ISerializableComponent MapToSerializable(Component component);

        /// <summary>
        ///     Maps <see cref="ISerializableComponent" /> to <see cref="Component" />.
        /// </summary>
        /// <param name="serializableComponent">Serializable component to be mapped.</param>
        /// <returns>Component that is equivalent of given serializable component.</returns>
        Component MapFromSerializable(ISerializableComponent serializableComponent);
    }
}