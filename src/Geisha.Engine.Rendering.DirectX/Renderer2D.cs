﻿using System;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Geisha.Common.Math;
using Geisha.Engine.Rendering.Backend;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.IO;
using SharpDX.Mathematics.Interop;
using SharpDX.WIC;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Bitmap = System.Drawing.Bitmap;
using BitmapInterpolationMode = SharpDX.Direct2D1.BitmapInterpolationMode;
using Device = SharpDX.Direct3D11.Device;
using Ellipse = Geisha.Common.Math.Ellipse;
using Factory = SharpDX.Direct2D1.Factory;
using FactoryType = SharpDX.DirectWrite.FactoryType;
using FeatureLevel = SharpDX.Direct3D.FeatureLevel;
using MapFlags = SharpDX.DXGI.MapFlags;
using PixelFormat = SharpDX.Direct2D1.PixelFormat;

namespace Geisha.Engine.Rendering.DirectX
{
    // TODO introduce batch rendering? I.e. SpriteBatch?
    internal sealed class Renderer2D : IRenderer2D, IDisposable
    {
        private readonly Surface _backBufferSurface;
        private readonly Factory _d2D1Factory;
        private readonly SharpDX.Direct2D1.DeviceContext _d2D1RenderTarget;
        private readonly Device _d3D11Device;
        private readonly SharpDX.DXGI.Factory _dxgiFactory;
        private readonly SwapChain _dxgiSwapChain;
        private readonly Form _form;
        private bool _clippingEnabled = false;

        public Renderer2D(Form form)
        {
            _form = form;

            var swapChainDescription = new SwapChainDescription
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(_form.ClientSize.Width, _form.ClientSize.Height, new Rational(60, 1), Format.B8G8R8A8_UNorm),
                IsWindowed = true,
                OutputHandle = _form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            Device.CreateWithSwapChain(
                DriverType.Hardware,
                DeviceCreationFlags.BgraSupport, // TODO Investigate DeviceCreationFlags.Debug
                new[] { FeatureLevel.Level_11_0 },
                swapChainDescription,
                out _d3D11Device,
                out _dxgiSwapChain);

            _dxgiFactory = _dxgiSwapChain.GetParent<SharpDX.DXGI.Factory>();
            _dxgiFactory.MakeWindowAssociation(_form.Handle, WindowAssociationFlags.IgnoreAll); // Ignore all windows events

            _backBufferSurface = _dxgiSwapChain.GetBackBuffer<Surface>(0);

            var dxgiDevice = _d3D11Device.QueryInterface<SharpDX.DXGI.Device>();
            var d = new SharpDX.Direct2D1.Device(dxgiDevice);
            _d2D1RenderTarget = new SharpDX.Direct2D1.DeviceContext(d, DeviceContextOptions.None);
            var bmp = new SharpDX.Direct2D1.Bitmap(_d2D1RenderTarget, _backBufferSurface,
                new BitmapProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)));
            _d2D1RenderTarget.Target = bmp;

            _d2D1Factory = new Factory();
            //_d2D1RenderTarget = new RenderTarget(_d2D1Factory, _backBufferSurface,
            //    new RenderTargetProperties(new PixelFormat(Format.Unknown, AlphaMode.Premultiplied)));
        }

        private Vector2 WindowCenter => new Vector2(ScreenWidth / 2d, ScreenHeight / 2d);

        public int ScreenWidth => _form.ClientSize.Width;
        public int ScreenHeight => _form.ClientSize.Height;

        // TODO It should specify more clearly what formats are supported and maybe expose some importer extensions?
        public ITexture CreateTexture(Stream stream)
        {
            using var gdiBitmap = new Bitmap(stream);
            SharpDX.Direct2D1.Bitmap d2D1Bitmap;

            // Get access to raw GDI bitmap data
            var gdiBitmapData = gdiBitmap.LockBits(new System.Drawing.Rectangle(0, 0, gdiBitmap.Width, gdiBitmap.Height), ImageLockMode.ReadOnly,
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

                convertedBitmapDataStream.Position = 0;

                // Create Direct2D1 bitmap from data stream
                d2D1Bitmap = new SharpDX.Direct2D1.Bitmap(_d2D1RenderTarget, new Size2(gdiBitmap.Width, gdiBitmap.Height), convertedBitmapDataStream, stride,
                    new BitmapProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)));
            }

            // Close access to raw GDI bitmap data
            gdiBitmap.UnlockBits(gdiBitmapData);

            return new Texture(d2D1Bitmap);
        }

        public void CaptureScreenShotPng(Stream stream)
        {
            using var cpuBitmap = new Bitmap1(_d2D1RenderTarget, _d2D1RenderTarget.PixelSize,
                new BitmapProperties1(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied), _d2D1RenderTarget.DotsPerInch.Width,
                    _d2D1RenderTarget.DotsPerInch.Height, BitmapOptions.CpuRead | BitmapOptions.CannotDraw));
            cpuBitmap.CopyFromRenderTarget(_d2D1RenderTarget);

            var dr = cpuBitmap.Surface.Map(MapFlags.Read, out var dataStream);
            var surfaceDescription = cpuBitmap.Surface.Description;

            using (dataStream)
            {
                using var gdiBitmap = new Bitmap(surfaceDescription.Width, surfaceDescription.Height);

                for (var y = 0; y < surfaceDescription.Height; y++)
                {
                    for (var x = 0; x < surfaceDescription.Width; x++)
                    {
                        dataStream.Seek((y * dr.Pitch) + (x * sizeof(int)), SeekOrigin.Begin);
                        var pixel = dataStream.Read<int>();
                        gdiBitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(pixel));
                    }
                }

                gdiBitmap.Save(stream, ImageFormat.Png);
            }

            // TODO Should it be in finally?
            cpuBitmap.Surface.Unmap();
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

        public void RenderSprite(Sprite sprite, in Matrix3x3 transform)
        {
            var d2D1Bitmap = ((Texture)sprite.SourceTexture).D2D1Bitmap;

            // Prepare destination rectangle to draw bitmap in final view and source rectangle to read specified part of bitmap for drawing
            var spriteRectangle = sprite.Rectangle;
            var destinationRawRectangleF = spriteRectangle.ToRawRectangleF();
            var sourceRawRectangleF = new RawRectangleF((float)sprite.SourceUV.X, (float)sprite.SourceUV.Y,
                (float)(sprite.SourceUV.X + sprite.SourceDimensions.X), (float)(sprite.SourceUV.Y + sprite.SourceDimensions.Y));

            _d2D1RenderTarget.Transform = ConvertTransformToDirectX(transform);
            _d2D1RenderTarget.DrawBitmap(d2D1Bitmap, destinationRawRectangleF, 1.0f, BitmapInterpolationMode.Linear, sourceRawRectangleF);
        }

        public void RenderText(string text, FontSize fontSize, Color color, in Matrix3x3 transform)
        {
            // TODO Creating these resources each time is quite expensive. There is space for optimization.
            using var d2D1SolidColorBrush = new SolidColorBrush(_d2D1RenderTarget, color.ToRawColor4());
            using var dwFactory = new SharpDX.DirectWrite.Factory(FactoryType.Shared);
            using var textFormat = new TextFormat(dwFactory, "Consolas", FontWeight.Normal, FontStyle.Normal, (float)fontSize.Dips);

            _d2D1RenderTarget.Transform = ConvertTransformToDirectX(transform);
            _d2D1RenderTarget.DrawText(text, textFormat, new RawRectangleF(0, 0, float.MaxValue, float.MaxValue), d2D1SolidColorBrush);
        }

        public void RenderRectangle(in AxisAlignedRectangle rectangle, Color color, bool fillInterior, in Matrix3x3 transform)
        {
            var rawRectangleF = rectangle.ToRawRectangleF();

            // TODO Creating these resources each time is quite expensive. There is space for optimization.
            using var d2D1SolidColorBrush = new SolidColorBrush(_d2D1RenderTarget, color.ToRawColor4());

            _d2D1RenderTarget.Transform = ConvertTransformToDirectX(transform);
            _d2D1RenderTarget.DrawRectangle(rawRectangleF, d2D1SolidColorBrush);
            if (fillInterior) _d2D1RenderTarget.FillRectangle(rawRectangleF, d2D1SolidColorBrush);
        }

        public void RenderEllipse(in Ellipse ellipse, Color color, bool fillInterior, in Matrix3x3 transform)
        {
            var directXEllipse = ellipse.ToDirectXEllipse();

            // TODO Creating these resources each time is quite expensive. There is space for optimization.
            using var d2D1SolidColorBrush = new SolidColorBrush(_d2D1RenderTarget, color.ToRawColor4());

            _d2D1RenderTarget.Transform = ConvertTransformToDirectX(transform);
            _d2D1RenderTarget.DrawEllipse(directXEllipse, d2D1SolidColorBrush);
            if (fillInterior) _d2D1RenderTarget.FillEllipse(directXEllipse, d2D1SolidColorBrush);
        }

        public void SetClippingRectangle(in AxisAlignedRectangle clippingRectangle)
        {
            if (_clippingEnabled)
            {
                _d2D1RenderTarget.PopAxisAlignedClip();
            }

            _clippingEnabled = true;
            _d2D1RenderTarget.Transform = ConvertTransformToDirectX(Matrix3x3.Identity);
            _d2D1RenderTarget.PushAxisAlignedClip(clippingRectangle.ToRawRectangleF(), AntialiasMode.Aliased);
        }

        public void ClearClipping()
        {
            if (_clippingEnabled)
            {
                _clippingEnabled = false;
                _d2D1RenderTarget.PopAxisAlignedClip();
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