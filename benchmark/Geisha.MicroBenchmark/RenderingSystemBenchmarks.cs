using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Rendering.Diagnostics;
using Geisha.Engine.Rendering.Systems;
using Geisha.TestUtils;

namespace Geisha.MicroBenchmark;

[MemoryDiagnoser]
public class RenderingSystemBenchmarks
{
    private Scene _scene = null!;
    private RenderingSystem _renderingSystem = null!;
    private StubRenderingBackend _stubRenderingBackend = new();

    private void InitializeRenderingSystem()
    {
        var renderingConfiguration = new RenderingConfiguration();
        var aggregatedDiagnosticInfoProvider = new AggregatedDiagnosticInfoProvider();
        aggregatedDiagnosticInfoProvider.Initialize(Enumerable.Empty<IDiagnosticInfoProvider>());
        var debugRenderer = new DebugRenderer();
        var renderingDiagnosticInfoProvider = new RenderingDiagnosticInfoProvider(renderingConfiguration);

        _renderingSystem = new RenderingSystem(
            _stubRenderingBackend,
            renderingConfiguration,
            aggregatedDiagnosticInfoProvider,
            debugRenderer,
            renderingDiagnosticInfoProvider
        );

        _scene = TestSceneFactory.Create();
        _scene.AddObserver(_renderingSystem);
    }

    private void CleanupRenderingSystem()
    {
        _scene.RemoveObserver(_renderingSystem);
        _scene = null!;
        _renderingSystem = null!;
    }

    [IterationSetup]
    public void IterationSetup()
    {
        InitializeRenderingSystem();
        //CreateAnimations();
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
        CleanupRenderingSystem();
    }

    [Benchmark]
    public void RenderScene_10_Seconds_10_000_StaticSprites()
    {
        // Assuming 60FPS it simulates 10s.
        for (var i = 0; i < 600; i++)
        {
            _renderingSystem.RenderScene();
        }
    }

    private sealed class StubRenderingBackend : IRenderingBackend
    {
        public IRenderingContext2D Context2D { get; } = new StubRenderingContext2D();
        public RenderingStatistics Statistics { get; }
        public RenderingBackendInfo Info { get; }
        public bool VSyncEnabled { get; set; }
        public BufferingMode BufferingMode { get; set; }

        public void WaitForFramePacing()
        {
            throw new System.NotImplementedException();
        }

        public void Present()
        {
        }

        public void ResizeBuffers(Size size)
        {
            throw new System.NotImplementedException();
        }
    }

    private sealed class StubRenderingContext2D : IRenderingContext2D
    {
        public Size RenderTargetSize { get; }
        public ITexture CreateTexture(Stream stream) => throw new System.NotImplementedException();

        public ITextLayout CreateTextLayout(string text, string fontFamilyName, FontSize fontSize, double maxWidth, double maxHeight) =>
            new StubTextLayout();

        public void CaptureScreenshotAsPng(Stream stream)
        {
            throw new System.NotImplementedException();
        }

        public void BeginDraw()
        {
        }

        public void EndDraw()
        {
        }

        public void Clear(Color color)
        {
        }

        public void DrawSprite(Sprite sprite, in Matrix3x3 transform, double opacity = 1,
            BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.Linear)
        {
            throw new System.NotImplementedException();
        }

        public void DrawSpriteBatch(SpriteBatch spriteBatch)
        {
            throw new System.NotImplementedException();
        }

        public void DrawText(string text, string fontFamilyName, FontSize fontSize, Color color, in Matrix3x3 transform)
        {
            throw new System.NotImplementedException();
        }

        public void DrawTextLayout(ITextLayout textLayout, Color color, in Vector2 pivot, in Matrix3x3 transform, bool clipToLayoutBox = false)
        {
            throw new System.NotImplementedException();
        }

        public void DrawRectangle(in AxisAlignedRectangle rectangle, Color color, bool fillInterior, in Matrix3x3 transform)
        {
            throw new System.NotImplementedException();
        }

        public void DrawEllipse(in Ellipse ellipse, Color color, bool fillInterior, in Matrix3x3 transform)
        {
            throw new System.NotImplementedException();
        }

        public void SetClippingRectangle(in AxisAlignedRectangle clippingRectangle)
        {
            throw new System.NotImplementedException();
        }

        public void ClearClipping()
        {
            throw new System.NotImplementedException();
        }
    }

    private sealed class StubTextLayout : ITextLayout
    {
        public void Dispose()
        {
        }

        public string Text { get; }
        public string FontFamilyName { get; set; }
        public FontSize FontSize { get; set; }
        public double MaxWidth { get; set; }
        public double MaxHeight { get; set; }
        public TextAlignment TextAlignment { get; set; }
        public ParagraphAlignment ParagraphAlignment { get; set; }
        public TextMetrics Metrics { get; }
    }
}