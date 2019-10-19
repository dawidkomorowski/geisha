using System;

namespace Geisha.Editor.SceneEditor.Model
{
    public sealed class ComponentAddedEventArgs : EventArgs
    {
        public ComponentAddedEventArgs(IComponentModel componentModel)
        {
            ComponentModel = componentModel;
        }

        public IComponentModel ComponentModel { get; }
    }
}