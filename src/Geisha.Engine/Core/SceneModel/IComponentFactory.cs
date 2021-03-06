using System;

namespace Geisha.Engine.Core.SceneModel
{
    public interface IComponentFactory
    {
        Type ComponentType { get; }
        ComponentId ComponentId { get; }

        IComponent Create();
    }
}