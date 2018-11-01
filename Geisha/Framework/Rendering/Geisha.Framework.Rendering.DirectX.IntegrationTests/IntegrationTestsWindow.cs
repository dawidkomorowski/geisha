using System;
using System.Drawing;
using SharpDX.Windows;

namespace Geisha.Framework.Rendering.DirectX.IntegrationTests
{
    public class IntegrationTestsWindow : IWindow
    {
        private readonly RenderForm _renderForm;

        public IntegrationTestsWindow()
        {
            _renderForm = new RenderForm($"{typeof(IntegrationTestsWindow).FullName}") {ClientSize = new Size(1280, 720)};
        }

        public int ClientAreaWidth => _renderForm.ClientSize.Width;
        public int ClientAreaHeight => _renderForm.ClientSize.Height;
        public IntPtr Handle => _renderForm.Handle;
    }
}