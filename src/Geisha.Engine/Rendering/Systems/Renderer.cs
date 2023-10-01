using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.Rendering.Diagnostics;
using NLog;

namespace Geisha.Engine.Rendering.Systems
{
    internal sealed class Renderer : IRenderNodeVisitor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IRenderingBackend _renderingBackend;
        private readonly IRenderingContext2D _renderingContext2D;
        private readonly IAggregatedDiagnosticInfoProvider _aggregatedDiagnosticInfoProvider;
        private readonly IDebugRendererForRenderingSystem _debugRendererForRenderingSystem;
        private readonly IRenderingDiagnosticInfoProvider _renderingDiagnosticInfoProvider;
        private readonly List<string> _sortingLayersOrder;
        private readonly RenderingState _renderingState;
        private readonly List<RenderNode> _renderList;

        private Matrix3x3 _cameraTransformationMatrix;

        public Renderer(IRenderingBackend renderingBackend, RenderingConfiguration renderingConfiguration,
            IAggregatedDiagnosticInfoProvider aggregatedDiagnosticInfoProvider, IDebugRendererForRenderingSystem debugRendererForRenderingSystem,
            IRenderingDiagnosticInfoProvider renderingDiagnosticInfoProvider, RenderingState renderingState)
        {
            _renderingBackend = renderingBackend;
            _renderingContext2D = renderingBackend.Context2D;
            _aggregatedDiagnosticInfoProvider = aggregatedDiagnosticInfoProvider;
            _debugRendererForRenderingSystem = debugRendererForRenderingSystem;
            _renderingDiagnosticInfoProvider = renderingDiagnosticInfoProvider;
            _renderingState = renderingState;

            _sortingLayersOrder = renderingConfiguration.SortingLayersOrder.ToList();
            _renderList = new List<RenderNode>();
        }

        public void RenderScene()
        {
            _renderingContext2D.BeginDraw();

            _renderingContext2D.Clear(Color.White);

            if (_renderingState.CameraNode != null)
            {
                var cameraNode = _renderingState.CameraNode;
                cameraNode.ScreenWidth = _renderingContext2D.ScreenWidth;
                cameraNode.ScreenHeight = _renderingContext2D.ScreenHeight;
                _cameraTransformationMatrix = _renderingState.CameraNode.CreateViewMatrix();

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
            var transformationMatrix = TransformHierarchy.Calculate2DTransformationMatrix(node.Entity);
            transformationMatrix = _cameraTransformationMatrix * transformationMatrix;

            var ellipse = new Ellipse(node.RadiusX, node.RadiusY);
            _renderingContext2D.DrawEllipse(ellipse, node.Color, node.FillInterior, transformationMatrix);
        }

        public void Visit(RectangleNode node)
        {
            var transformationMatrix = TransformHierarchy.Calculate2DTransformationMatrix(node.Entity);
            transformationMatrix = _cameraTransformationMatrix * transformationMatrix;

            var rectangle = new AxisAlignedRectangle(node.Dimensions);
            _renderingContext2D.DrawRectangle(rectangle, node.Color, node.FillInterior, transformationMatrix);
        }

        public void Visit(SpriteNode node)
        {
            if (node.Sprite != null)
            {
                var transformationMatrix = TransformHierarchy.Calculate2DTransformationMatrix(node.Entity);
                transformationMatrix = _cameraTransformationMatrix * transformationMatrix;
                _renderingContext2D.DrawSprite(node.Sprite, transformationMatrix, node.Opacity);
            }
        }

        public void Visit(TextNode node)
        {
            var transformationMatrix = TransformHierarchy.Calculate2DTransformationMatrix(node.Entity);
            transformationMatrix = _cameraTransformationMatrix * transformationMatrix;

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

            foreach (var renderNode in _renderingState.GetRenderNodes())
            {
                if (renderNode.ShouldSkipRendering()) continue;
                if (!boundingRectangleOfView.Overlaps(renderNode.GetBoundingRectangle())) continue;

                _renderList.Add(renderNode);
            }

            _renderList.Sort((renderNode1, renderNode2) =>
            {
                var r1 = renderNode1.Renderer2DComponent;
                var r2 = renderNode2.Renderer2DComponent;

                var layersComparison = _sortingLayersOrder.IndexOf(r1.SortingLayerName) - _sortingLayersOrder.IndexOf(r2.SortingLayerName);

                return layersComparison == 0 ? r1.OrderInLayer - r2.OrderInLayer : layersComparison;
            });
        }

        private void RenderNodes()
        {
            foreach (var renderNode in _renderList)
            {
                renderNode.Accept(this);
            }
        }

        private void RenderDiagnosticInfo()
        {
            var width = _renderingContext2D.ScreenWidth;
            var height = _renderingContext2D.ScreenHeight;
            var color = Color.Green;

            var translation = new Vector2(-(width / 2d) + 1, height / 2d - 1);

            foreach (var diagnosticInfo in _aggregatedDiagnosticInfoProvider.GetAllDiagnosticInfo())
            {
                _renderingContext2D.DrawText(diagnosticInfo.ToString(), "Consolas", FontSize.FromDips(14), color, Matrix3x3.CreateTranslation(translation));
                translation -= new Vector2(0, 14);
            }
        }
    }
}