using System;

namespace Geisha.Engine.Core.SceneModel.Serialization
{
    /// <summary>
    ///     Marks component type to use <see cref="AutomaticComponentDefinition" /> as serializable representation.
    /// </summary>
    /// <remarks>
    ///     Component marked with <see cref="ComponentDefinitionAttribute" /> is automatically mapped to
    ///     <see cref="AutomaticComponentDefinition" /> and does not require custom implementation of
    ///     <see cref="IComponentDefinition" /> interface. Use <see cref="PropertyDefinitionAttribute" /> to mark properties
    ///     that should be mapped and serialized automatically.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentDefinitionAttribute : Attribute
    {
    }
}