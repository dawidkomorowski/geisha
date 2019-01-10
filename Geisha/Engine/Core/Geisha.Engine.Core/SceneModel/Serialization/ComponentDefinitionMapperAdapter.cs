namespace Geisha.Engine.Core.SceneModel.Serialization
{
    /// <inheritdoc />
    /// <summary>
    ///     Adapter class simplifying implementation of <see cref="IComponentDefinitionMapper" />.
    /// </summary>
    /// <typeparam name="TComponent">Component type the mapper supports.</typeparam>
    /// <typeparam name="TComponentDefinition">Component definition type the mapper supports.</typeparam>
    public abstract class ComponentDefinitionMapperAdapter<TComponent, TComponentDefinition> : IComponentDefinitionMapper
        where TComponent : IComponent where TComponentDefinition : IComponentDefinition
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
        ///     Tests applicability of this mapper for given component definition.
        /// </summary>
        public bool IsApplicableForComponentDefinition(IComponentDefinition componentDefinition)
        {
            return componentDefinition is TComponentDefinition;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Maps <see cref="IComponent" /> to <see cref="IComponentDefinition" />.
        /// </summary>
        public IComponentDefinition ToDefinition(IComponent component)
        {
            return ToDefinition((TComponent) component);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Maps <see cref="IComponentDefinition" /> to <see cref="IComponent" />.
        /// </summary>
        public IComponent FromDefinition(IComponentDefinition componentDefinition)
        {
            return FromDefinition((TComponentDefinition) componentDefinition);
        }

        /// <summary>
        ///     Maps <typeparamref name="TComponent" /> to <typeparamref name="TComponentDefinition" />.
        /// </summary>
        /// <param name="component">Component to be mapped.</param>
        /// <returns>Component definition that is equivalent of given component.</returns>
        protected abstract TComponentDefinition ToDefinition(TComponent component);

        /// <summary>
        ///     Maps <typeparamref name="TComponentDefinition" /> to <typeparamref name="TComponent" />.
        /// </summary>
        /// <param name="componentDefinition">Component definition to be mapped.</param>
        /// <returns>Component that is equivalent of given component definition.</returns>
        protected abstract TComponent FromDefinition(TComponentDefinition componentDefinition);
    }
}