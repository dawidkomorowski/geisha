using Geisha.Editor.Core;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.Core
{
    [TestFixture]
    public class RelayCommandTests
    {
        [Test]
        public void RaiseCanExecuteChanged_ShouldRaiseCanExecuteChangedEvent()
        {
            // Arrange
            var wasCanExecuteChangedRaised = false;

            var relayCommand = RelayCommand.Create(() => { });
            relayCommand.CanExecuteChanged += (sender, args) => wasCanExecuteChangedRaised = true;

            // Act
            relayCommand.RaiseCanExecuteChanged();

            // Assert
            Assert.That(wasCanExecuteChangedRaised, Is.True);
        }

        [Test]
        public void Create_ExecuteActionWithoutParameter()
        {
            // Arrange
            var wasExecuted = false;
            void Execute() => wasExecuted = true;

            // Act
            var relayCommand = RelayCommand.Create(Execute);
            var canExecuteResult = relayCommand.CanExecute(null);
            relayCommand.Execute(null);

            // Assert
            Assert.That(canExecuteResult, Is.True);
            Assert.That(wasExecuted, Is.True);
        }

        [Test]
        public void Create_ExecuteActionWithoutParameter_CanExecuteFuncWithoutParameter()
        {
            // Arrange
            var wasExecuted = false;
            void Execute() => wasExecuted = true;

            var canExecuteToBeReturned = false;
            bool CanExecute() => canExecuteToBeReturned;

            // Act
            var relayCommand = RelayCommand.Create(Execute, CanExecute);

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
        public void Create_ExecuteActionWithReferenceTypeParameter()
        {
            // Arrange
            object? actualParameter = null;
            void Execute(object? parameter) => actualParameter = parameter;

            var expectedParameter = new object();

            // Act
            var relayCommand = RelayCommand.Create<object>(Execute);
            var canExecuteResult = relayCommand.CanExecute(expectedParameter);
            relayCommand.Execute(expectedParameter);

            // Assert
            Assert.That(canExecuteResult, Is.True);
            Assert.That(actualParameter, Is.EqualTo(expectedParameter));
        }

        [Test]
        public void Create_ExecuteActionWithReferenceTypeParameter_CanExecuteFuncWithReferenceTypeParameter()
        {
            // Arrange
            object? actualExecuteParameter = null;
            void Execute(object? parameter) => actualExecuteParameter = parameter;

            object? actualCanExecuteParameter = null;
            var canExecuteToBeReturned = false;

            bool CanExecute(object? parameter)
            {
                actualCanExecuteParameter = parameter;
                return canExecuteToBeReturned;
            }

            var expectedParameter = new object();

            // Act
            var relayCommand = RelayCommand.Create<object>(Execute, CanExecute);

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
        public void Create_ExecuteActionWithValueTypeParameter()
        {
            // Arrange
            int? actualParameter = null;
            void Execute(int? parameter) => actualParameter = parameter;

            const int expectedParameter = 1;

            // Act
            var relayCommand = RelayCommand.Create<int>(Execute);
            var canExecuteResult = relayCommand.CanExecute(expectedParameter);
            relayCommand.Execute(expectedParameter);

            // Assert
            Assert.That(canExecuteResult, Is.True);
            Assert.That(actualParameter, Is.EqualTo(expectedParameter));
        }

        [Test]
        public void Create_ExecuteActionWithValueTypeParameter_CanExecuteFuncWithValueTypeParameter()
        {
            // Arrange
            int? actualExecuteParameter = null;
            void Execute(int? parameter) => actualExecuteParameter = parameter;

            int? actualCanExecuteParameter = null;
            var canExecuteToBeReturned = false;

            bool CanExecute(int? parameter)
            {
                actualCanExecuteParameter = parameter;
                return canExecuteToBeReturned;
            }

            const int expectedParameter = 1;

            // Act
            var relayCommand = RelayCommand.Create<int>(Execute, CanExecute);

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