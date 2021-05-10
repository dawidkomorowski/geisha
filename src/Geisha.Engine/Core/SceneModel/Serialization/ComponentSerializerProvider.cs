using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geisha.Engine.Core.SceneModel.Serialization
{
    internal interface IComponentSerializerProvider
    {
        IComponentSerializer Get(ComponentId componentId);
    }

    internal sealed class ComponentSerializerProvider : IComponentSerializerProvider
    {
        private readonly Dictionary<ComponentId, IComponentSerializer> _serializersById;

        public ComponentSerializerProvider(IEnumerable<IComponentSerializer> serializers)
        {
            var serializersArray = serializers as IComponentSerializer[] ?? serializers.ToArray();

            if (MultipleImplementationsValidator.ShouldThrow(serializersArray, serializer => serializer.ComponentId, out var exceptionMessage))
            {
                throw new ArgumentException(exceptionMessage, nameof(serializers));
            }

            _serializersById = serializersArray.ToDictionary(s => s.ComponentId);
        }

        public IComponentSerializer Get(ComponentId componentId)
        {
            if (_serializersById.TryGetValue(componentId, out var serializer))
            {
                return serializer;
            }

            throw new ComponentSerializerNotFoundException(componentId, _serializersById.Values);
        }
    }

    public sealed class ComponentSerializerNotFoundException : Exception
    {
        public ComponentSerializerNotFoundException(ComponentId componentId, IReadOnlyCollection<IComponentSerializer> componentSerializers) : base(
            GetMessage(componentId, Sorted(componentSerializers)))
        {
            ComponentId = componentId;
            ComponentSerializers = Sorted(componentSerializers);
        }

        public ComponentId ComponentId { get; }
        public IEnumerable<IComponentSerializer> ComponentSerializers { get; }

        private static string GetMessage(ComponentId componentId, IEnumerable<IComponentSerializer> serializers)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"No implementation of {nameof(IComponentSerializer)} for component id \"{componentId}\" was found.");
            stringBuilder.AppendLine("Available serializers:");

            foreach (var serializer in serializers)
            {
                stringBuilder.AppendLine($"- {serializer.GetType().FullName} for component id \"{serializer.ComponentId}\"");
            }

            return stringBuilder.ToString();
        }

        private static IEnumerable<IComponentSerializer> Sorted(IEnumerable<IComponentSerializer> factories)
        {
            return factories.OrderBy(f => f.GetType().FullName);
        }
    }
}