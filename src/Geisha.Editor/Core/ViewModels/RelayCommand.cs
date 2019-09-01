using System;
using System.Windows.Input;

namespace Geisha.Editor.Core.ViewModels
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action execute) : this(o => execute())
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute) : this(o => execute(), o => canExecute())
        {
        }

        public RelayCommand(Action<object> execute) : this(execute, o => true)
        {
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            _canExecute = canExecute;
            _execute = execute;
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class RelayCommand<T> : RelayCommand
    {
        public RelayCommand(Action<T> execute) : base(o => execute((T) o))
        {
        }

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute) : base(o => execute((T) o), o => canExecute((T) o))
        {
        }
    }
}