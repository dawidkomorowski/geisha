using System;
using Geisha.Engine.Rendering;
using SharpDX.Windows;

namespace Geisha.Engine.Host.DirectX
{
    internal sealed class Window : IWindow
    {
        private readonly RenderForm _renderForm;

        public Window(RenderForm renderForm)
        {
            _renderForm = renderForm;
        }

        public int ClientAreaWidth => _renderForm.ClientSize.Width;
        public int ClientAreaHeight => _renderForm.ClientSize.Height;
        public IntPtr Handle => _renderForm.Handle;
    }
}