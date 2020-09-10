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

        /// <summary>
        ///     Creates new instance of <see cref="DirectXRenderingBackend" /> with default hidden <see cref="Form" /> as a render
        ///     target. This constructor is meant for providing rendering backend services without rendering output e.g. current
        ///     implementation of Geisha.Editor.
        /// </summary>
        public DirectXRenderingBackend() : this(new Form())
        {
        }

        /// <summary>
        ///     2D rendering service provided by the rendering backend.
        /// </summary>
        public IRenderer2D Renderer2D => _renderer2D;

        public void Dispose()
        {
            _renderer2D.Dispose();
        }
    }
}