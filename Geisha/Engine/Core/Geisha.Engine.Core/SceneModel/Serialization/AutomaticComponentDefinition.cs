using System.Collections.Generic;

namespace Geisha.Engine.Core.SceneModel.Serialization
{
    /// <summary>
    ///     Serializable representation of any component implementing <see cref="IComponent" /> interface that is used in a
    ///     scene file content.
    /// </summary>
    /// <remarks>
    ///     <see cref="AutomaticComponentDefinition" /> is component definition that is created automatically for
    ///     component types marked with <see cref="ComponentDefinitionAttribute" />. Only properties marked with
    ///     <see cref="PropertyDefinitionAttribute" /> are mapped respectively to <see cref="IntProperties" />,
    ///     <see cref="DoubleProperties" /> or <see cref="StringProperties" />.
    /// </remarks>
    public sealed class AutomaticComponentDefinition : IComponentDefinition
    {
        /// <summary>
        ///     Assembly qualified name of component type.
        /// </summary>
        /// <remarks>This assembly qualified name does not contain assembly version, culture info and processor architecture.</remarks>
        public string ComponentType { get; set; }

        /// <summary>
        ///     Integer properties of component. Key is property name, value is property value.
        /// </summary>
        public Dictionary<string, int> IntProperties { get; set; }

        /// <summary>
        ///     Double properties of component. Key is property name, value is property value.
        /// </summary>
        public Dictionary<string, double> DoubleProperties { get; set; }

        /// <summary>
        ///     String properties of component. Key is property name, value is property value.
        /// </summary>
        public Dictionary<string, string> StringProperties { get; set; }
    }
}