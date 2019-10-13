using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Editor.Core;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Editor.SceneEditor.Model
{
    public sealed class EntityModel
    {
        private readonly Entity _entity;
        private readonly List<EntityModel> _children;
        private int _entityNameCounter = 1;

        public EntityModel(Entity entity)
        {
            _entity = entity;
            _children = _entity.Children.Select(e => new EntityModel(e)).ToList();
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

        public event EventHandler<EntityAddedEventArgs> EntityAdded;
        public event EventHandler<PropertyChangedEventArgs<string>> NameChanged;

        public void AddChildEntity()
        {
            var entity = new Entity();
            _entity.AddChild(entity);

            var entityModel = new EntityModel(entity);
            _children.Add(entityModel);

            entityModel.Name = NextEntityName();

            EntityAdded?.Invoke(this, new EntityAddedEventArgs(entityModel));
        }

        private string NextEntityName() => $"Child entity {_entityNameCounter++}";
    }
}