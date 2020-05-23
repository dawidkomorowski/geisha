using System.Threading;
using System.Windows.Controls;
using Geisha.Editor.Core;
using Geisha.Editor.Core.Properties;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.Core.Properties
{
    [TestFixture]
    public class PropertiesViewModelTests
    {
        private IEventBus _eventBus = null!;
        private IViewRepository _viewRepository = null!;
        private PropertiesViewModel _propertiesViewModel = null!;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new EventBus();
            _viewRepository = new ViewRepository();
            _propertiesViewModel = new PropertiesViewModel(_eventBus, _viewRepository);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void PropertiesSubjectChangedEvent_ShouldUpdatePropertiesEditorProperty()
        {
            // Arrange
            _viewRepository.RegisterView(typeof(ContentControl), typeof(TestViewModel));
            var viewModel = new TestViewModel();

            // Assume
            Assume.That(_propertiesViewModel.PropertiesEditor, Is.Null);

            // Act
            _eventBus.SendEvent(new PropertiesSubjectChangedEvent(viewModel));

            // Assert
            Assert.That(_propertiesViewModel.PropertiesEditor, Is.TypeOf<ContentControl>());
            Assert.That(_propertiesViewModel.PropertiesEditor.DataContext, Is.EqualTo(viewModel));
        }

        private sealed class TestViewModel : ViewModel
        {
        }
    }
}