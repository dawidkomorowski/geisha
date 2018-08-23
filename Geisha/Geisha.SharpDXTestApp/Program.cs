using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Geisha.Common.Math;
using Geisha.Framework.Rendering;
using Geisha.Framework.Rendering.DirectX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;
using SharpDX.Windows;
using Color = Geisha.Framework.Rendering.Color;
using Factory = SharpDX.DirectWrite.Factory;
using FactoryType = SharpDX.DirectWrite.FactoryType;
using FontStyle = SharpDX.DirectWrite.FontStyle;
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
                var window = new OutputWindow(form);
                var windowProvider = new OutputWindowProvider(window);
                using (var renderer2D = new Renderer2D(windowProvider))
                {
                    using (var fileStream = new FileStream(@"C:\Users\Dawid Komorowski\Documents\GitRepos\geisha\Geisha\Geisha.TestGame\Assets\box.jpg",
                        FileMode.Open))
                    {
                        using (var texture = renderer2D.CreateTexture(fileStream))
                        {
                            var sprite = new Sprite
                            {
                                SourceUV = Vector2.Zero,
                                SourceDimension = texture.Dimension,
                                SourceTexture = texture,
                                PixelsPerUnit = 1,
                                SourceAnchor = texture.Dimension / 2
                            };

                            var stopWatch = new Stopwatch();
                            var framesCount = 0;
                            var totalTime = TimeSpan.Zero;
                            var oneSecondElapsed = TimeSpan.Zero;
                            var oneSecondFramesCount = 0;
                            var fps = 0;

                            var transformers = Enumerable.Range(0, 500).Select(i => GetRandomTransformTransformer()).ToList();

                            RenderLoop.Run(form, () =>
                            {
                                var elapsed = stopWatch.Elapsed;
                                stopWatch.Restart();

                                framesCount++;
                                totalTime += elapsed;
                                oneSecondElapsed += elapsed;
                                oneSecondFramesCount++;

                                renderer2D.BeginRendering();

                                renderer2D.Clear(Color.FromArgb(255, 0, 0, 0));

                                foreach (var transformer in transformers)
                                {
                                    var transform = transformer(totalTime);
                                    renderer2D.RenderSprite(sprite, transform.Create2DTransformationMatrix());
                                }


                                renderer2D.EndRendering();

                                if (oneSecondElapsed.TotalSeconds > 1)
                                {
                                    oneSecondElapsed = TimeSpan.Zero;
                                    fps = oneSecondFramesCount;
                                    oneSecondFramesCount = 0;
                                }
                            });
                        }
                    }
                }


                //RenderLoop.Run(form, () => { DrawDiagnostics(framesCount, fps, d2D1RenderTarget); });
            }
        }

        private static void DrawDiagnostics(int framesCount, int fps, RenderTarget d2D1RenderTarget)
        {
            d2D1RenderTarget.Transform = new RawMatrix3x2(1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f);

            using (var d2D1SolidColorBrush = new SolidColorBrush(d2D1RenderTarget, new RawColor4(0, 1, 0, 1)))
            {
                using (var dwFactory = new Factory(FactoryType.Shared))
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
                    Scale = baseTransform.Scale + new Vector3((Math.Sin(actualSeconds) * 0.01 + 0.01) / 2, (Math.Sin(actualSeconds) * 0.01 + 0.01) / 2, 1)
                };

                return transformedTransform;
            };
        }

        private sealed class OutputWindow : IWindow
        {
            private readonly RenderForm _renderForm;

            public OutputWindow(RenderForm renderForm)
            {
                _renderForm = renderForm;
            }

            public int ClientAreaWidth => _renderForm.ClientSize.Width;
            public int ClientAreaHeight => _renderForm.ClientSize.Height;
            public IntPtr Handle => _renderForm.Handle;
        }

        private sealed class OutputWindowProvider : IWindowProvider
        {
            public OutputWindowProvider(IWindow window)
            {
                Window = window;
            }

            public IWindow Window { get; }
        }
    }
}