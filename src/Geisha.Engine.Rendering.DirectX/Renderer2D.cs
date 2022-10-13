using System;
using System.IO;
using System.Windows.Forms;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering.Backend;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using BitmapInterpolationMode = SharpDX.Direct2D1.BitmapInterpolationMode;
using Color = Geisha.Engine.Core.Math.Color;
using Device = SharpDX.Direct3D11.Device;
using Ellipse = Geisha.Engine.Core.Math.Ellipse;
using FactoryType = SharpDX.DirectWrite.FactoryType;
using FeatureLevel = SharpDX.Direct3D.FeatureLevel;
using Image = SixLabors.ImageSharp.Image;
using MapFlags = SharpDX.DXGI.MapFlags;
using PixelFormat = SharpDX.Direct2D1.PixelFormat;
using Rational = SharpDX.DXGI.Rational;

namespace Geisha.Engine.Rendering.DirectX
{
    // TODO introduce batch rendering? I.e. SpriteBatch?
    internal sealed class Renderer2D : IRenderer2D, IDisposable
    {
        private readonly SharpDX.Direct2D1.DeviceContext _d2D1DeviceContext;
        private readonly Device _d3D11Device;
        private readonly SwapChain _dxgiSwapChain;
        private readonly SolidColorBrush _d2D1SolidColorBrush;
        private readonly Form _form;
        private bool _clippingEnabled = false;

        public Renderer2D(Form form, DriverType driverType)
        {
            _form = form;

            var swapChainDescription = new SwapChainDescription
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(_form.ClientSize.Width, _form.ClientSize.Height, new Rational(60, 1), Format.B8G8R8A8_UNorm),
                IsWindowed = true,
                OutputHandle = _form.Handle,
                SampleDescription = new SampleDescription(1, 0),
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
            dxgiFactory.MakeWindowAssociation(_form.Handle, WindowAssociationFlags.IgnoreAll); // Ignore all windows events

            using var dxgiDevice = _d3D11Device.QueryInterface<SharpDX.DXGI.Device>();
            using var d2D1Device = new SharpDX.Direct2D1.Device(dxgiDevice);
            _d2D1DeviceContext = new SharpDX.Direct2D1.DeviceContext(d2D1Device, DeviceContextOptions.None);

            using var backBufferSurface = _dxgiSwapChain.GetBackBuffer<Surface>(0);
            var renderTargetBitmap = new Bitmap(_d2D1DeviceContext, backBufferSurface,
                new BitmapProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)));
            _d2D1DeviceContext.Target = renderTargetBitmap;

            _d2D1SolidColorBrush = new SolidColorBrush(_d2D1DeviceContext, default);
        }

        private Vector2 WindowCenter => new(ScreenWidth / 2d, ScreenHeight / 2d);

        public int ScreenWidth => _form.ClientSize.Width;
        public int ScreenHeight => _form.ClientSize.Height;

        // TODO It should specify more clearly what formats are supported and maybe expose some importer extensions?
        public ITexture CreateTexture(Stream stream)
        {
            using var cpuBitmap = Image.Load<Bgra32>(stream);

            // Fill data stream with CPU bitmap data to create Direct2D1 bitmap from it.
            const int bitsPerByte = 8;
            var stride = cpuBitmap.Width * cpuBitmap.PixelType.BitsPerPixel / bitsPerByte;
            using var bitmapDataStream = new DataStream(cpuBitmap.Height * stride, true, true);
            cpuBitmap.ProcessPixelRows(accessor =>
            {
                for (var y = 0; y < accessor.Height; y++)
                {
                    var pixelRow = accessor.GetRowSpan(y);
                    foreach (ref var pixel in pixelRow)
                    {
                        // ReSharper disable AccessToDisposedClosure
                        bitmapDataStream.WriteByte((byte)Math.Round(pixel.B * pixel.A / 255d)); // Convert to premultiplied alpha.
                        bitmapDataStream.WriteByte((byte)Math.Round(pixel.G * pixel.A / 255d)); // Convert to premultiplied alpha.
                        bitmapDataStream.WriteByte((byte)Math.Round(pixel.R * pixel.A / 255d)); // Convert to premultiplied alpha.
                        bitmapDataStream.WriteByte(pixel.A);
                        // ReSharper restore AccessToDisposedClosure
                    }
                }
            });

            bitmapDataStream.Position = 0;

            // Create Direct2D1 bitmap from data stream
            var d2D1Bitmap = new Bitmap(_d2D1DeviceContext, new Size2(cpuBitmap.Width, cpuBitmap.Height), bitmapDataStream, stride,
                new BitmapProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)));

            return new Texture(d2D1Bitmap);
        }

        public void CaptureScreenShotAsPng(Stream stream)
        {
            using var d2D1CpuBitmap = new Bitmap1(_d2D1DeviceContext, _d2D1DeviceContext.PixelSize,
                new BitmapProperties1(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied), _d2D1DeviceContext.DotsPerInch.Width,
                    _d2D1DeviceContext.DotsPerInch.Height, BitmapOptions.CpuRead | BitmapOptions.CannotDraw));
            d2D1CpuBitmap.CopyFromRenderTarget(_d2D1DeviceContext);

            var dataRectangle = d2D1CpuBitmap.Surface.Map(MapFlags.Read, out var dataStream);
            try
            {
                var surfaceDescription = d2D1CpuBitmap.Surface.Description;

                using (dataStream)
                {
                    using var cpuBitmap = new Image<Bgra32>(surfaceDescription.Width, surfaceDescription.Height);

                    for (var y = 0; y < surfaceDescription.Height; y++)
                    {
                        for (var x = 0; x < surfaceDescription.Width; x++)
                        {
                            dataStream.Seek((y * dataRectangle.Pitch) + (x * sizeof(int)), SeekOrigin.Begin);
                            var b = (byte)dataStream.ReadByte();
                            var g = (byte)dataStream.ReadByte();
                            var r = (byte)dataStream.ReadByte();
                            var a = (byte)dataStream.ReadByte();
                            cpuBitmap[x, y] = new Bgra32(r, g, b, a);
                        }
                    }

                    cpuBitmap.SaveAsPng(stream);
                }
            }
            finally
            {
                d2D1CpuBitmap.Surface.Unmap();
            }
        }

        public void BeginRendering()
        {
            _d2D1DeviceContext.BeginDraw();
        }

        public void EndRendering(bool waitForVSync)
        {
            _d2D1DeviceContext.EndDraw();

            // Present rendering results to the screen
            _dxgiSwapChain.Present(waitForVSync ? 1 : 0, PresentFlags.None);
        }

        public void Clear(Color color)
        {
            _d2D1DeviceContext.Clear(color.ToRawColor4());
        }

        public void RenderSprite(Sprite sprite, in Matrix3x3 transform)
        {
            var d2D1Bitmap = ((Texture)sprite.SourceTexture).D2D1Bitmap;

            // Prepare destination rectangle to draw bitmap in final view and source rectangle to read specified part of bitmap for drawing
            var spriteRectangle = sprite.Rectangle;
            var destinationRawRectangleF = spriteRectangle.ToRawRectangleF();
            var sourceRawRectangleF = new RawRectangleF((float)sprite.SourceUV.X, (float)sprite.SourceUV.Y,
                (float)(sprite.SourceUV.X + sprite.SourceDimensions.X), (float)(sprite.SourceUV.Y + sprite.SourceDimensions.Y));

            _d2D1DeviceContext.Transform = ConvertTransformToDirectX(transform);
            _d2D1DeviceContext.DrawBitmap(d2D1Bitmap, destinationRawRectangleF, 1.0f, BitmapInterpolationMode.Linear, sourceRawRectangleF);
        }

        public void RenderText(string text, FontSize fontSize, Color color, in Matrix3x3 transform)
        {
            // TODO Creating these resources each time is quite expensive. There is space for optimization.
            _d2D1SolidColorBrush.Color = color.ToRawColor4();
            using var dwFactory = new SharpDX.DirectWrite.Factory(FactoryType.Shared);
            using var textFormat = new TextFormat(dwFactory, "Consolas", FontWeight.Normal, FontStyle.Normal, (float)fontSize.Dips);

            _d2D1DeviceContext.Transform = ConvertTransformToDirectX(transform);
            _d2D1DeviceContext.DrawText(text, textFormat, new RawRectangleF(0, 0, float.MaxValue, float.MaxValue), _d2D1SolidColorBrush);
        }

        public void RenderRectangle(in AxisAlignedRectangle rectangle, Color color, bool fillInterior, in Matrix3x3 transform)
        {
            var rawRectangleF = rectangle.ToRawRectangleF();

            _d2D1SolidColorBrush.Color = color.ToRawColor4();

            _d2D1DeviceContext.Transform = ConvertTransformToDirectX(transform);
            _d2D1DeviceContext.DrawRectangle(rawRectangleF, _d2D1SolidColorBrush);
            if (fillInterior) _d2D1DeviceContext.FillRectangle(rawRectangleF, _d2D1SolidColorBrush);
        }

        public void RenderEllipse(in Ellipse ellipse, Color color, bool fillInterior, in Matrix3x3 transform)
        {
            var directXEllipse = ellipse.ToDirectXEllipse();

            _d2D1SolidColorBrush.Color = color.ToRawColor4();

            _d2D1DeviceContext.Transform = ConvertTransformToDirectX(transform);
            _d2D1DeviceContext.DrawEllipse(directXEllipse, _d2D1SolidColorBrush);
            if (fillInterior) _d2D1DeviceContext.FillEllipse(directXEllipse, _d2D1SolidColorBrush);
        }

        public void SetClippingRectangle(in AxisAlignedRectangle clippingRectangle)
        {
            if (_clippingEnabled)
            {
                _d2D1DeviceContext.PopAxisAlignedClip();
            }

            _clippingEnabled = true;
            _d2D1DeviceContext.Transform = ConvertTransformToDirectX(Matrix3x3.Identity);
            _d2D1DeviceContext.PushAxisAlignedClip(clippingRectangle.ToRawRectangleF(), AntialiasMode.Aliased);
        }

        public void ClearClipping()
        {
            if (_clippingEnabled)
            {
                _clippingEnabled = false;
                _d2D1DeviceContext.PopAxisAlignedClip();
            }
            else
            {
                throw new InvalidOperationException("No clipping rectangle is defined.");
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
        private RawMatrix3x2 ConvertTransformToDirectX(in Matrix3x3 transform)
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
                (float)finalTransform.M11, (float)finalTransform.M21,
                (float)finalTransform.M12, (float)finalTransform.M22,
                (float)finalTransform.M13, (float)finalTransform.M23);
        }

        public void Dispose()
        {
            _d2D1SolidColorBrush.Dispose();
            _d2D1DeviceContext.Dispose();
            _dxgiSwapChain.Dispose();
            _d3D11Device.Dispose();
        }
    }
}