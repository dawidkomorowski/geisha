﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor
{
    internal sealed class EntityPropertiesEditorViewModel : ViewModel
    {
        private readonly EntityModel _entityModel;
        private readonly IComponentPropertiesEditorViewModelFactory _componentPropertiesEditorViewModelFactory;
        private readonly IProperty<string> _name;

        public EntityPropertiesEditorViewModel(EntityModel entityModel, IComponentPropertiesEditorViewModelFactory componentPropertiesEditorViewModelFactory)
        {
            _entityModel = entityModel;
            _componentPropertiesEditorViewModelFactory = componentPropertiesEditorViewModelFactory;
            _name = CreateProperty(nameof(Name), _entityModel.Name);

            Components = new ObservableCollection<ComponentPropertiesEditorViewModel>(_entityModel.Components.Select(c =>
                _componentPropertiesEditorViewModelFactory.Create(c)));

            AddTransform3DComponentCommand = RelayCommand.Create(AddTransform3DComponent);
            AddEllipseRendererComponentCommand = RelayCommand.Create(AddEllipseRendererComponent);
            AddRectangleRendererComponentCommand = RelayCommand.Create(AddRectangleRendererComponent);
            AddTextRendererComponentCommand = RelayCommand.Create(AddTextRendererComponent);
            AddCircleColliderComponentCommand = RelayCommand.Create(AddCircleColliderComponent);
            AddRectangleColliderComponentCommand = RelayCommand.Create(AddRectangleColliderComponent);

            _name.Subscribe(name => _entityModel.Name = name);

            _entityModel.ComponentAdded += EntityModelOnComponentAdded;
        }

        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }

        public ObservableCollection<ComponentPropertiesEditorViewModel> Components { get; }

        public ICommand AddTransform3DComponentCommand { get; }
        public ICommand AddEllipseRendererComponentCommand { get; }
        public ICommand AddRectangleRendererComponentCommand { get; }
        public ICommand AddTextRendererComponentCommand { get; }
        public ICommand AddCircleColliderComponentCommand { get; }
        public ICommand AddRectangleColliderComponentCommand { get; }

        private void AddTransform3DComponent()
        {
            _entityModel.AddTransform3DComponent();
        }

        private void AddEllipseRendererComponent()
        {
            _entityModel.AddEllipseRendererComponent();
        }

        private void AddRectangleRendererComponent()
        {
            _entityModel.AddRectangleRendererComponent();
        }

        private void AddTextRendererComponent()
        {
            _entityModel.AddTextRendererComponent();
        }

        private void AddCircleColliderComponent()
        {
            _entityModel.AddCircleColliderComponent();
        }

        private void AddRectangleColliderComponent()
        {
            _entityModel.AddRectangleColliderComponent();
        }

        private void EntityModelOnComponentAdded(object? sender, ComponentAddedEventArgs e)
        {
            Components.Add(_componentPropertiesEditorViewModelFactory.Create(e.ComponentModel));
        }
    }
}