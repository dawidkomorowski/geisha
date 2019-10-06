using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Editor.SceneEditor.Model
{
    public sealed class SceneModel
    {
        private readonly Scene _scene;
        private readonly List<EntityModel> _entities;
        private int _entityNameCounter = 1;

        public SceneModel(Scene scene)
        {
            _scene = scene;
            _entities = _scene.RootEntities.Select(e => new EntityModel(e)).ToList();
        }

        public IReadOnlyCollection<EntityModel> RootEntities => _entities.AsReadOnly();

        public event EventHandler<EntityAddedEventArgs> EntityAdded;

        public void AddEntity()
        {
            var entity = new Entity();
            _scene.AddEntity(entity);

            var entityModel = new EntityModel(entity);
            _entities.Add(entityModel);

            entityModel.Name = NextEntityName();

            EntityAdded?.Invoke(this, new EntityAddedEventArgs(entityModel));
        }

        private string NextEntityName() => $"Entity {_entityNameCounter++}";
    }
}