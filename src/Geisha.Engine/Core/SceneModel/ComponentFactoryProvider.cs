using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geisha.Engine.Core.SceneModel
{
    internal interface IComponentFactoryProvider
    {
        IComponentFactory Get<TComponent>() where TComponent : IComponent;
        IComponentFactory Get(Type componentType);
        IComponentFactory Get(ComponentId componentId);
    }

    internal sealed class ComponentFactoryProvider : IComponentFactoryProvider
    {
        private readonly Dictionary<Type, IComponentFactory> _factoriesByType;
        private readonly Dictionary<ComponentId, IComponentFactory> _factoriesById;

        public ComponentFactoryProvider(IEnumerable<IComponentFactory> factories)
        {
            var factoriesArray = factories as IComponentFactory[] ?? factories.ToArray();

            if (MultipleImplementationsValidator.ShouldThrow(factoriesArray, factory => factory.ComponentType,
                out var componentTypeExceptionMessage))
            {
                throw new ArgumentException(componentTypeExceptionMessage, nameof(factories));
            }

            if (MultipleImplementationsValidator.ShouldThrow(factoriesArray, factory => factory.ComponentId, out var componentIdExceptionMessage))
            {
                throw new ArgumentException(componentIdExceptionMessage, nameof(factories));
            }

            _factoriesByType = factoriesArray.ToDictionary(f => f.ComponentType);
            _factoriesById = factoriesArray.ToDictionary(f => f.ComponentId);
        }

        public IComponentFactory Get<TComponent>() where TComponent : IComponent => Get(typeof(TComponent));

        public IComponentFactory Get(Type componentType)
        {
            if (_factoriesByType.TryGetValue(componentType, out var factory))
            {
                return factory;
            }

            throw new ComponentFactoryNotFoundException(componentType, _factoriesByType.Values);
        }

        public IComponentFactory Get(ComponentId componentId)
        {
            if (_factoriesById.TryGetValue(componentId, out var factory))
            {
                return factory;
            }

            throw new ComponentFactoryNotFoundException(componentId, _factoriesById.Values);
        }
    }

    /// <summary>
    ///     The exception that is thrown when no implementation of <see cref="IComponentFactory" /> was found for specified
    ///     component type or component id.
    /// </summary>
    public sealed class ComponentFactoryNotFoundException : Exception
    {
        /// <summary>
        ///     Creates new instance of <see cref="ComponentFactoryNotFoundException" /> class for specified <paramref name="componentType"/>.
        /// </summary>
        /// <param name="componentType">Type of component for which factory was not found.</param>
        /// <param name="componentFactories">Collection of all available factories.</param>
        public ComponentFactoryNotFoundException(Type componentType, IReadOnlyCollection<IComponentFactory> componentFactories) : base(
            GetMessage(componentType, Sorted(componentFactories)))
        {
            ComponentType = componentType;
            ComponentId = null;
            ComponentFactories = Sorted(componentFactories);
        }

        /// <summary>
        ///     Creates new instance of <see cref="ComponentFactoryNotFoundException" /> class for specified <paramref name="componentId"/>.
        /// </summary>
        /// <param name="componentId">Id of component for which factory was not found.</param>
        /// <param name="componentFactories">Collection of all available factories.</param>
        public ComponentFactoryNotFoundException(ComponentId componentId, IReadOnlyCollection<IComponentFactory> componentFactories) : base(
            GetMessage(componentId, Sorted(componentFactories)))
        {
            ComponentType = null;
            ComponentId = componentId;
            ComponentFactories = Sorted(componentFactories);
        }

        /// <summary>
        ///     Type of component for which factory was not found.
        /// </summary>
        public Type? ComponentType { get; }

        /// <summary>
        /// Id of component for which factory was not found.
        /// </summary>
        public ComponentId? ComponentId { get; }

        /// <summary>
        ///     Collection of all available factories.
        /// </summary>
        public IEnumerable<IComponentFactory> ComponentFactories { get; }

        private static string GetMessage(Type componentType, IEnumerable<IComponentFactory> factories)
        {
            return GetMessage($"No implementation of {nameof(IComponentFactory)} for component type \"{componentType}\" was found.", factories);
        }

        private static string GetMessage(ComponentId componentId, IEnumerable<IComponentFactory> factories)
        {
            return GetMessage($"No implementation of {nameof(IComponentFactory)} for component id \"{componentId}\" was found.", factories);
        }

        private static string GetMessage(string header, IEnumerable<IComponentFactory> factories)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(header);
            stringBuilder.AppendLine("Available factories:");

            foreach (var factory in factories)
            {
                stringBuilder.AppendLine(
                    $"- {factory.GetType().FullName} for component type \"{factory.ComponentType}\", component id \"{factory.ComponentId}\"");
            }

            return stringBuilder.ToString();
        }

        private static IEnumerable<IComponentFactory> Sorted(IEnumerable<IComponentFactory> factories)
        {
            return factories.OrderBy(f => f.GetType().FullName);
        }
    }
}