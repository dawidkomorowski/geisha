using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using Geisha.Common.Math;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.Windows;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Bitmap = SharpDX.Direct2D1.Bitmap;
using Device = SharpDX.Direct3D11.Device;
using Factory = SharpDX.Direct2D1.Factory;
using FactoryType = SharpDX.DirectWrite.FactoryType;
using FeatureLevel = SharpDX.Direct3D.FeatureLevel;
using FontStyle = SharpDX.DirectWrite.FontStyle;
using PixelFormat = SharpDX.Direct2D1.PixelFormat;
using Rectangle = System.Drawing.Rectangle;
using Resource = SharpDX.Direct3D11.Resource;
using Transform = Geisha.Engine.Core.Components.Transform;

namespace Geisha.SharpDXTestApp
{
    internal static class Program
    {
        private static Random Random { get; } = new Random();

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            using (var form = new RenderForm("SharpDXTestApp") {ClientSize = new Size(1280, 720)})
            {
                var swapChainDescription = new SwapChainDescription
                {
                    BufferCount = 1,
                    ModeDescription = new ModeDescription(form.ClientSize.Width, form.ClientSize.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                    IsWindowed = true,
                    OutputHandle = form.Handle,
                    SampleDescription = new SampleDescription(1, 0),
                    SwapEffect = SwapEffect.Discard,
                    Usage = Usage.RenderTargetOutput
                };

                Device.CreateWithSwapChain(
                    DriverType.Hardware,
                    DeviceCreationFlags.BgraSupport,
                    new[] {FeatureLevel.Level_10_0},
                    swapChainDescription,
                    out var d3D11Device,
                    out var dxgiSwapChain);

                using (dxgiSwapChain)
                {
                    using (d3D11Device)
                    {
                        using (var d2D1Factory = new Factory())
                        {
                            using (var dxgiFactory = dxgiSwapChain.GetParent<SharpDX.DXGI.Factory>())
                            {
                                // Ignore all windows events
                                dxgiFactory.MakeWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAll);

                                using (var backBuffer = Resource.FromSwapChain<Texture2D>(dxgiSwapChain, 0))
                                {
                                    // TODO What is it responsible for?
                                    //using (var renderTargetView = new RenderTargetView(d3DDevice, backBuffer))
                                    //{
                                    using (var surface = backBuffer.QueryInterface<Surface>())
                                    {
                                        using (var d2D1RenderTarget = new RenderTarget(d2D1Factory, surface,
                                            new RenderTargetProperties(new PixelFormat(Format.Unknown, AlphaMode.Premultiplied))))
                                        {
                                            var stopWatch = new Stopwatch();
                                            var framesCount = 0;
                                            var totalTime = TimeSpan.Zero;
                                            var oneSecondElapsed = TimeSpan.Zero;
                                            var oneSecondFramesCount = 0;
                                            var fps = 0;

                                            var d2D1Bitmap =
                                                LoadBitmap(@"C:\Users\Dawid Komorowski\Documents\GitRepos\geisha\Geisha\Geisha.TestGame\Assets\box.jpg",
                                                    d2D1RenderTarget);

                                            var transformers = Enumerable.Range(0, 500).Select(i => GetRandomTransformTransformer()).ToList();


                                            RenderLoop.Run(form, () =>
                                            {
                                                var elapsed = stopWatch.Elapsed;
                                                stopWatch.Restart();

                                                framesCount++;
                                                totalTime += elapsed;
                                                oneSecondElapsed += elapsed;
                                                oneSecondFramesCount++;

                                                d2D1RenderTarget.BeginDraw();
                                                d2D1RenderTarget.Clear(new RawColor4(0, 0, 0, 0));
                                                d2D1RenderTarget.Transform = new RawMatrix3x2(1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f);

                                                foreach (var transformer in transformers)
                                                {
                                                    var transform = transformer(totalTime);
                                                    DrawBitmap(d2D1Bitmap, transform.Create2DTransformationMatrix(), d2D1RenderTarget);
                                                }

                                                DrawDiagnostics(framesCount, fps, d2D1RenderTarget);

                                                d2D1RenderTarget.EndDraw();

                                                dxgiSwapChain.Present(0, PresentFlags.None);


                                                if (oneSecondElapsed.TotalSeconds > 1)
                                                {
                                                    oneSecondElapsed = TimeSpan.Zero;
                                                    fps = oneSecondFramesCount;
                                                    oneSecondFramesCount = 0;
                                                }
                                            });
                                        }
                                    }

                                    //}
                                }
                            }
                        }

                        d3D11Device.ImmediateContext.ClearState();
                        d3D11Device.ImmediateContext.Flush();
                    }
                }
            }
        }

        private static Bitmap LoadBitmap(string filePath, RenderTarget d2D1RenderTarget)
        {
            Bitmap d2D1Bitmap;

            using (var gdiBitmap = new System.Drawing.Bitmap(filePath))
            {
                var gdiBitmapData = gdiBitmap.LockBits(new Rectangle(0, 0, gdiBitmap.Width, gdiBitmap.Height), ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                var stride = Math.Abs(gdiBitmapData.Stride);
                using (var convertedBitmapDataStream = new DataStream(gdiBitmap.Height * stride, true, true))
                {
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

                    d2D1Bitmap = new Bitmap(d2D1RenderTarget, new Size2(gdiBitmap.Width, gdiBitmap.Height), convertedBitmapDataStream, stride,
                        new BitmapProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)));
                }

                gdiBitmap.UnlockBits(gdiBitmapData);
            }

            return d2D1Bitmap;
        }

        private static void DrawBitmap(Bitmap d2D1Bitmap, Matrix3 transform, RenderTarget d2D1RenderTarget)
        {
            var bitmapSize = d2D1Bitmap.PixelSize;

            var final =
                Matrix3.Translation(new Vector2(640, 360)) * // Set to center of screen
                new Matrix3(
                    transform.M11, -transform.M12, transform.M13,
                    -transform.M21, transform.M22, -transform.M23,
                    transform.M31, transform.M32, transform.M33
                ) * // Convert coordinates system
                Matrix3.Translation(new Vector2(-bitmapSize.Width / 2d, -bitmapSize.Height / 2d)) * // Set center of square to (0,0)
                Matrix3.Identity;

            var geishaMatrix = new RawMatrix3x2(
                (float) final.M11, (float) final.M21,
                (float) final.M12, (float) final.M22,
                (float) final.M13, (float) final.M23);
            d2D1RenderTarget.Transform = geishaMatrix;

            d2D1RenderTarget.DrawBitmap(d2D1Bitmap, 1.0f, BitmapInterpolationMode.Linear);
        }

        private static void DrawDiagnostics(int framesCount, int fps, RenderTarget d2D1RenderTarget)
        {
            d2D1RenderTarget.Transform = new RawMatrix3x2(1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f);

            using (var d2D1SolidColorBrush = new SolidColorBrush(d2D1RenderTarget, new RawColor4(0, 1, 0, 1)))
            {
                using (var dwFactory = new SharpDX.DirectWrite.Factory(FactoryType.Shared))
                {
                    using (var textFormat = new TextFormat(dwFactory, "Arial", FontWeight.Regular, FontStyle.Normal,
                        16.0f))
                    {
                        d2D1RenderTarget.DrawText($"TotalFrames: {framesCount}", textFormat,
                            new RawRectangleF(10, 10, 500, 500), d2D1SolidColorBrush);
                        d2D1RenderTarget.DrawText($"FPS: {fps}", textFormat,
                            new RawRectangleF(10, 26, 500, 500), d2D1SolidColorBrush);
                    }
                }
            }
        }

        private static Transform GetRandomTransform()
        {
            return new Transform
            {
                Translation = new Vector3(Random.Next(-640, 640), Random.Next(-360, 360), 0),
                Rotation = new Vector3(0, 0, Random.NextDouble() * Math.PI * 2),
                Scale = new Vector3(Random.NextDouble() * 0.02 + 0.01, Random.NextDouble() * 0.02 + 0.01, 0)
            };
        }

        private static Func<TimeSpan, Transform> GetRandomTransformTransformer()
        {
            var baseTransform = GetRandomTransform();
            var randomTimeOffset = TimeSpan.FromSeconds(Random.NextDouble() * 5);

            return time =>
            {
                var actualSeconds = (time + randomTimeOffset).TotalSeconds;

                var transformedTransform = new Transform
                {
                    Translation =
                        (baseTransform.Translation.ToVector2() + new Vector2(Math.Sin(actualSeconds) * 500, Math.Sin(actualSeconds) * 500)).ToVector3(),
                    Rotation = baseTransform.Rotation + new Vector3(0, 0, actualSeconds),
                    Scale = baseTransform.Scale + new Vector3((Math.Sin(actualSeconds)*0.01 + 0.01) / 2, (Math.Sin(actualSeconds)*0.01 + 0.01) / 2, 1)
                };

                return transformedTransform;
            };
        }
    }
}