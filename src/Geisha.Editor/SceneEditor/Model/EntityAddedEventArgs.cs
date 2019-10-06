using System;

namespace Geisha.Editor.SceneEditor.Model
{
    public sealed class EntityAddedEventArgs : EventArgs
    {
        public EntityAddedEventArgs(EntityModel entityModel)
        {
            EntityModel = entityModel;
        }

        public EntityModel EntityModel { get; }
    }
}