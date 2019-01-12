using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Geisha.Engine.Core.SceneModel.Serialization
{
    internal class AutomaticComponentDefinitionMapper : ISerializableComponentMapper
    {
        private readonly Type[] _supportedTypes = {typeof(int), typeof(double), typeof(string)};

        public bool IsApplicableForComponent(IComponent component)
        {
            return component.GetType().GetCustomAttribute<SerializableComponentAttribute>() != null;
        }

        public bool IsApplicableForSerializableComponent(ISerializableComponent serializableComponent)
        {
            return serializableComponent is AutomaticComponentDefinition;
        }

        public ISerializableComponent MapToSerializable(IComponent component)
        {
            var properties = GetProperties(component).ToArray();
            var intProperties = properties.Where(p => p.PropertyType == typeof(int));
            var doubleProperties = properties.Where(p => p.PropertyType == typeof(double));
            var stringProperties = properties.Where(p => p.PropertyType == typeof(string));

            return new AutomaticComponentDefinition
            {
                ComponentType = GetComponentType(component),
                IntProperties = intProperties.ToDictionary(p => p.Name, p => (int) p.GetValue(component)),
                DoubleProperties = doubleProperties.ToDictionary(p => p.Name, p => (double) p.GetValue(component)),
                StringProperties = stringProperties.ToDictionary(p => p.Name, p => (string) p.GetValue(component))
            };
        }

        public IComponent MapFromSerializable(ISerializableComponent serializableComponent)
        {
            var automaticComponentDefinition = (AutomaticComponentDefinition) serializableComponent;
            var componentType = Type.GetType(automaticComponentDefinition.ComponentType);

            if (componentType == null)
                throw new InvalidOperationException($"Type {automaticComponentDefinition.ComponentType} could not be created.");

            var component = (IComponent) Activator.CreateInstance(componentType);

            var properties = GetProperties(component).ToArray();
            var intProperties = properties.Where(p => p.PropertyType == typeof(int));
            var doubleProperties = properties.Where(p => p.PropertyType == typeof(double));
            var stringProperties = properties.Where(p => p.PropertyType == typeof(string));

            foreach (var property in intProperties)
            {
                property.SetValue(component, automaticComponentDefinition.IntProperties[property.Name]);
            }

            foreach (var property in doubleProperties)
            {
                property.SetValue(component, automaticComponentDefinition.DoubleProperties[property.Name]);
            }

            foreach (var property in stringProperties)
            {
                property.SetValue(component, automaticComponentDefinition.StringProperties[property.Name]);
            }

            return component;
        }

        private IEnumerable<PropertyInfo> GetProperties(IComponent component)
        {
            var properties = component.GetType().GetProperties().Where(p => p.GetCustomAttribute<SerializablePropertyAttribute>() != null).ToList();

            var unsupportedProperty = properties.FirstOrDefault(p => !_supportedTypes.Contains(p.PropertyType));
            if (unsupportedProperty != null)
            {
                throw new GeishaEngineException(
                    $"Component contains property of unsupported type. Component type: {component.GetType().FullName}, Property type: {unsupportedProperty.PropertyType.FullName}, Property name: {unsupportedProperty.Name}. Following types are supported: {_supportedTypes.Skip(1).Select(t => t.FullName).Aggregate($"{_supportedTypes.First().FullName}", (s, n) => $"{s}, {n}")}.");
            }

            return properties;
        }

        // TODO Introduce TypeDefinition class in Geisha.Common.Serialization that enables serializing/deserializing types.
        private static string GetComponentType(IComponent component)
        {
            var componentType = component.GetType();
            return $"{componentType.FullName}, {componentType.Assembly.GetName().Name}";
        }
    }
}