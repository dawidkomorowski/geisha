using System;

namespace Geisha.Engine.Core.SceneModel
{
    public interface IComponentDefinitionMapper
    {
        Type ComponentType { get; }
        Type ComponentDefinitionType { get; }
        IComponentDefinition ToDefinition(IComponent component);
        IComponent FromDefinition(IComponentDefinition componentDefinition);
    }
}