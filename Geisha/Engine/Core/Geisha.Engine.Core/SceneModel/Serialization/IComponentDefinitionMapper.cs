namespace Geisha.Engine.Core.SceneModel.Serialization
{
    /// <summary>
    ///     Provides functionality to map between <see cref="IComponent" /> and <see cref="IComponentDefinition" /> in both
    ///     directions.
    /// </summary>
    /// <remarks>
    ///     For each component that requires custom component definition the implementation of this interface must be provided.
    ///     There can be only single implementation per component type.
    /// </remarks>
    public interface IComponentDefinitionMapper
    {
        /// <summary>
        ///     Tests applicability of this mapper for given component.
        /// </summary>
        /// <param name="component">Component instance.</param>
        /// <returns>True, if given component is supported by mapper; false otherwise.</returns>
        bool IsApplicableForComponent(IComponent component);

        /// <summary>
        ///     Tests applicability of this mapper for given component definition.
        /// </summary>
        /// <param name="componentDefinition">Component definition instance.</param>
        /// <returns>True, if given component definition is supported by mapper; false otherwise. </returns>
        bool IsApplicableForComponentDefinition(IComponentDefinition componentDefinition);

        /// <summary>
        ///     Maps <see cref="IComponent" /> to <see cref="IComponentDefinition" />.
        /// </summary>
        /// <param name="component">Component to be mapped.</param>
        /// <returns>Component definition that is equivalent of given component.</returns>
        IComponentDefinition ToDefinition(IComponent component);

        /// <summary>
        ///     Maps <see cref="IComponentDefinition" /> to <see cref="IComponent" />.
        /// </summary>
        /// <param name="componentDefinition">Component definition to be mapped.</param>
        /// <returns>Component that is equivalent of given component definition.</returns>
        IComponent FromDefinition(IComponentDefinition componentDefinition);
    }
}