using Geisha.Editor.Core.ViewModels.Infrastructure;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.Core.ViewModels.Infrastructure
{
    [TestFixture]
    public class DockableViewViewModelTests
    {
        [Test]
        public void Constructor_ShouldSetTitleAndViewModel()
        {
            // Arrange
            const string title = "SomeTitle";
            var viewModel = new TestViewModel();

            // Act
            var dockableViewViewModel = new DockableViewViewModel(title, viewModel);

            // Assert
            Assert.That(dockableViewViewModel.Title, Is.EqualTo(title));
            Assert.That(dockableViewViewModel.ViewModel, Is.EqualTo(viewModel));
        }

        [Test]
        public void ShowCommand_ShouldSetIsVisibleToTrue()
        {
            // Arrange
            const string title = "SomeTitle";
            var viewModel = new TestViewModel();
            var dockableViewViewModel = new DockableViewViewModel(title, viewModel);

            // Assume
            Assume.That(dockableViewViewModel.IsVisible, Is.False);

            // Act
            dockableViewViewModel.ShowCommand.Execute(null);

            // Assert
            Assert.That(dockableViewViewModel.IsVisible, Is.True);
        }

        [Test]
        public void CloseCommand_ShouldSetIsVisibleToFalse()
        {
            // Arrange
            const string title = "SomeTitle";
            var viewModel = new TestViewModel();
            var dockableViewViewModel = new DockableViewViewModel(title, viewModel) {IsVisible = true};

            // Assume
            Assume.That(dockableViewViewModel.IsVisible, Is.True);

            // Act
            dockableViewViewModel.CloseCommand.Execute(null);

            // Assert
            Assert.That(dockableViewViewModel.IsVisible, Is.False);
        }

        private class TestViewModel : ViewModel
        {
        }
    }
}