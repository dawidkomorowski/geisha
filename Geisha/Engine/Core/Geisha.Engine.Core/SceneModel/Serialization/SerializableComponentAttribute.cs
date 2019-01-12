using System;

namespace Geisha.Engine.Core.SceneModel.Serialization
{
    /// <summary>
    ///     Marks component type to use <see cref="AutomaticComponentDefinition" /> as serializable representation.
    /// </summary>
    /// <remarks>
    ///     Component marked with <see cref="SerializableComponentAttribute" /> is automatically mapped to
    ///     <see cref="AutomaticComponentDefinition" /> and does not require custom implementation of
    ///     <see cref="ISerializableComponent" /> interface. Use <see cref="SerializablePropertyAttribute" /> to mark properties
    ///     that should be mapped and serialized automatically.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class SerializableComponentAttribute : Attribute
    {
    }
}