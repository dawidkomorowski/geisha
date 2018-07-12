using System;
using System.Diagnostics;
using System.Drawing;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.Windows;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Device = SharpDX.Direct3D11.Device;
using Factory = SharpDX.Direct2D1.Factory;
using FeatureLevel = SharpDX.Direct3D.FeatureLevel;
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
                    out var d3DDevice,
                    out var swapChain);

                using (swapChain)
                {
                    using (d3DDevice)
                    {
                        using (var d2DFactory = new Factory())
                        {
                            using (var dxgiFactory = swapChain.GetParent<SharpDX.DXGI.Factory>())
                            {
                                // Ignore all windows events
                                dxgiFactory.MakeWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAll);

                                using (var backBuffer = Resource.FromSwapChain<Texture2D>(swapChain, 0))
                                {
                                    using (var renderTargetView = new RenderTargetView(d3DDevice, backBuffer))
                                    {
                                        using (var surface = backBuffer.QueryInterface<Surface>())
                                        {
                                            using (var d2DRenderTarget = new RenderTarget(d2DFactory, surface,
                                                new RenderTargetProperties(new PixelFormat(Format.Unknown, AlphaMode.Premultiplied))))
                                            {
                                                var stopWatch = new Stopwatch();
                                                var framesCount = 0;
                                                var totalTime = TimeSpan.Zero;

                                                RenderLoop.Run(form, () =>
                                                {
                                                    using (var brush = new SolidColorBrush(d2DRenderTarget, new RawColor4(1, 0, 0, 1)))
                                                    {
                                                        var elapsed = stopWatch.Elapsed;
                                                        stopWatch.Restart();

                                                        d2DRenderTarget.BeginDraw();
                                                        d2DRenderTarget.Clear(new RawColor4(0, 0, 0, 0));

                                                        var x = 200 + 100 * (float) Math.Sin(totalTime.TotalSeconds);
                                                        var y = 200 + 100 * (float) Math.Cos(totalTime.TotalSeconds);
                                                        const float width = 200;
                                                        const float height = 100;
                                                        d2DRenderTarget.FillRectangle(new RawRectangleF(x, y, x + width, y + height), brush);

                                                        d2DRenderTarget.EndDraw();

                                                        swapChain.Present(0, PresentFlags.None);

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
                                    }
                                }
                            }
                        }

                        d3DDevice.ImmediateContext.ClearState();
                        d3DDevice.ImmediateContext.Flush();
                    }
                }
            }
        }
    }
}