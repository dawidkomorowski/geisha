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
        private IEventBus _eventBus;
        private PropertiesViewModel _propertiesViewModel;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new EventBus();
            _propertiesViewModel = new PropertiesViewModel(_eventBus);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void PropertiesSubjectChangedEvent_ShouldUpdatePropertiesEditorProperty()
        {
            // Arrange
            var propertiesEditor = new ContentControl();

            // Assume
            Assume.That(_propertiesViewModel.PropertiesEditor, Is.Null);

            // Act
            _eventBus.SendEvent(new PropertiesSubjectChangedEvent(propertiesEditor));

            // Assert
            Assert.That(_propertiesViewModel.PropertiesEditor, Is.EqualTo(propertiesEditor));
        }
    }
}