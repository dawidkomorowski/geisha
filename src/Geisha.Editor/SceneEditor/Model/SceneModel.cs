using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Editor.SceneEditor.Model
{
    public sealed class SceneModel
    {
        private readonly ISceneBehaviorFactoryProvider _sceneBehaviorFactoryProvider;
        private readonly Scene _scene;
        private readonly List<EntityModel> _entities;
        private int _entityNameCounter = 1;

        public SceneModel(Scene scene, IEnumerable<SceneBehaviorName> availableSceneBehaviors, ISceneBehaviorFactoryProvider sceneBehaviorFactoryProvider)
        {
            _scene = scene;
            _entities = _scene.RootEntities.Select(e => new EntityModel(e)).ToList();
            _sceneBehaviorFactoryProvider = sceneBehaviorFactoryProvider;

            AvailableSceneBehaviors = availableSceneBehaviors.ToList().AsReadOnly();
        }

        public IReadOnlyCollection<SceneBehaviorName> AvailableSceneBehaviors { get; }

        public SceneBehaviorName SceneBehavior
        {
            get => new SceneBehaviorName(_scene.SceneBehavior.Name);
            set => _scene.SceneBehavior = _sceneBehaviorFactoryProvider.Get(value.Value).Create(_scene);
        }

        public IReadOnlyCollection<EntityModel> RootEntities => _entities.AsReadOnly();

        public event EventHandler<EntityAddedEventArgs>? EntityAdded;

        public void AddEntity()
        {
            var entity = _scene.CreateEntity();
            var entityModel = new EntityModel(entity);
            _entities.Add(entityModel);

            entityModel.Name = NextEntityName();

            EntityAdded?.Invoke(this, new EntityAddedEventArgs(entityModel));
        }

        private string NextEntityName() => $"Entity {_entityNameCounter++}";
    }
}