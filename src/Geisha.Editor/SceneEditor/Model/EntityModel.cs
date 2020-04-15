﻿using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Rendering.Components;

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
            var transformComponent = TransformComponent.CreateDefault();
            _entity.AddComponent(transformComponent);
            var transformComponentModel = new TransformComponentModel(transformComponent);
            _components.Add(transformComponentModel);

            ComponentAdded?.Invoke(this, new ComponentAddedEventArgs(transformComponentModel));
        }

        public void AddEllipseRendererComponent()
        {
            var ellipseRendererComponent = new EllipseRendererComponent();
            _entity.AddComponent(ellipseRendererComponent);
            var ellipseRendererComponentModel = new EllipseRendererComponentModel(ellipseRendererComponent);
            _components.Add(ellipseRendererComponentModel);

            ComponentAdded?.Invoke(this, new ComponentAddedEventArgs(ellipseRendererComponentModel));
        }

        public void AddRectangleRendererComponent()
        {
            var rectangleRendererComponent = new RectangleRendererComponent();
            _entity.AddComponent(rectangleRendererComponent);
            var rectangleRendererComponentModel = new RectangleRendererComponentModel(rectangleRendererComponent);
            _components.Add(rectangleRendererComponentModel);

            ComponentAdded?.Invoke(this, new ComponentAddedEventArgs(rectangleRendererComponentModel));
        }

        public void AddTextRendererComponent()
        {
            var textRendererComponent = new TextRendererComponent();
            _entity.AddComponent(textRendererComponent);
            var textRendererComponentModel = new TextRendererComponentModel(textRendererComponent);
            _components.Add(textRendererComponentModel);

            ComponentAdded?.Invoke(this, new ComponentAddedEventArgs(textRendererComponentModel));
        }

        public void AddCircleColliderComponent()
        {
            var circleColliderComponent = new CircleColliderComponent();
            _entity.AddComponent(circleColliderComponent);
            var circleColliderComponentModel = new CircleColliderComponentModel(circleColliderComponent);
            _components.Add(circleColliderComponentModel);

            ComponentAdded?.Invoke(this, new ComponentAddedEventArgs(circleColliderComponentModel));
        }

        public void AddRectangleColliderComponent()
        {
            var rectangleColliderComponent = new RectangleColliderComponent();
            _entity.AddComponent(rectangleColliderComponent);
            var rectangleColliderComponentModel = new RectangleColliderComponentModel(rectangleColliderComponent);
            _components.Add(rectangleColliderComponentModel);

            ComponentAdded?.Invoke(this, new ComponentAddedEventArgs(rectangleColliderComponentModel));
        }

        private string NextEntityName() => $"Child entity {_entityNameCounter++}";

        private IComponentModel CreateComponentModel(IComponent component)
        {
            switch (component)
            {
                case TransformComponent transformComponent:
                    return new TransformComponentModel(transformComponent);
                case EllipseRendererComponent ellipseRendererComponent:
                    return new EllipseRendererComponentModel(ellipseRendererComponent);
                case RectangleRendererComponent rectangleRendererComponent:
                    return new RectangleRendererComponentModel(rectangleRendererComponent);
                case TextRendererComponent textRendererComponent:
                    return new TextRendererComponentModel(textRendererComponent);
                case CircleColliderComponent circleColliderComponent:
                    return new CircleColliderComponentModel(circleColliderComponent);
                case RectangleColliderComponent rectangleColliderComponent:
                    return new RectangleColliderComponentModel(rectangleColliderComponent);
                default:
                    throw new ArgumentOutOfRangeException(nameof(component), $"Component of type {component.GetType()} is not supported.");
            }
        }
    }
}