using System;

namespace Geisha.Engine.Core.SceneModel.Definition
{
    /// <summary>
    ///     Marks property of component to be mapped and serialized automatically. Component should be marked with
    ///     <see cref="ComponentDefinitionAttribute" />.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyDefinitionAttribute : Attribute
    {
    }
}