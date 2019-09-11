using Geisha.Editor.Core.Docking;
using Geisha.Editor.Core.ViewModels;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.Core.Docking
{
    [TestFixture]
    public class ToolViewModelTests
    {
        [Test]
        public void Constructor_ShouldSetTitle_IsVisible_View_AndShouldSetViewModelAsDataContextOfView()
        {
            // Arrange
            const string title = "SomeTitle";
            var view = Substitute.For<IView>();
            var viewModel = new TestViewModel();

            // Act
            var toolViewModel = new TestToolViewModel(title, view, viewModel, true);

            // Assert
            Assert.That(toolViewModel.Title, Is.EqualTo(title));
            Assert.That(toolViewModel.IsVisible, Is.True);
            Assert.That(toolViewModel.View, Is.EqualTo(view));
            Assert.That(toolViewModel.View.DataContext, Is.EqualTo(viewModel));
        }

        [Test]
        public void ShowCommand_ShouldSetIsVisibleToTrue()
        {
            // Arrange
            const string title = "SomeTitle";
            var view = Substitute.For<IView>();
            var viewModel = new TestViewModel();
            var toolViewModel = new TestToolViewModel(title, view, viewModel, false);

            // Assume
            Assume.That(toolViewModel.IsVisible, Is.False);

            // Act
            toolViewModel.ShowCommand.Execute(null);

            // Assert
            Assert.That(toolViewModel.IsVisible, Is.True);
        }

        [Test]
        public void CloseCommand_ShouldSetIsVisibleToFalse()
        {
            // Arrange
            const string title = "SomeTitle";
            var view = Substitute.For<IView>();
            var viewModel = new TestViewModel();
            var toolViewModel = new TestToolViewModel(title, view, viewModel, true);

            // Assume
            Assume.That(toolViewModel.IsVisible, Is.True);

            // Act
            toolViewModel.CloseCommand.Execute(null);

            // Assert
            Assert.That(toolViewModel.IsVisible, Is.False);
        }

        private sealed class TestToolViewModel : ToolViewModel
        {
            public TestToolViewModel(string title, IView view, ViewModel viewModel, bool isVisible) : base(title, view, viewModel, isVisible)
            {
            }
        }

        private sealed class TestViewModel : ViewModel
        {
        }
    }
}