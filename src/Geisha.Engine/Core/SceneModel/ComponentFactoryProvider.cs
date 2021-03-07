using System;
using System.Collections.Generic;
using System.Linq;

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
        public ComponentFactoryProvider(IEnumerable<IComponentFactory> factories)
        {
            var factoriesArray = factories as IComponentFactory[] ?? factories.ToArray();

            if (MultipleImplementationsValidator.ShouldThrow(factoriesArray, factory => factory.ComponentType,
                out var componentTypeExceptionMessage))
            {
                throw new ArgumentException(componentTypeExceptionMessage, nameof(factories));
            }

            if (MultipleImplementationsValidator.ShouldThrow(factoriesArray, factory => factory.ComponentId,
                id => id.Value, out var componentIdExceptionMessage))
            {
                throw new ArgumentException(componentIdExceptionMessage, nameof(factories));
            }
        }

        public IComponentFactory Get<TComponent>() where TComponent : IComponent => throw new NotImplementedException();

        public IComponentFactory Get(Type componentType) => throw new NotImplementedException();

        public IComponentFactory Get(ComponentId componentId) => throw new NotImplementedException();
    }
}