using System;
using System.Threading;
using System.Windows.Controls;
using Geisha.Editor.Core;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.Core
{
    [TestFixture]
    public class ViewRepositoryTests
    {
        [Test]
        [Apartment(ApartmentState.STA)]
        public void CreateView_ShouldCreateViewRegisteredForGivenViewModel()
        {
            // Arrange
            var viewRepository = new ViewRepository();
            viewRepository.RegisterView(typeof(TestView), typeof(TestViewModel));

            var viewModel = new TestViewModel();

            // Act
            var view = viewRepository.CreateView(viewModel);

            // Assert
            Assert.That(view, Is.Not.Null);
            Assert.That(view, Is.TypeOf<TestView>());
        }

        [Test]
        public void CreateView_ShouldThrowException_GivenViewModelForNotRegisteredView()
        {
            // Arrange
            var viewRepository = new ViewRepository();
            var viewModel = new TestViewModel();

            // Act
            // Assert
            Assert.That(() => viewRepository.CreateView(viewModel), Throws.ArgumentException);
        }

        [TestCase(typeof(int), typeof(TestViewModel))]
        [TestCase(typeof(TestView), typeof(int))]
        [TestCase(typeof(int), typeof(int))]
        public void RegisterView_ShouldThrowException_GivenIncorrectTypes(Type viewType, Type viewModelType)
        {
            // Arrange
            var viewRepository = new ViewRepository();

            // Act
            // Assert
            Assert.That(() => viewRepository.RegisterView(viewType, viewModelType), Throws.ArgumentException);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void RegisterViewsFromCurrentlyLoadedAssemblies_ShouldRegisterViewsWithAttribute()
        {
            // Arrange
            var viewRepository = new ViewRepository();

            // Act
            viewRepository.RegisterViewsFromCurrentlyLoadedAssemblies();

            // Assert
            var viewModel = new TestViewModel();
            var view = viewRepository.CreateView(viewModel);
            Assert.That(view, Is.Not.Null);
            Assert.That(view, Is.TypeOf<AttributedTestView>());
        }

        private sealed class TestView : Control
        {
        }

        [RegisterViewFor(typeof(TestViewModel))]
        private sealed class AttributedTestView : Control
        {
        }

        private sealed class TestViewModel : ViewModel
        {
        }
    }
}