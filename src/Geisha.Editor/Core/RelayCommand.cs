using System;
using System.Windows.Input;

namespace Geisha.Editor.Core
{
    public interface IRelayCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }

    public static class RelayCommand
    {
        public static IRelayCommand Create(Action execute) => new RelayCommandWithoutParameter(execute);
        public static IRelayCommand Create(Action execute, Func<bool> canExecute) => new RelayCommandWithoutParameter(execute, canExecute);
        public static IRelayCommand Create<T>(Action<T?> execute) where T : class => new RelayCommandWithReferenceTypeParameter<T>(execute);

        public static IRelayCommand Create<T>(Action<T?> execute, Func<T?, bool> canExecute) where T : class =>
            new RelayCommandWithReferenceTypeParameter<T>(execute, canExecute);

        public static IRelayCommand Create<T>(Action<T?> execute) where T : struct => new RelayCommandWithValueTypeParameter<T>(execute);

        public static IRelayCommand Create<T>(Action<T?> execute, Func<T?, bool> canExecute) where T : struct =>
            new RelayCommandWithValueTypeParameter<T>(execute, canExecute);


        private abstract class RelayCommandBase : IRelayCommand
        {
            public event EventHandler? CanExecuteChanged;

            public void RaiseCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }

            public abstract void Execute(object? parameter);
            public abstract bool CanExecute(object? parameter);
        }

        private sealed class RelayCommandWithoutParameter : RelayCommandBase
        {
            private readonly Action _execute;
            private readonly Func<bool> _canExecute;

            public RelayCommandWithoutParameter(Action execute) : this(execute, () => true)
            {
            }

            public RelayCommandWithoutParameter(Action execute, Func<bool> canExecute)
            {
                _execute = execute;
                _canExecute = canExecute;
            }

            public override void Execute(object? parameter) => _execute();
            public override bool CanExecute(object? parameter) => _canExecute();
        }

        private sealed class RelayCommandWithReferenceTypeParameter<T> : RelayCommandBase where T : class
        {
            private readonly Action<T?> _execute;
            private readonly Func<T?, bool> _canExecute;

            public RelayCommandWithReferenceTypeParameter(Action<T?> execute) : this(execute, parameter => true)
            {
            }

            public RelayCommandWithReferenceTypeParameter(Action<T?> execute, Func<T?, bool> canExecute)
            {
                _execute = execute;
                _canExecute = canExecute;
            }

            public override void Execute(object? parameter) => _execute((T?) parameter);

            public override bool CanExecute(object? parameter) => _canExecute((T?) parameter);
        }

        private sealed class RelayCommandWithValueTypeParameter<T> : RelayCommandBase where T : struct
        {
            private readonly Action<T?> _execute;
            private readonly Func<T?, bool> _canExecute;

            public RelayCommandWithValueTypeParameter(Action<T?> execute) : this(execute, parameter => true)
            {
            }

            public RelayCommandWithValueTypeParameter(Action<T?> execute, Func<T?, bool> canExecute)
            {
                _execute = execute;
                _canExecute = canExecute;
            }

            public override void Execute(object? parameter) => _execute((T?) parameter);

            public override bool CanExecute(object? parameter) => _canExecute((T?) parameter);
        }
    }
}