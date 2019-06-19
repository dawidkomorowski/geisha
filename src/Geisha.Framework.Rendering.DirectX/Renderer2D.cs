using System;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Geisha.Common.Math;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Bitmap = System.Drawing.Bitmap;
using Device = SharpDX.Direct3D11.Device;
using Factory = SharpDX.Direct2D1.Factory;
using FactoryType = SharpDX.DirectWrite.FactoryType;
using FeatureLevel = SharpDX.Direct3D.FeatureLevel;
using PixelFormat = SharpDX.Direct2D1.PixelFormat;
using Rectangle = System.Drawing.Rectangle;
using Resource = SharpDX.Direct3D11.Resource;

namespace Geisha.Framework.Rendering.DirectX
{
    // TODO introduce batch rendering? I.e. SpriteBatch?
    public sealed class Renderer2D : IRenderer2D
    {
        private readonly Surface _backBufferSurface;
        private readonly Texture2D _backBufferTexture;
        private readonly Factory _d2D1Factory;
        private readonly RenderTarget _d2D1RenderTarget;
        private readonly Device _d3D11Device;
        private readonly SharpDX.DXGI.Factory _dxgiFactory;
        private readonly SwapChain _dxgiSwapChain;

        public Renderer2D(IWindow window)
        {
            Window = window;

            var swapChainDescription = new SwapChainDescription
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(Window.ClientAreaWidth, Window.ClientAreaHeight, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = Window.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };


            Device.CreateWithSwapChain(
                DriverType.Hardware,
                DeviceCreationFlags.BgraSupport,
                new[] {FeatureLevel.Level_11_0},
                swapChainDescription,
                out _d3D11Device,
                out _dxgiSwapChain);

            _dxgiFactory = _dxgiSwapChain.GetParent<SharpDX.DXGI.Factory>();
            _dxgiFactory.MakeWindowAssociation(Window.Handle, WindowAssociationFlags.IgnoreAll); // Ignore all windows events

            _backBufferTexture = Resource.FromSwapChain<Texture2D>(_dxgiSwapChain, 0);

            _backBufferSurface = _backBufferTexture.QueryInterface<Surface>();

            _d2D1Factory = new Factory();

            _d2D1RenderTarget = new RenderTarget(_d2D1Factory, _backBufferSurface,
                new RenderTargetProperties(new PixelFormat(Format.Unknown, AlphaMode.Premultiplied)));
        }

        private Vector2 WindowCenter => new Vector2(Window.ClientAreaWidth / 2d, Window.ClientAreaHeight / 2d);

        public IWindow Window { get; }

        // TODO It should specify more clearly what formats are supported and maybe expose some importer extensions?
        public ITexture CreateTexture(Stream stream)
        {
            // Create GDI bitmap from stream
            using (var gdiBitmap = new Bitmap(stream))
            {
                SharpDX.Direct2D1.Bitmap d2D1Bitmap;

                // Get access to raw GDI bitmap data
                var gdiBitmapData = gdiBitmap.LockBits(new Rectangle(0, 0, gdiBitmap.Width, gdiBitmap.Height), ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                // Fill data stream with GDI bitmap data to create Direct2D1 bitmap from it
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

                    // Set data stream position at the beginning
                    convertedBitmapDataStream.Position = 0;

                    // Create Direct2D1 bitmap from data stream
                    d2D1Bitmap = new SharpDX.Direct2D1.Bitmap(_d2D1RenderTarget, new Size2(gdiBitmap.Width, gdiBitmap.Height), convertedBitmapDataStream,
                        stride,
                        new BitmapProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)));
                }

                // Close access to raw GDI bitmap data
                gdiBitmap.UnlockBits(gdiBitmapData);

                // Create texture from Direct2D1 bitmap
                return new Texture(d2D1Bitmap);
            }
        }

        public void BeginRendering()
        {
            _d2D1RenderTarget.BeginDraw();
        }

        public void EndRendering(bool waitForVSync)
        {
            _d2D1RenderTarget.EndDraw();

            // Present rendering results to the screen
            _dxgiSwapChain.Present(waitForVSync ? 1 : 0, PresentFlags.None);
        }

        public void Clear(Color color)
        {
            _d2D1RenderTarget.Clear(color.ToRawColor4());
        }

        public void RenderSprite(Sprite sprite, Matrix3x3 transform)
        {
            // Extract Direct2D1 bitmap from sprite
            var d2D1Bitmap = ((Texture) sprite.SourceTexture).D2D1Bitmap;

            // Prepare destination rectangle to draw bitmap in final view and source rectangle to read specified part of bitmap for drawing
            var spriteRectangle = sprite.Rectangle;
            var destinationRawRectangleF = spriteRectangle.ToRawRectangleF();
            var sourceRawRectangleF = new RawRectangleF((float) sprite.SourceUV.X, (float) sprite.SourceUV.Y,
                (float) (sprite.SourceUV.X + sprite.SourceDimension.X), (float) (sprite.SourceUV.Y + sprite.SourceDimension.Y));

            // Convert Geisha matrix to DirectX matrix
            _d2D1RenderTarget.Transform = CreateMatrixWithAdjustedCoordinatesSystem(transform);

            // Draw a bitmap
            _d2D1RenderTarget.DrawBitmap(d2D1Bitmap, destinationRawRectangleF, 1.0f, BitmapInterpolationMode.Linear, sourceRawRectangleF);
        }

        public void RenderText(string text, FontSize fontSize, Color color, Matrix3x3 transform)
        {
            // TODO Creating these resources each time is quite expensive. There is space for optimization.
            // Create brush with given color
            using (var d2D1SolidColorBrush = new SolidColorBrush(_d2D1RenderTarget, color.ToRawColor4()))
            {
                using (var dwFactory = new SharpDX.DirectWrite.Factory(FactoryType.Shared))
                {
                    // Create text format with given font properties
                    using (var textFormat = new TextFormat(dwFactory, "Consolas", FontWeight.Normal, FontStyle.Normal, (float) fontSize.Dips))
                    {
                        // Convert Geisha matrix to DirectX matrix
                        _d2D1RenderTarget.Transform = CreateMatrixWithAdjustedCoordinatesSystem(transform);

                        // Draw text
                        _d2D1RenderTarget.DrawText(text, textFormat, new RawRectangleF(0, 0, float.MaxValue, float.MaxValue), d2D1SolidColorBrush);
                    }
                }
            }
        }

        public void RenderRectangle(Vector2 dimension, Color color, bool fillInterior, Matrix3x3 transform)
        {
            var rectangle = new Common.Math.Rectangle(dimension);
            var rawRectangleF = rectangle.ToRawRectangleF();

            // TODO Creating these resources each time is quite expensive. There is space for optimization.
            // Create brush with given color
            using (var d2D1SolidColorBrush = new SolidColorBrush(_d2D1RenderTarget, color.ToRawColor4()))
            {
                // Convert Geisha matrix to DirectX matrix
                _d2D1RenderTarget.Transform = CreateMatrixWithAdjustedCoordinatesSystem(transform);

                // Draw rectangle
                _d2D1RenderTarget.DrawRectangle(rawRectangleF, d2D1SolidColorBrush);
                if (fillInterior) _d2D1RenderTarget.FillRectangle(rawRectangleF, d2D1SolidColorBrush);
            }
        }

        /// <summary>
        ///     Converts given <see cref="Matrix3x3" /> transform to Direct2D <see cref="RawMatrix3x2" /> adjusting coordinates
        ///     system.
        /// </summary>
        /// <remarks>
        ///     Direct2D renders from upper left corner with Y axis towards bottom of the screen while it is required to have
        ///     origin in center of screen with Y axis towards top of the screen.
        /// </remarks>
        /// <param name="transform">Raw transform to be used for rendering.</param>
        /// <returns></returns>
        private RawMatrix3x2 CreateMatrixWithAdjustedCoordinatesSystem(Matrix3x3 transform)
        {
            // Prepare transformation matrix to be used in rendering
            var finalTransform =
                Matrix3x3.CreateTranslation(WindowCenter) * // Set coordinates system origin to center of the screen
                new Matrix3x3(
                    transform.M11, -transform.M12, transform.M13,
                    -transform.M21, transform.M22, -transform.M23,
                    transform.M31, transform.M32, transform.M33
                ) * // Make Y axis to point towards top of the screen
                Matrix3x3.Identity;

            // Convert Geisha matrix to DirectX matrix
            return new RawMatrix3x2(
                (float) finalTransform.M11, (float) finalTransform.M21,
                (float) finalTransform.M12, (float) finalTransform.M22,
                (float) finalTransform.M13, (float) finalTransform.M23);
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