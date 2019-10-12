using System.Windows.Controls;

namespace Geisha.Editor.Core.Properties
{
    internal sealed class PropertiesSubjectChangedEvent : IEvent
    {
        public PropertiesSubjectChangedEvent(Control propertiesEditor)
        {
            PropertiesEditor = propertiesEditor;
        }

        public Control PropertiesEditor { get; }
    }
}