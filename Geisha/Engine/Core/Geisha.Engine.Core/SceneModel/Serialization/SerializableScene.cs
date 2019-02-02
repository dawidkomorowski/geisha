﻿using System.Collections.Generic;

namespace Geisha.Engine.Core.SceneModel.Serialization
{
    /// <summary>
    ///     Represents serializable <see cref="Scene" /> that is used as a scene file content.
    /// </summary>
    public sealed class SerializableScene
    {
        /// <summary>
        ///     Represents <see cref="Scene.RootEntities" /> property of <see cref="Scene" />.
        /// </summary>
        public List<SerializableEntity> RootEntities { get; set; }

        /// <summary>
        ///     Represents <see cref="Scene.ConstructionScript" /> property of <see cref="Scene" />.
        /// </summary>
        public string ConstructionScript { get; set; }
    }
}