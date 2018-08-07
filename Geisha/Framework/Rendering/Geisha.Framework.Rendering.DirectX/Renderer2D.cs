using System;
using System.ComponentModel.Composition;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Geisha.Common.Math;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Bitmap = System.Drawing.Bitmap;
using Device = SharpDX.Direct3D11.Device;
using Factory = SharpDX.Direct2D1.Factory;
using FeatureLevel = SharpDX.Direct3D.FeatureLevel;
using PixelFormat = SharpDX.Direct2D1.PixelFormat;
using Rectangle = System.Drawing.Rectangle;
using Resource = SharpDX.Direct3D11.Resource;

namespace Geisha.Framework.Rendering.DirectX
{
    [Export(typeof(IRenderer2D))]
    public sealed class Renderer2D : IRenderer2D
    {
        private readonly Surface _backBufferSurface;
        private readonly Texture2D _backBufferTexture;
        private readonly Factory _d2D1Factory;
        private readonly RenderTarget _d2D1RenderTarget;
        private readonly Device _d3D11Device;
        private readonly SharpDX.DXGI.Factory _dxgiFactory;
        private readonly SwapChain _dxgiSwapChain;

        [ImportingConstructor]
        public Renderer2D(IWindowProvider windowProvider)
        {
            var window = windowProvider.Window;

            var swapChainDescription = new SwapChainDescription
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(window.ClientAreaWidth, window.ClientAreaHeight, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = window.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };


            Device.CreateWithSwapChain(
                DriverType.Hardware,
                DeviceCreationFlags.BgraSupport,
                new[] {FeatureLevel.Level_10_0},
                swapChainDescription,
                out _d3D11Device,
                out _dxgiSwapChain);

            _dxgiFactory = _dxgiSwapChain.GetParent<SharpDX.DXGI.Factory>();
            _dxgiFactory.MakeWindowAssociation(window.Handle, WindowAssociationFlags.IgnoreAll); // Ignore all windows events

            _backBufferTexture = Resource.FromSwapChain<Texture2D>(_dxgiSwapChain, 0);

            _backBufferSurface = _backBufferTexture.QueryInterface<Surface>();

            _d2D1Factory = new Factory();

            _d2D1RenderTarget = new RenderTarget(_d2D1Factory, _backBufferSurface,
                new RenderTargetProperties(new PixelFormat(Format.Unknown, AlphaMode.Premultiplied)));
        }

        public IRenderingContext RenderingContext { get; }

        public ITexture CreateTexture(Stream stream)
        {
            // Create GDI bitmap from stream
            using (var gdiBitmap = new Bitmap(stream))
            {
                SharpDX.Direct2D1.Bitmap d2D1Bitmap;

                // Get access to raw GDI bitmap data
                var gdiBitmapData = gdiBitmap.LockBits(new Rectangle(0, 0, gdiBitmap.Width, gdiBitmap.Height), ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                // Fill data stream with GDI bitmap data to create from it a Direct2D1 bitmap
                var stride = Math.Abs(gdiBitmapData.Stride);
                using (var convertedBitmapDataStream = new DataStream(gdiBitmap.Height * stride, true, true))
                {
                    // Convert pixel format from ARGB to BGRA
                    for (var i = 0; i < gdiBitmap.Height * stride; i += sizeof(int))
                    {
                        var pixelValue = Marshal.ReadInt32(gdiBitmapData.Scan0, i);
                        var pixelColor = Color.FromArgb(pixelValue);
                        convertedBitmapDataStream.WriteByte(pixelColor.B);
                        convertedBitmapDataStream.WriteByte(pixelColor.G);
                        convertedBitmapDataStream.WriteByte(pixelColor.R);
                        convertedBitmapDataStream.WriteByte(pixelColor.A);
                    }

                    // Set data stream at the beginning
                    convertedBitmapDataStream.Position = 0;

                    // Create Direct2D1 bitmap from data stream
                    d2D1Bitmap = new SharpDX.Direct2D1.Bitmap(_d2D1RenderTarget, new Size2(gdiBitmap.Width, gdiBitmap.Height), convertedBitmapDataStream,
                        stride,
                        new BitmapProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)));
                }

                // Complete access to raw GDI bitmap data
                gdiBitmap.UnlockBits(gdiBitmapData);

                // Create texture from Direct2D1 bitmap
                return new Texture(d2D1Bitmap);
            }
        }

        public void BeginDraw()
        {
            _d2D1RenderTarget.BeginDraw();
        }

        public void EndDraw()
        {
            _d2D1RenderTarget.EndDraw();
            _dxgiSwapChain.Present(0, PresentFlags.None);
        }

        public void Clear(Color color)
        {
            _d2D1RenderTarget.Clear(new RawColor4((float) color.ScR, (float) color.ScG, (float) color.ScB, (float) color.ScA));
        }

        public void RenderText(string text, int fontSize, Color color, Matrix3 transform)
        {
            throw new NotImplementedException();
        }

        public void RenderSprite(Sprite sprite, Matrix3 transform)
        {
            throw new NotImplementedException();
        }

        #region Dispose

        private void ReleaseUnmanagedResources()
        {
            // TODO release unmanaged resources here
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                _d2D1RenderTarget.Dispose();
                _d2D1Factory.Dispose();
                _backBufferSurface.Dispose();
                _backBufferTexture.Dispose();
                _dxgiFactory.Dispose();
                _dxgiSwapChain.Dispose();
                _d3D11Device.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Renderer2D()
        {
            Dispose(false);
        }

        #endregion
    }
}