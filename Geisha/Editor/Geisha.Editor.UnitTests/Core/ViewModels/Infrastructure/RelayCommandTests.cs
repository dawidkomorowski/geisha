using System;
using Geisha.Editor.Core.ViewModels.Infrastructure;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.Core.ViewModels.Infrastructure
{
    [TestFixture]
    public class RelayCommandTests
    {
        [Test]
        public void RaiseCanExecuteChanged_ShouldRaiseCanExecuteChangedEvent()
        {
            // Arrange
            var wasCanExecuteChangedRaised = false;

            var relayCommand = new RelayCommand(() => { });
            relayCommand.CanExecuteChanged += (sender, args) => wasCanExecuteChangedRaised = true;

            // Act
            relayCommand.RaiseCanExecuteChanged();

            // Assert
            Assert.That(wasCanExecuteChangedRaised, Is.True);
        }

        [Test]
        public void Constructor_ExecuteActionWithoutParameter()
        {
            // Arrange
            var wasExecuted = false;
            void Execute() => wasExecuted = true;

            // Act
            var relayCommand = new RelayCommand(Execute);
            var canExecuteResult = relayCommand.CanExecute(null);
            relayCommand.Execute(null);

            // Assert
            Assert.That(canExecuteResult, Is.True);
            Assert.That(wasExecuted, Is.True);
        }

        [Test]
        public void Constructor_ExecuteActionWithoutParameter_CanExecuteFuncWithoutParameter()
        {
            // Arrange
            var wasExecuted = false;
            void Execute() => wasExecuted = true;

            var canExecuteToBeReturned = false;
            bool CanExecute() => canExecuteToBeReturned;

            // Act
            var relayCommand = new RelayCommand(Execute, CanExecute);

            canExecuteToBeReturned = true;
            var canExecuteResultTrue = relayCommand.CanExecute(null);

            canExecuteToBeReturned = false;
            var canExecuteResultFalse = relayCommand.CanExecute(null);

            relayCommand.Execute(null);

            // Assert
            Assert.That(canExecuteResultTrue, Is.True);
            Assert.That(canExecuteResultFalse, Is.False);
            Assert.That(wasExecuted, Is.True);
        }

        [Test]
        public void Constructor_ExecuteActionWithParameter()
        {
            // Arrange
            object actualParameter = null;
            void Execute(object parameter) => actualParameter = parameter;

            var expectedParameter = new object();

            // Act
            var relayCommand = new RelayCommand(Execute);
            var canExecuteResult = relayCommand.CanExecute(expectedParameter);
            relayCommand.Execute(expectedParameter);

            // Assert
            Assert.That(canExecuteResult, Is.True);
            Assert.That(actualParameter, Is.EqualTo(expectedParameter));
        }

        [Test]
        public void Constructor_ExecuteActionWithParameter_CanExecuteFuncWithParameter()
        {
            // Arrange
            object actualExecuteParameter = null;
            void Execute(object parameter) => actualExecuteParameter = parameter;

            object actualCanExecuteParameter = null;
            var canExecuteToBeReturned = false;

            bool CanExecute(object parameter)
            {
                actualCanExecuteParameter = parameter;
                return canExecuteToBeReturned;
            }

            var expectedParameter = new object();

            // Act
            var relayCommand = new RelayCommand(Execute, CanExecute);

            canExecuteToBeReturned = true;
            var canExecuteResultTrue = relayCommand.CanExecute(expectedParameter);

            canExecuteToBeReturned = false;
            var canExecuteResultFalse = relayCommand.CanExecute(expectedParameter);

            relayCommand.Execute(expectedParameter);

            // Assert
            Assert.That(canExecuteResultTrue, Is.True);
            Assert.That(canExecuteResultFalse, Is.False);
            Assert.That(actualCanExecuteParameter, Is.EqualTo(expectedParameter));
            Assert.That(actualExecuteParameter, Is.EqualTo(expectedParameter));
        }

        [Test]
        public void Constructor_ExecuteActionWithTypedParameter()
        {
            // Arrange
            var actualParameter = 0;
            void Execute(int parameter) => actualParameter = parameter;

            const int expectedParameter = 1;

            // Act
            var relayCommand = new RelayCommand<int>(Execute);
            var canExecuteResult = relayCommand.CanExecute(expectedParameter);
            relayCommand.Execute(expectedParameter);

            // Assert
            Assert.That(canExecuteResult, Is.True);
            Assert.That(actualParameter, Is.EqualTo(expectedParameter));
        }

        [Test]
        public void Constructor_ExecuteActionWithTypedParameter_CanExecuteFuncWithTypedParameter()
        {
            // Arrange
            var actualExecuteParameter = 0;
            void Execute(int parameter) => actualExecuteParameter = parameter;

            var actualCanExecuteParameter = 0;
            var canExecuteToBeReturned = false;

            bool CanExecute(int parameter)
            {
                actualCanExecuteParameter = parameter;
                return canExecuteToBeReturned;
            }

            const int expectedParameter = 1;

            // Act
            var relayCommand = new RelayCommand<int>(Execute, CanExecute);

            canExecuteToBeReturned = true;
            var canExecuteResultTrue = relayCommand.CanExecute(expectedParameter);

            canExecuteToBeReturned = false;
            var canExecuteResultFalse = relayCommand.CanExecute(expectedParameter);

            relayCommand.Execute(expectedParameter);

            // Assert
            Assert.That(canExecuteResultTrue, Is.True);
            Assert.That(canExecuteResultFalse, Is.False);
            Assert.That(actualCanExecuteParameter, Is.EqualTo(expectedParameter));
            Assert.That(actualExecuteParameter, Is.EqualTo(expectedParameter));
        }
    }
}