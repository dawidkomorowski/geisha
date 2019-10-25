using System.Collections.ObjectModel;
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

            AddTransformComponentCommand = new RelayCommand(AddTransformComponent);

            _name.Subscribe(name => _entityModel.Name = name);

            _entityModel.ComponentAdded += EntityModelOnComponentAdded;
        }

        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }

        public ObservableCollection<ComponentPropertiesEditorViewModel> Components { get; }

        public ICommand AddTransformComponentCommand { get; }

        private void AddTransformComponent()
        {
            _entityModel.AddTransformComponent();
        }

        private void EntityModelOnComponentAdded(object sender, ComponentAddedEventArgs e)
        {
            Components.Add(_componentPropertiesEditorViewModelFactory.Create(e.ComponentModel));
        }
    }
}