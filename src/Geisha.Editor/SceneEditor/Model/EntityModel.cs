using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Editor.SceneEditor.Model
{
    public sealed class EntityModel
    {
        private readonly Entity _entity;
        private readonly List<EntityModel> _children;
        private readonly List<IComponentModel> _components;
        private int _entityNameCounter = 1;

        public EntityModel(Entity entity)
        {
            _entity = entity;
            _children = _entity.Children.Select(e => new EntityModel(e)).ToList();
            _components = _entity.Components.Select(CreateComponentModel).ToList();
        }

        public string Name
        {
            get => _entity.Name;
            set
            {
                if (_entity.Name != value)
                {
                    var eventArgs = new PropertyChangedEventArgs<string>(_entity.Name, value);
                    _entity.Name = value;
                    NameChanged?.Invoke(this, eventArgs);
                }
            }
        }

        public IReadOnlyCollection<EntityModel> Children => _children.AsReadOnly();
        public IReadOnlyCollection<IComponentModel> Components => _components.AsReadOnly();

        public event EventHandler<PropertyChangedEventArgs<string>> NameChanged;
        public event EventHandler<EntityAddedEventArgs> EntityAdded;
        public event EventHandler<ComponentAddedEventArgs> ComponentAdded;

        public void AddChildEntity()
        {
            var entity = new Entity();
            _entity.AddChild(entity);

            var entityModel = new EntityModel(entity);
            _children.Add(entityModel);

            entityModel.Name = NextEntityName();

            EntityAdded?.Invoke(this, new EntityAddedEventArgs(entityModel));
        }

        public void AddTransformComponent()
        {
            var transformComponent = new TransformComponent();
            _entity.AddComponent(transformComponent);
            var transformComponentModel = new TransformComponentModel(transformComponent);
            _components.Add(transformComponentModel);

            ComponentAdded?.Invoke(this, new ComponentAddedEventArgs(transformComponentModel));
        }

        private string NextEntityName() => $"Child entity {_entityNameCounter++}";

        private IComponentModel CreateComponentModel(IComponent component)
        {
            switch (component)
            {
                case TransformComponent transformComponent:
                    return new TransformComponentModel(transformComponent);
                default:
                    throw new ArgumentOutOfRangeException(nameof(component), $"Component of type {component.GetType()} is not supported.");
            }
        }
    }
}