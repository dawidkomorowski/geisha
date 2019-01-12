using System;

namespace Geisha.Engine.Core.SceneModel.Serialization
{
    /// <summary>
    ///     Marks property of component to be mapped and serialized automatically. Component should be marked with
    ///     <see cref="SerializableComponentAttribute" />.
    /// </summary>
    /// <remarks>
    ///     Property types supported for automatic serialization are: <see cref="int" />, <see cref="double" />,
    ///     <see cref="string" />.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class SerializablePropertyAttribute : Attribute
    {
    }
}