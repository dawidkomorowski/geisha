using System;
using System.Windows.Forms;

namespace Geisha.Engine.Rendering.DirectX
{
    /// <summary>
    ///     Rendering backend implementation using DirectX rendering API. This implementation depends on WinForms.
    /// </summary>
    public sealed class DirectXRenderingBackend : IRenderingBackend, IDisposable
    {
        private readonly Renderer2D _renderer2D;

        /// <summary>
        ///     Creates new instance of <see cref="DirectXRenderingBackend" /> with specified <see cref="Form" /> as render target.
        /// </summary>
        /// <param name="form"><see cref="Form" /> that serves as render target.</param>
        public DirectXRenderingBackend(Form form)
        {
            _renderer2D = new Renderer2D(form);
        }

        public void Dispose()
        {
            _renderer2D?.Dispose();
        }

        /// <summary>
        ///     2D rendering service provided by the rendering backend.
        /// </summary>
        public IRenderer2D Renderer2D => _renderer2D;
    }
}