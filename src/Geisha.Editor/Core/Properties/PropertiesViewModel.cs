using System.Windows.Controls;

namespace Geisha.Editor.Core.Properties
{
    internal sealed class PropertiesViewModel : ViewModel
    {
        private readonly IProperty<Control> _propertiesEditor;

        public PropertiesViewModel(IEventBus eventBus)
        {
            _propertiesEditor = CreateProperty<Control>(nameof(PropertiesEditor));

            eventBus.RegisterEventHandler<PropertiesSubjectChangedEvent>(PropertiesSubjectChangedEventHandler);
        }

        public Control PropertiesEditor
        {
            get => _propertiesEditor.Get();
            set => _propertiesEditor.Set(value);
        }

        private void PropertiesSubjectChangedEventHandler(PropertiesSubjectChangedEvent e)
        {
            PropertiesEditor = e.PropertiesEditor;
        }
    }
}