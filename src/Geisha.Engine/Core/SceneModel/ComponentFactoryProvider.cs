using System;

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
        public IComponentFactory Get<TComponent>() where TComponent : IComponent => throw new NotImplementedException();

        public IComponentFactory Get(Type componentType) => throw new NotImplementedException();

        public IComponentFactory Get(ComponentId componentId) => throw new NotImplementedException();
    }
}