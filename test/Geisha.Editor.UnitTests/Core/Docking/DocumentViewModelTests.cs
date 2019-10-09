using Geisha.Editor.Core.Docking;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.Core.Docking
{
    [TestFixture]
    public class DocumentViewModelTests
    {
        private const string Title = "Some document";
        private IView _view;
        private DocumentContentViewModel _documentContentViewModel;

        [SetUp]
        public void SetUp()
        {
            _view = Substitute.For<IView>();
            _documentContentViewModel = Substitute.For<DocumentContentViewModel>();
        }

        [Test]
        public void Constructor_ShouldSetTitleAndViewAsSpecifiedByParameters_AndShouldSetIsSelectedToTrue()
        {
            // Arrange
            // Act
            var documentViewModel = new DocumentViewModel(Title, _view, _documentContentViewModel);

            // Assert
            Assert.That(documentViewModel.Title, Is.EqualTo(Title));
            Assert.That(documentViewModel.View, Is.EqualTo(_view));
            Assert.That(documentViewModel.IsSelected, Is.True);
        }

        [Test]
        public void Constructor_ShouldSetIsSelectedToTrueAndInvokeOnDocumentSelected()
        {
            // Arrange
            // Act
            var documentViewModel = new DocumentViewModel(Title, _view, _documentContentViewModel);

            // Assert
            Assert.That(documentViewModel.IsSelected, Is.True);
            _documentContentViewModel.Received(1).OnDocumentSelected();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsSelected_ShouldInvokeOnDocumentSelected_WhenSetToTrue(bool isSelected)
        {
            // Arrange
            var documentViewModel = new DocumentViewModel(Title, _view, _documentContentViewModel) {IsSelected = !isSelected};

            _documentContentViewModel.ClearReceivedCalls();

            // Act
            documentViewModel.IsSelected = isSelected;

            // Assert
            if (isSelected) _documentContentViewModel.Received(1).OnDocumentSelected();
            else _documentContentViewModel.DidNotReceive().OnDocumentSelected();
        }

        [Test]
        public void SaveDocument_ShouldInvokeSaveDocumentOfDocumentContentViewModel()
        {
            // Arrange
            var documentViewModel = new DocumentViewModel(Title, _view, _documentContentViewModel);

            // Act
            documentViewModel.SaveDocument();

            // Assert
            _documentContentViewModel.Received(1).SaveDocument();
        }
    }
}