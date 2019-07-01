﻿using System.Windows.Forms;

namespace Geisha.Engine.Input.Windows
{
    public sealed class WindowsInputBackend : IInputBackend
    {
        private readonly Form _form;

        public WindowsInputBackend(Form form)
        {
            _form = form;
        }

        public IInputProvider CreateInputProvider()
        {
            return new InputProvider(_form);
        }
    }
}