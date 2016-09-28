using System.Collections.Generic;
using Geisha.Engine.Core.Components;

namespace Geisha.Engine.Core
{
    public interface IEntity
    {
        IList<IComponent> Components { get; }
        TComponent GetComponent<TComponent>() where TComponent : IComponent;
        bool HasComponent<TComponent>() where TComponent : IComponent;
        void AddComponent(IComponent component);
        void RemoveComponent(IComponent component);
    }
}