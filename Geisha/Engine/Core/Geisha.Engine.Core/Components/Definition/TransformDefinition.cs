using Geisha.Common.Math.Definition;
using Geisha.Engine.Core.SceneModel.Definition;

namespace Geisha.Engine.Core.Components.Definition
{
    /// <summary>
    ///     Represents serializable <see cref="Transform" /> that is used in a scene file content.
    /// </summary>
    public sealed class TransformDefinition : IComponentDefinition
    {
        /// <summary>
        ///     Defines <see cref="Transform.Translation" /> property of <see cref="Transform" />.
        /// </summary>
        public Vector3Definition Translation { get; set; }

        /// <summary>
        ///     Defines <see cref="Transform.Rotation" /> property of <see cref="Transform" />.
        /// </summary>
        public Vector3Definition Rotation { get; set; }

        /// <summary>
        ///     Defines <see cref="Transform.Scale" /> property of <see cref="Transform" />.
        /// </summary>
        public Vector3Definition Scale { get; set; }
    }
}