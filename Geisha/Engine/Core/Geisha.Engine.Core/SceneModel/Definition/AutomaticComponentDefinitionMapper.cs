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
            return component.GetType().GetCustomAttribute<ComponentDefinitionAttribute>() != null;
        }

        public bool IsApplicableForComponentDefinition(IComponentDefinition componentDefinition)
        {
            return componentDefinition is AutomaticComponentDefinition;
        }

        public IComponentDefinition ToDefinition(IComponent component)
        {
            return new AutomaticComponentDefinition
            {
                ComponentType = GetComponentType(component),
                Properties = GetProperties(component).ToDictionary(p => p.Name, p => p.GetValue(component))
            };
        }

        public IComponent FromDefinition(IComponentDefinition componentDefinition)
        {
            var automaticComponentDefinition = (AutomaticComponentDefinition) componentDefinition;
            var componentType = Type.GetType(automaticComponentDefinition.ComponentType);

            if (componentType == null)
            {
                throw new InvalidOperationException($"Type {automaticComponentDefinition.ComponentType} could not be created.");
            }

            var component = (IComponent) Activator.CreateInstance(componentType);

            foreach (var property in GetProperties(component))
            {
                property.SetValue(component, automaticComponentDefinition.Properties[property.Name]);
            }

            return component;
        }

        private IEnumerable<PropertyInfo> GetProperties(IComponent component)
        {
            var properties = component.GetType().GetProperties().Where(p => p.GetCustomAttribute<PropertyDefinitionAttribute>() != null).ToList();

            var unsupportedProperty = properties.FirstOrDefault(p => !_supportedTypes.Contains(p.PropertyType));
            if (unsupportedProperty != null)
            {
                throw new GeishaEngineException(
                    $"Component contains property of unsupported type. Component type: {component.GetType().FullName}, Property type: {unsupportedProperty.PropertyType.FullName}, Property name: {unsupportedProperty.Name}. Following types are supported: {_supportedTypes.Skip(1).Select(t => t.FullName).Aggregate($"{_supportedTypes.First().FullName}", (s, n) => $"{s}, {n}")}.");
            }

            return properties;
        }

        private static string GetComponentType(IComponent component)
        {
            var componentType = component.GetType();
            return $"{componentType.FullName}, {componentType.Assembly.GetName().Name}";
        }
    }
}