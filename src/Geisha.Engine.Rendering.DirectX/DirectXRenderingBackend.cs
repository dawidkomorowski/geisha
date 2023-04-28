using System;
using System.Windows.Forms;
using Geisha.Engine.Rendering.Backend;

namespace Geisha.Engine.Rendering.DirectX
{
    /// <summary>
    ///     Rendering backend implementation using DirectX rendering API. This implementation depends on WinForms.
    /// </summary>
    public sealed class DirectXRenderingBackend : IRenderingBackend, IDisposable
    {
        private readonly RenderingContext2D _renderingContext2D;

        /// <summary>
        ///     Creates new instance of <see cref="DirectXRenderingBackend" /> with specified <see cref="Form" /> as render target.
        /// </summary>
        /// <param name="form"><see cref="Form" /> that serves as render target.</param>
        /// <param name="driverType">Type of driver to use by rendering API.</param>
        public DirectXRenderingBackend(Form form, DriverType driverType)
        {
            _renderingContext2D = new RenderingContext2D(form, driverType);
        }

        /// <summary>
        ///     Creates new instance of <see cref="DirectXRenderingBackend" /> with default hidden <see cref="Form" /> as a render
        ///     target. This constructor is meant for providing rendering backend services without rendering output e.g. current
        ///     implementation of Geisha.Editor.
        /// </summary>
        public DirectXRenderingBackend() : this(new Form(), DriverType.Hardware)
        {
        }

        /// <summary>
        ///     2D rendering context provided by the rendering backend.
        /// </summary>
        public IRenderingContext2D Context2D => _renderingContext2D;

        /// <summary>
        ///     Releases rendering API resources.
        /// </summary>
        public void Dispose()
        {
            _renderingContext2D.Dispose();
        }
    }
}