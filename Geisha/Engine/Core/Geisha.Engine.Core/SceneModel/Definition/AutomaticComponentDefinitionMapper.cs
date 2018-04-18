using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;

namespace Geisha.Engine.Core.SceneModel.Definition
{
    [Export(typeof(IComponentDefinitionMapper))]
    internal class AutomaticComponentDefinitionMapper : IComponentDefinitionMapper
    {
        private readonly Type[] _supportedTypes = {typeof(int), typeof(double), typeof(string)};

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
                Properties = GetProperties(component).ToDictionary(p => p.Name, p => p.GetValue(component))
            };
        }

        public IComponent FromDefinition(IComponentDefinition componentDefinition)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<PropertyInfo> GetProperties(IComponent component)
        {
            var properties = component.GetType().GetProperties();

            var unsupportedProperty = properties.FirstOrDefault(p => !_supportedTypes.Contains(p.PropertyType));
            if (unsupportedProperty != null)
            {
                throw new GeishaEngineException(
                    $"Component contains property of not supported type. Component type: {component.GetType().FullName}, Property type: {unsupportedProperty.PropertyType.FullName}, Property name: {unsupportedProperty.Name}.");
            }

            return properties;
        }
    }
}