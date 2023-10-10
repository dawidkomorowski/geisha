using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering.Backend;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using BitmapInterpolationMode = SharpDX.Direct2D1.BitmapInterpolationMode;
using Color = Geisha.Engine.Core.Math.Color;
using Ellipse = Geisha.Engine.Core.Math.Ellipse;
using FactoryType = SharpDX.DirectWrite.FactoryType;
using Image = SixLabors.ImageSharp.Image;
using MapFlags = SharpDX.DXGI.MapFlags;
using PixelFormat = SharpDX.Direct2D1.PixelFormat;

namespace Geisha.Engine.Rendering.DirectX
{
    internal sealed class RenderingContext2D : IRenderingContext2D, IDisposable
    {
        private readonly Form _form;
        private readonly DeviceContext3 _d2D1DeviceContext;
        private readonly Statistics _statistics;
        private readonly SharpDX.DirectWrite.Factory _dwFactory;
        private readonly SolidColorBrush _d2D1SolidColorBrush;
        private bool _clippingEnabled;

        public RenderingContext2D(Form form, DeviceContext3 d2D1DeviceContext, Statistics statistics)
        {
            _form = form;
            _d2D1DeviceContext = d2D1DeviceContext;
            _statistics = statistics;
            _dwFactory = new SharpDX.DirectWrite.Factory(FactoryType.Shared);
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

        public ITextLayout CreateTextLayout(string text, string fontFamilyName, FontSize fontSize, double maxWidth, double maxHeight)
        {
            var dwTextFormat = new TextFormat(_dwFactory, fontFamilyName, FontWeight.Normal, FontStyle.Normal, (float)fontSize.Dips);
            var dwTextLayout = new SharpDX.DirectWrite.TextLayout(_dwFactory, text, dwTextFormat, (float)maxWidth, (float)maxHeight);
            return new TextLayout(dwTextFormat, dwTextLayout, text);
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

        public void BeginDraw()
        {
            _d2D1DeviceContext.BeginDraw();
        }

        public void EndDraw()
        {
            _d2D1DeviceContext.EndDraw();
        }

        public void Clear(Color color)
        {
            _d2D1DeviceContext.Clear(color.ToRawColor4());
        }

        public void DrawSprite(Sprite sprite, in Matrix3x3 transform, double opacity = 1d)
        {
            var d2D1Bitmap = ((Texture)sprite.SourceTexture).D2D1Bitmap;

            // Prepare destination rectangle to draw bitmap in final view and source rectangle to read specified part of bitmap for drawing
            var spriteRectangle = sprite.Rectangle;
            var destinationRawRectangleF = spriteRectangle.ToRawRectangleF();
            var sourceRawRectangleF = new RawRectangleF((float)sprite.SourceUV.X, (float)sprite.SourceUV.Y,
                (float)(sprite.SourceUV.X + sprite.SourceDimensions.X), (float)(sprite.SourceUV.Y + sprite.SourceDimensions.Y));

            _d2D1DeviceContext.Transform = ConvertTransformToDirectX(transform);
            _d2D1DeviceContext.DrawBitmap(d2D1Bitmap, destinationRawRectangleF, (float)opacity, BitmapInterpolationMode.Linear, sourceRawRectangleF);

            _statistics.IncrementDrawCalls();
        }

        public void DrawSpriteBatch(Span<Sprite> sprites, Span<Matrix3x3> transforms, Span<double> opacities)
        {
            Debug.Assert(sprites.Length == transforms.Length, "sprites.Length == transforms.Length");
            Debug.Assert(sprites.Length == opacities.Length, "sprites.Length == opacities.Length");

            var spritesCount = sprites.Length;
            var d2D1Bitmap = ((Texture)sprites[0].SourceTexture).D2D1Bitmap;

            _d2D1DeviceContext.Transform = new RawMatrix3x2(1, 0, 0, 1, 0, 0);

            var destinationRectangles = ArrayPool<RawRectangleF>.Shared.Rent(spritesCount);
            var sourceRectangles = ArrayPool<RawRectangle>.Shared.Rent(spritesCount);
            var colors = ArrayPool<RawColor4>.Shared.Rent(spritesCount);
            var dxTransforms = ArrayPool<RawMatrix3x2>.Shared.Rent(spritesCount);

            try
            {
                for (var i = 0; i < spritesCount; i++)
                {
                    var sprite = sprites[i];

                    destinationRectangles[i] = sprite.Rectangle.ToRawRectangleF();
                    sourceRectangles[i] = new RawRectangle((int)sprite.SourceUV.X, (int)sprite.SourceUV.Y,
                        (int)(sprite.SourceUV.X + sprite.SourceDimensions.X), (int)(sprite.SourceUV.Y + sprite.SourceDimensions.Y));
                    colors[i] = new RawColor4(1f, 1f, 1f, (float)opacities[i]);
                    dxTransforms[i] = ConvertTransformToDirectX(transforms[i]);
                }

                using var spriteBatch = new SpriteBatch(_d2D1DeviceContext);
                spriteBatch.AddSprites(
                    spritesCount,
                    destinationRectangles,
                    sourceRectangles,
                    colors,
                    dxTransforms,
                    Marshal.SizeOf<RawRectangleF>(),
                    Marshal.SizeOf<RawRectangle>(),
                    Marshal.SizeOf<RawColor4>(),
                    Marshal.SizeOf<RawMatrix3x2>()
                );

                _d2D1DeviceContext.DrawSpriteBatch(spriteBatch, 0, spriteBatch.SpriteCount, d2D1Bitmap, BitmapInterpolationMode.Linear, SpriteOptions.None);
            }
            finally
            {
                ArrayPool<RawRectangleF>.Shared.Return(destinationRectangles);
                ArrayPool<RawRectangle>.Shared.Return(sourceRectangles);
                ArrayPool<RawColor4>.Shared.Return(colors);
                ArrayPool<RawMatrix3x2>.Shared.Return(dxTransforms);
            }

            _statistics.IncrementDrawCalls();
        }

        public void DrawText(string text, string fontFamilyName, FontSize fontSize, Color color, in Matrix3x3 transform)
        {
            using var textFormat = new TextFormat(_dwFactory, fontFamilyName, FontWeight.Normal, FontStyle.Normal, (float)fontSize.Dips);

            _d2D1SolidColorBrush.Color = color.ToRawColor4();

            _d2D1DeviceContext.Transform = ConvertTransformToDirectX(transform);
            _d2D1DeviceContext.DrawText(text, textFormat, new RawRectangleF(0, 0, float.MaxValue, float.MaxValue), _d2D1SolidColorBrush);

            _statistics.IncrementDrawCalls();
        }

        public void DrawTextLayout(ITextLayout textLayout, Color color, in Vector2 pivot, in Matrix3x3 transform, bool clipToLayoutBox = false)
        {
            var internalTextLayout = (TextLayout)textLayout;
            var drawTextOptions = DrawTextOptions.None;
            if (clipToLayoutBox)
            {
                drawTextOptions |= DrawTextOptions.Clip;
            }

            _d2D1SolidColorBrush.Color = color.ToRawColor4();

            _d2D1DeviceContext.Transform = ConvertTransformToDirectX(transform);
            _d2D1DeviceContext.DrawTextLayout(
                new RawVector2((float)-pivot.X, (float)-pivot.Y),
                internalTextLayout.DWTextLayout,
                _d2D1SolidColorBrush,
                drawTextOptions
            );

            _statistics.IncrementDrawCalls();
        }

        public void DrawRectangle(in AxisAlignedRectangle rectangle, Color color, bool fillInterior, in Matrix3x3 transform)
        {
            var rawRectangleF = rectangle.ToRawRectangleF();

            _d2D1SolidColorBrush.Color = color.ToRawColor4();

            _d2D1DeviceContext.Transform = ConvertTransformToDirectX(transform);
            _d2D1DeviceContext.DrawRectangle(rawRectangleF, _d2D1SolidColorBrush);
            _statistics.IncrementDrawCalls();

            if (fillInterior)
            {
                _d2D1DeviceContext.FillRectangle(rawRectangleF, _d2D1SolidColorBrush);
                _statistics.IncrementDrawCalls();
            }
        }

        public void DrawEllipse(in Ellipse ellipse, Color color, bool fillInterior, in Matrix3x3 transform)
        {
            var directXEllipse = ellipse.ToDirectXEllipse();

            _d2D1SolidColorBrush.Color = color.ToRawColor4();

            _d2D1DeviceContext.Transform = ConvertTransformToDirectX(transform);
            _d2D1DeviceContext.DrawEllipse(directXEllipse, _d2D1SolidColorBrush);
            _statistics.IncrementDrawCalls();

            if (fillInterior)
            {
                _d2D1DeviceContext.FillEllipse(directXEllipse, _d2D1SolidColorBrush);
                _statistics.IncrementDrawCalls();
            }
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
            // TODO This method can be greatly optimized by removing redundant matrix multiplications.
            // Prepare transformation matrix to be used in rendering
            var finalTransform =
                Matrix3x3.CreateTranslation(WindowCenter) * // Set coordinates system origin to center of the screen
                new Matrix3x3(
                    transform.M11, -transform.M12, transform.M13,
                    -transform.M21, transform.M22, -transform.M23,
                    transform.M31, transform.M32, transform.M33
                ) * // Make Y axis to point towards top of the screen
                Matrix3x3.Identity; // TODO Is this identity multiplication needed? How it impacts performance?

            // Convert Geisha matrix to DirectX matrix
            return new RawMatrix3x2(
                (float)finalTransform.M11, (float)finalTransform.M21,
                (float)finalTransform.M12, (float)finalTransform.M22,
                (float)finalTransform.M13, (float)finalTransform.M23);
        }

        public void Dispose()
        {
            _d2D1SolidColorBrush.Dispose();
            _dwFactory.Dispose();
        }
    }
}