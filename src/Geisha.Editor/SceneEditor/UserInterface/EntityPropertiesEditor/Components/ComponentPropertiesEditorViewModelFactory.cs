using System;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.TransformComponent;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components
{
    internal interface IComponentPropertiesEditorViewModelFactory
    {
        ComponentPropertiesEditorViewModel Create(IComponentModel componentModel);
    }

    internal sealed class ComponentPropertiesEditorViewModelFactory : IComponentPropertiesEditorViewModelFactory
    {
        public ComponentPropertiesEditorViewModel Create(IComponentModel componentModel)
        {
            switch (componentModel)
            {
                case TransformComponentModel transformComponentModel:
                    return new TransformComponentPropertiesEditorViewModel(transformComponentModel);
                default:
                    throw new ArgumentOutOfRangeException(nameof(componentModel), $"Component model of type {componentModel.GetType()} is not supported.");
            }
        }
    }
}