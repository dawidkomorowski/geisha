using System;

namespace Geisha.Engine.Core.SceneModel.Definition
{
    /// <inheritdoc />
    /// <summary>
    /// Adapter class simplifying implementation of <see cref="T:Geisha.Engine.Core.SceneModel.Definition.IComponentDefinitionMapper" />.
    /// </summary>
    /// <typeparam name="TComponent">Component type the mapper supports.</typeparam>
    /// <typeparam name="TComponentDefinition">Component definition type the mapper supports.</typeparam>
    public abstract class ComponentDefinitionMapperAdapter<TComponent, TComponentDefinition> : IComponentDefinitionMapper
        where TComponent : IComponent where TComponentDefinition : IComponentDefinition
    {
        public Type ComponentType => typeof(TComponent);
        public Type ComponentDefinitionType => typeof(TComponentDefinition);

        public IComponentDefinition ToDefinition(IComponent component)
        {
            return ToDefinition((TComponent) component);
        }

        public IComponent FromDefinition(IComponentDefinition componentDefinition)
        {
            return FromDefinition((TComponentDefinition) componentDefinition);
        }

        protected abstract TComponentDefinition ToDefinition(TComponent component);
        protected abstract TComponent FromDefinition(TComponentDefinition componentDefinition);
    }
}