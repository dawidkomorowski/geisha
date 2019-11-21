namespace Geisha.Editor.Core.Properties
{
    internal sealed class PropertiesSubjectChangedEvent : IEvent
    {
        public PropertiesSubjectChangedEvent(ViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public ViewModel ViewModel { get; }
    }
}