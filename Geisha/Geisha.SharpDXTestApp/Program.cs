using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Geisha.Common.Math;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.Windows;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Bitmap = SharpDX.Direct2D1.Bitmap;
using Device = SharpDX.Direct3D11.Device;
using Factory = SharpDX.Direct2D1.Factory;
using FeatureLevel = SharpDX.Direct3D.FeatureLevel;
using PixelFormat = SharpDX.Direct2D1.PixelFormat;
using Rectangle = System.Drawing.Rectangle;
using Resource = SharpDX.Direct3D11.Resource;

namespace Geisha.SharpDXTestApp
{
    internal static class Program
    {
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

                                            var d2D1Bitmap =
                                                LoadBitmap(@"C:\Users\Dawid Komorowski\Documents\GitRepos\geisha\Geisha\Geisha.TestGame\Assets\box.jpg",
                                                    d2D1RenderTarget);

                                            RenderLoop.Run(form, () =>
                                            {
                                                using (var brush = new SolidColorBrush(d2D1RenderTarget, new RawColor4(1, 0, 0, 1)))
                                                {
                                                    var elapsed = stopWatch.Elapsed;
                                                    stopWatch.Restart();

                                                    d2D1RenderTarget.BeginDraw();
                                                    d2D1RenderTarget.Clear(new RawColor4(0, 0, 0, 0));
                                                    d2D1RenderTarget.Transform = new RawMatrix3x2(1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f);

                                                    var modelMatrix = Matrix3.Translation(new Vector2(0, 100)) * // Model transform
                                                                      Matrix3.Rotation(totalTime.TotalSeconds) * // Model transform
                                                                      Matrix3.Scale(new Vector2(0.5, 0.1)) * // Model transform
                                                                      Matrix3.Identity;

                                                    var final =
                                                        Matrix3.Translation(new Vector2(640, 360)) * // Set to center of screen
                                                        new Matrix3(
                                                            modelMatrix.M11, -modelMatrix.M12, modelMatrix.M13,
                                                            -modelMatrix.M21, modelMatrix.M22, -modelMatrix.M23,
                                                            modelMatrix.M31, modelMatrix.M32, modelMatrix.M33
                                                        ) *
                                                        Matrix3.Translation(new Vector2(-256, -256)) * // Set center of square to (0,0)
                                                        Matrix3.Identity;

                                                    var geishaMatrix = new RawMatrix3x2(
                                                        (float) final.M11, (float) final.M21,
                                                        (float) final.M12, (float) final.M22,
                                                        (float) final.M13, (float) final.M23);
                                                    d2D1RenderTarget.Transform = geishaMatrix;

                                                    d2D1RenderTarget.DrawBitmap(d2D1Bitmap, 1.0f, BitmapInterpolationMode.Linear);


                                                    var global = Matrix3.Identity;
                                                    geishaMatrix = new RawMatrix3x2(
                                                        (float) global.M11, (float) global.M12,
                                                        (float) global.M21, (float) global.M22,
                                                        (float) global.M13, (float) global.M23);
                                                    d2D1RenderTarget.Transform = geishaMatrix;

                                                    var custom = Matrix3.Identity;
                                                    custom = Matrix3.Translation(new Vector2(640, -360)) * // Set to center of screen
                                                             Matrix3.Translation(new Vector2(200, -100)) * // Model transform
                                                             Matrix3.Rotation(totalTime.TotalSeconds) * // Model transform
                                                             Matrix3.Scale(new Vector2(0.5, 0.1)) * // Model transform
                                                             Matrix3.Translation(new Vector2(-256, -256)) * // Set center of square to (0,0)
                                                             Matrix3.Identity;

                                                    var topLeft = custom * new Vector2(0, 0).Homogeneous;
                                                    var topRight = custom * new Vector2(512, 0).Homogeneous;
                                                    var bottomLeft = custom * new Vector2(0, 512).Homogeneous;
                                                    var bottomRight = custom * new Vector2(512, 512).Homogeneous;

                                                    d2D1RenderTarget.DrawLine(new RawVector2((float) topLeft.X, (float) -topLeft.Y),
                                                        new RawVector2((float) topRight.X, (float) -topRight.Y), brush);
                                                    d2D1RenderTarget.DrawLine(new RawVector2((float) topRight.X, (float) -topRight.Y),
                                                        new RawVector2((float) bottomRight.X, (float) -bottomRight.Y), brush);
                                                    d2D1RenderTarget.DrawLine(new RawVector2((float) bottomRight.X, (float) -bottomRight.Y),
                                                        new RawVector2((float) bottomLeft.X, (float) -bottomLeft.Y), brush);
                                                    d2D1RenderTarget.DrawLine(new RawVector2((float) bottomLeft.X, (float) -bottomLeft.Y),
                                                        new RawVector2((float) topLeft.X, (float) -topLeft.Y), brush);

                                                    d2D1RenderTarget.EndDraw();

                                                    dxgiSwapChain.Present(0, PresentFlags.None);

                                                    totalTime += elapsed;
                                                    framesCount++;
                                                    if (framesCount % 1000 == 0)
                                                    {
                                                        Debug.WriteLine($"TotalFrames: {framesCount}");
                                                        Debug.WriteLine($"FPS: {1000 / elapsed.TotalMilliseconds}");
                                                    }
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

        private static Bitmap LoadBitmap(string filePath, RenderTarget d2DRenderTarget)
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

                    d2D1Bitmap = new Bitmap(d2DRenderTarget, new Size2(gdiBitmap.Width, gdiBitmap.Height), convertedBitmapDataStream, stride,
                        new BitmapProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)));
                }

                gdiBitmap.UnlockBits(gdiBitmapData);
            }

            return d2D1Bitmap;
        }
    }
}