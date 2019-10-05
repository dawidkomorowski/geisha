using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Editor.SceneEditor.Model
{
    public sealed class SceneModel
    {
        private readonly Scene _scene;
        private readonly List<EntityModel> _entities;

        public SceneModel(Scene scene)
        {
            _scene = scene;
            _entities = _scene.RootEntities.Select(e => new EntityModel(e)).ToList();
        }

        public IReadOnlyCollection<EntityModel> RootEntities => _entities.AsReadOnly();
    }
}