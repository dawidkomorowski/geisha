using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Geisha.Common.Math;
using Geisha.Framework.Rendering;
using Geisha.Framework.Rendering.DirectX;
using SharpDX.Windows;
using Color = Geisha.Framework.Rendering.Color;
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

                                renderer2D.RenderText($"TotalFrames: {framesCount}", FontSize.FromPoints(12), Color.FromArgb(255, 0, 255, 0),
                                    new Transform {Translation = new Vector3(0, 0, 0), Rotation = Vector3.Zero, Scale = Vector3.One}
                                        .Create2DTransformationMatrix());
                                renderer2D.RenderText($"FPS: {fps}", FontSize.FromPoints(12), Color.FromArgb(255, 0, 255, 0),
                                    new Transform {Translation = new Vector3(0, -FontSize.FromPoints(12).Dips, 0), Rotation = Vector3.Zero, Scale = Vector3.One}
                                        .Create2DTransformationMatrix());

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