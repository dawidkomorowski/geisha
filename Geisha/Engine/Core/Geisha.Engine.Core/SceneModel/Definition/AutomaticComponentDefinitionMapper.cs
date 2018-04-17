using System;
using System.ComponentModel.Composition;
using System.Reflection;

namespace Geisha.Engine.Core.SceneModel.Definition
{
    [Export(typeof(IComponentDefinitionMapper))]
    internal class AutomaticComponentDefinitionMapper : IComponentDefinitionMapper
    {
        public bool IsApplicableForComponent(IComponent component)
        {
            return component.GetType().GetCustomAttribute<UseAutomaticComponentDefinitionAttribute>() != null;
        }

        public bool IsApplicableForComponentDefinition(IComponentDefinition componentDefinition)
        {
            return componentDefinition is AutomaticComponentDefinition;
        }

        public IComponentDefinition ToDefinition(IComponent component)
        {
            throw new NotImplementedException();
        }

        public IComponent FromDefinition(IComponentDefinition componentDefinition)
        {
            throw new NotImplementedException();
        }
    }
}