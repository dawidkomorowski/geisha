using System;
using System.Collections.Generic;
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
            return new AutomaticComponentDefinition
            {
                ComponentTypeFullName = component.GetType().FullName,
                Properties = new Dictionary<string, object>()
            };
        }

        public IComponent FromDefinition(IComponentDefinition componentDefinition)
        {
            throw new NotImplementedException();
        }
    }
}