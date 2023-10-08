using System;
using System.Windows.Forms;
using Geisha.Engine.Rendering.Backend;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Device = SharpDX.Direct3D11.Device;
using Factory4 = SharpDX.Direct2D1.Factory4;
using FeatureLevel = SharpDX.Direct3D.FeatureLevel;
using PixelFormat = SharpDX.Direct2D1.PixelFormat;
using Rational = SharpDX.DXGI.Rational;

namespace Geisha.Engine.Rendering.DirectX
{
    /// <summary>
    ///     Rendering backend implementation using DirectX rendering API. This implementation depends on WinForms.
    /// </summary>
    public sealed class DirectXRenderingBackend : IRenderingBackend, IDisposable
    {
        private readonly Statistics _statistics;
        private readonly Device _d3D11Device;
        private readonly SwapChain _dxgiSwapChain;
        private readonly SharpDX.Direct2D1.DeviceContext3 _d2D1DeviceContext;
        private readonly RenderingContext2D _renderingContext2D;

        /// <summary>
        ///     Creates new instance of <see cref="DirectXRenderingBackend" /> with specified <see cref="Form" /> as render target.
        /// </summary>
        /// <param name="form"><see cref="Form" /> that serves as render target.</param>
        /// <param name="driverType">Type of driver to use by rendering API.</param>
        public DirectXRenderingBackend(Form form, DriverType driverType)
        {
            _statistics = new Statistics();

            var swapChainDescription = new SwapChainDescription
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(form.ClientSize.Width, form.ClientSize.Height, new Rational(60, 1), Format.B8G8R8A8_UNorm),
                IsWindowed = true,
                OutputHandle = form.Handle,
                SampleDescription = new SampleDescription(4, 0),
                SwapEffect = SwapEffect.Discard, // TODO FlipDiscard is preferred for performance but it breaks current screen shot capture.
                Usage = Usage.RenderTargetOutput
            };

            var directXDriverType = driverType switch
            {
                DriverType.Hardware => SharpDX.Direct3D.DriverType.Hardware,
                DriverType.Software => SharpDX.Direct3D.DriverType.Warp,
                _ => throw new ArgumentOutOfRangeException(nameof(driverType), driverType, "Unknown driver type.")
            };

            Device.CreateWithSwapChain(
                directXDriverType,
                DeviceCreationFlags.BgraSupport, // TODO Investigate DeviceCreationFlags.Debug
                new[] { FeatureLevel.Level_11_0 },
                swapChainDescription,
                out _d3D11Device,
                out _dxgiSwapChain);

            using var dxgiFactory = _dxgiSwapChain.GetParent<SharpDX.DXGI.Factory>();
            dxgiFactory.MakeWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAll); // Ignore all windows events

            using var dxgiDevice = _d3D11Device.QueryInterface<SharpDX.DXGI.Device>();
            using var d2D1Factory = CreateD2D1Factory(FactoryType.SingleThreaded, DebugLevel.None);
            using var d2D1Device = new SharpDX.Direct2D1.Device3(d2D1Factory, dxgiDevice);
            _d2D1DeviceContext = new SharpDX.Direct2D1.DeviceContext3(d2D1Device, DeviceContextOptions.None);

            using var backBufferSurface = _dxgiSwapChain.GetBackBuffer<Surface>(0);
            var renderTargetBitmap = new Bitmap(_d2D1DeviceContext, backBufferSurface,
                new BitmapProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)));
            _d2D1DeviceContext.Target = renderTargetBitmap;
            _d2D1DeviceContext.AntialiasMode = AntialiasMode.Aliased;

            _renderingContext2D = new RenderingContext2D(form, _d2D1DeviceContext, _statistics);
        }

        /// <summary>
        ///     Creates new instance of <see cref="DirectXRenderingBackend" /> with default hidden <see cref="Form" /> as a render
        ///     target. This constructor is meant for providing rendering backend services without rendering output e.g. current
        ///     implementation of Geisha.Editor.
        /// </summary>
        public DirectXRenderingBackend() : this(new Form(), DriverType.Hardware)
        {
        }

        /// <inheritdoc />
        public IRenderingContext2D Context2D => _renderingContext2D;

        /// <inheritdoc />
        public RenderingStatistics Statistics => _statistics.LastFrameStats;

        /// <inheritdoc />
        public void Present(bool waitForVSync)
        {
            _dxgiSwapChain.Present(waitForVSync ? 1 : 0, PresentFlags.None);

            _statistics.UpdateLastFrameStats();
        }

        /// <summary>
        ///     Releases rendering API resources.
        /// </summary>
        public void Dispose()
        {
            _renderingContext2D.Dispose();
            _d2D1DeviceContext.Dispose();
            _dxgiSwapChain.Dispose();
            _d3D11Device.Dispose();
        }

        private static Factory4 CreateD2D1Factory(FactoryType factoryType, DebugLevel debugLevel)
        {
            FactoryOptions? factoryOptionsRef = null;
            if (debugLevel != DebugLevel.None)
            {
                factoryOptionsRef = new FactoryOptions
                {
                    DebugLevel = debugLevel
                };
            }

            D2D1.CreateFactory(factoryType, Utilities.GetGuidFromType(typeof(Factory4)), factoryOptionsRef, out var iFactoryOut);
            return new Factory4(iFactoryOut);
        }
    }
}