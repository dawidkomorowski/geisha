using System;

namespace Geisha.Engine.Core.SceneModel.Definition
{
    /// <summary>
    ///     Provides functionality to map between <see cref="IComponent" /> and <see cref="IComponentDefinition" /> in both
    ///     directions.
    /// </summary>
    /// <remarks>
    ///     Each component that requires custom component definition must provide implementation of this interface. There can
    ///     be only single implementation per component type.
    /// </remarks>
    public interface IComponentDefinitionMapper
    {
        /// <summary>
        ///     Type of component supported by mapper.
        /// </summary>
        Type ComponentType { get; }

        /// <summary>
        ///     Type of component definition supported by mapper.
        /// </summary>
        Type ComponentDefinitionType { get; }

        /// <summary>
        ///     Maps <see cref="IComponent" /> to <see cref="IComponentDefinition" />.
        /// </summary>
        /// <param name="component">Component to be mapped.</param>
        /// <returns><see cref="IComponentDefinition" /> that is equivalent of given component.</returns>
        IComponentDefinition ToDefinition(IComponent component);

        /// <summary>
        ///     Maps <see cref="IComponentDefinition" /> to <see cref="IComponent" />.
        /// </summary>
        /// <param name="componentDefinition">Component definition to be mapped.</param>
        /// <returns>Component that is equivalent of given component definition.</returns>
        IComponent FromDefinition(IComponentDefinition componentDefinition);
    }
}