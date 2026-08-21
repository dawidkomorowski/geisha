using System.Collections.Generic;
using System.Diagnostics;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.Rendering.Diagnostics;
using NLog;

namespace Geisha.Engine.Rendering.Systems;

internal sealed class Renderer : IRenderNodeVisitor
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly IRenderingBackend _renderingBackend;
    private readonly IRenderingContext2D _renderingContext2D;
    private readonly IAggregatedDiagnosticInfoProvider _aggregatedDiagnosticInfoProvider;
    private readonly IDebugRendererForRenderingSystem _debugRendererForRenderingSystem;
    private readonly IRenderingDiagnosticInfoProvider _renderingDiagnosticInfoProvider;
    private readonly RenderingState _renderingState;
    private readonly List<RenderNode> _renderList = new();
    private readonly List<RenderNode> _sortingBuffer = new();
    private readonly SpriteBatch _spriteBatch = new();

    private Matrix3x3 _cameraTransformationMatrix;

    public Renderer(
        IRenderingBackend renderingBackend,
        IAggregatedDiagnosticInfoProvider aggregatedDiagnosticInfoProvider,
        IDebugRendererForRenderingSystem debugRendererForRenderingSystem,
        IRenderingDiagnosticInfoProvider renderingDiagnosticInfoProvider,
        RenderingState renderingState
    )
    {
        _renderingBackend = renderingBackend;
        _renderingContext2D = renderingBackend.Context2D;
        _aggregatedDiagnosticInfoProvider = aggregatedDiagnosticInfoProvider;
        _debugRendererForRenderingSystem = debugRendererForRenderingSystem;
        _renderingDiagnosticInfoProvider = renderingDiagnosticInfoProvider;
        _renderingState = renderingState;
    }

    public void RenderScene()
    {
        _renderingContext2D.BeginDraw();

        _renderingContext2D.Clear(Color.White);

        if (_renderingState.CameraNode != null)
        {
            var cameraNode = _renderingState.CameraNode;
            cameraNode.ScreenSize = _renderingContext2D.ScreenSize;
            _cameraTransformationMatrix = _renderingState.CameraNode.CreateViewMatrixScaledToScreen();

            EnableAspectRatio(cameraNode);
            UpdateRenderList();
            RenderNodes();

            _debugRendererForRenderingSystem.DrawDebugInformation(_renderingContext2D, _cameraTransformationMatrix);

            DisableAspectRatio(cameraNode);
        }
        else
        {
            Logger.Warn("No camera component found in scene.");
        }

        _renderingDiagnosticInfoProvider.UpdateDiagnostics(_renderingBackend.Statistics);

        RenderDiagnosticInfo();

        _renderingContext2D.EndDraw();
    }

    #region Implementation of IRenderNodeVisitor

    public void Visit(RenderNode node)
    {
    }

    public void Visit(EllipseNode node)
    {
        FlushSpriteBatch();

        var transformationMatrix = ComputeNodeTransform(node);

        var ellipse = new Ellipse(node.RadiusX, node.RadiusY);
        _renderingContext2D.DrawEllipse(ellipse, node.Color, node.FillInterior, transformationMatrix);
    }

    public void Visit(RectangleNode node)
    {
        FlushSpriteBatch();

        var transformationMatrix = ComputeNodeTransform(node);

        var rectangle = new AxisAlignedRectangle(node.Dimensions);
        _renderingContext2D.DrawRectangle(rectangle, node.Color, node.FillInterior, transformationMatrix);
    }

    public void Visit(SpriteNode node)
    {
        if (node.Sprite == null) return;

        var transformationMatrix = ComputeNodeTransform(node);

        if (!_spriteBatch.IsEmpty)
        {
            var flushRequired = false;

            flushRequired = flushRequired || !ReferenceEquals(_spriteBatch.Texture, node.Sprite.SourceTexture);
            flushRequired = flushRequired || _spriteBatch.BitmapInterpolationMode != node.BitmapInterpolationMode;

            if (flushRequired)
            {
                FlushSpriteBatch();
            }
        }

        _spriteBatch.AddSprite(node.Sprite, transformationMatrix, node.Opacity);
        _spriteBatch.BitmapInterpolationMode = node.BitmapInterpolationMode;
    }

    public void Visit(TextNode node)
    {
        FlushSpriteBatch();

        var transformationMatrix = ComputeNodeTransform(node);

        _renderingContext2D.DrawTextLayout(node.TextLayout, node.Color, node.Pivot, transformationMatrix, node.ClipToLayoutBox);
    }

    #endregion

    private void EnableAspectRatio(CameraNode cameraNode)
    {
        if (cameraNode.AspectRatioBehavior == AspectRatioBehavior.Underscan)
        {
            _renderingContext2D.Clear(Color.Black);
            _renderingContext2D.SetClippingRectangle(cameraNode.GetClippingRectangle());
            _renderingContext2D.Clear(Color.White);
        }
    }

    private void DisableAspectRatio(CameraNode cameraNode)
    {
        if (cameraNode.AspectRatioBehavior == AspectRatioBehavior.Underscan)
        {
            _renderingContext2D.ClearClipping();
        }
    }

    private void UpdateRenderList()
    {
        _renderList.Clear();

        Debug.Assert(_renderingState.CameraNode != null, "_renderingState.CameraNode != null");
        var boundingRectangleOfView = _renderingState.CameraNode.GetBoundingRectangleOfView();

        foreach (var sortingLayer in _renderingState.GetSortingLayersSpan())
        {
            _sortingBuffer.Clear();
            foreach (var renderNode in sortingLayer.GetRenderNodesSpan())
            {
                if (renderNode.ShouldSkipRendering()) continue;
                if (!boundingRectangleOfView.Overlaps(renderNode.GetBoundingRectangle())) continue;

                _sortingBuffer.Add(renderNode);
            }

            _sortingBuffer.Sort((renderNode1, renderNode2) =>
            {
                var orderInLayerComparison = renderNode1.OrderInLayer.CompareTo(renderNode2.OrderInLayer);
                return orderInLayerComparison != 0 ? orderInLayerComparison : renderNode1.BatchId.CompareTo(renderNode2.BatchId);
            });

            _renderList.AddRange(_sortingBuffer);
        }
    }

    private void RenderNodes()
    {
        foreach (var renderNode in _renderList)
        {
            renderNode.Accept(this);
        }

        FlushSpriteBatch();
    }

    private void RenderDiagnosticInfo()
    {
        var screenSize = _renderingContext2D.ScreenSize;
        var translation = new Vector2(-(screenSize.Width / 2d) + 3, screenSize.Height / 2d - 3);

        foreach (var diagnosticInfo in _aggregatedDiagnosticInfoProvider.GetAllDiagnosticInfo())
        {
            // TODO: How to measure text?
            var text = diagnosticInfo.ToString();
            var rectSize = new Vector2(text.Length * 8, 16);
            var rectCenter = translation + new Vector2(rectSize.X * 0.5, -rectSize.Y * 0.5);

            _renderingContext2D.DrawRectangle(new AxisAlignedRectangle(rectCenter, rectSize), Color.Green, true, Matrix3x3.Identity);
            _renderingContext2D.DrawText(text, "Consolas", FontSize.FromDips(14), Color.White, Matrix3x3.CreateTranslation(translation + new Vector2(2, 0)));

            translation -= new Vector2(0, 20);
        }
    }

    private void FlushSpriteBatch()
    {
        if (_spriteBatch.IsEmpty) return;

        if (_spriteBatch.Count == 1)
        {
            var sprites = _spriteBatch.GetSpritesSpan();
            _renderingContext2D.DrawSprite(sprites[0].Sprite, sprites[0].Transform, sprites[0].Opacity, _spriteBatch.BitmapInterpolationMode);
        }
        else
        {
            _renderingContext2D.DrawSpriteBatch(_spriteBatch);
        }

        _spriteBatch.Clear();
    }

    private Matrix3x3 ComputeNodeTransform(RenderNode node)
    {
        return _cameraTransformationMatrix * node.Transform.ComputeInterpolatedWorldTransformMatrix();
    }
}