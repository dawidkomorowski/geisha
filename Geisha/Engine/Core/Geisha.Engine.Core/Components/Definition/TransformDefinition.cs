using Geisha.Common.Math.Definition;
using Geisha.Engine.Core.SceneModel.Definition;

namespace Geisha.Engine.Core.Components.Definition
{
    /// <inheritdoc />
    /// <summary>
    ///     Represents serializable <see cref="Transform" /> that is used in a scene file content.
    /// </summary>
    public class TransformDefinition : IComponentDefinition
    {
        public Vector3Definition Translation { get; set; }
        public Vector3Definition Rotation { get; set; }
        public Vector3Definition Scale { get; set; }
    }
}