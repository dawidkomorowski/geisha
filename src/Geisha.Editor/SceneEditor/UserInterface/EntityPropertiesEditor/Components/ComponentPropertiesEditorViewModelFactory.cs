using System;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.CircleColliderComponent;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.EllipseRendererComponent;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.RectangleColliderComponent;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.RectangleRendererComponent;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.TextRendererComponent;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.Transform3DComponent;

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
                case Transform3DComponentModel transformComponentModel:
                    return new Transform3DComponentPropertiesEditorViewModel(transformComponentModel);
                case EllipseRendererComponentModel ellipseRendererComponentModel:
                    return new EllipseRendererComponentPropertiesEditorViewModel(ellipseRendererComponentModel);
                case RectangleRendererComponentModel rectangleRendererComponentModel:
                    return new RectangleRendererComponentPropertiesEditorViewModel(rectangleRendererComponentModel);
                case TextRendererComponentModel textRendererComponentModel:
                    return new TextRendererComponentPropertiesEditorViewModel(textRendererComponentModel);
                case CircleColliderComponentModel circleColliderComponentModel:
                    return new CircleColliderComponentPropertiesEditorViewModel(circleColliderComponentModel);
                case RectangleColliderComponentModel rectangleColliderComponentModel:
                    return new RectangleColliderComponentPropertiesEditorViewModel(rectangleColliderComponentModel);
                default:
                    throw new ArgumentOutOfRangeException(nameof(componentModel), $"Component model of type {componentModel.GetType()} is not supported.");
            }
        }
    }
}