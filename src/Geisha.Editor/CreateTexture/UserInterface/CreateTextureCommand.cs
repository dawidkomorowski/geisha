using System;
using System.Windows.Input;

namespace Geisha.Editor.CreateTexture.UserInterface
{
    internal sealed class CreateTextureCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }

        public event EventHandler CanExecuteChanged;
    }
}