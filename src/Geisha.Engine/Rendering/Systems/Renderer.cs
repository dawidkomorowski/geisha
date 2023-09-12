using System.Collections.Generic;
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
                var cameraComponent = _renderingState.CameraNode.Camera;
                cameraComponent.ScreenWidth = _renderingContext2D.ScreenWidth;
                cameraComponent.ScreenHeight = _renderingContext2D.ScreenHeight;
                _cameraTransformationMatrix = _renderingState.CameraNode.Entity.Create2DWorldToScreenMatrix();

                EnableAspectRatio(cameraComponent);
                UpdateRenderList();
                RenderNodes();

                _debugRendererForRenderingSystem.DrawDebugInformation(_renderingContext2D, _cameraTransformationMatrix);

                DisableAspectRatio(cameraComponent);
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

            var ellipseRendererComponent = node.EllipseRendererComponent;

            var ellipse = new Ellipse(ellipseRendererComponent.RadiusX, ellipseRendererComponent.RadiusY);
            _renderingContext2D.DrawEllipse(ellipse, ellipseRendererComponent.Color, ellipseRendererComponent.FillInterior, transformationMatrix);
        }

        public void Visit(RectangleNode node)
        {
            var transformationMatrix = TransformHierarchy.Calculate2DTransformationMatrix(node.Entity);
            transformationMatrix = _cameraTransformationMatrix * transformationMatrix;

            var rectangleRendererComponent = node.RectangleRendererComponent;

            var rectangle = new AxisAlignedRectangle(rectangleRendererComponent.Dimension);
            _renderingContext2D.DrawRectangle(
                rectangle,
                rectangleRendererComponent.Color,
                rectangleRendererComponent.FillInterior,
                transformationMatrix
            );
        }

        public void Visit(SpriteNode node)
        {
            var transformationMatrix = TransformHierarchy.Calculate2DTransformationMatrix(node.Entity);
            transformationMatrix = _cameraTransformationMatrix * transformationMatrix;

            var spriteRendererComponent = node.SpriteRendererComponent;

            var sprite = spriteRendererComponent.Sprite;
            if (sprite != null)
            {
                _renderingContext2D.DrawSprite(sprite, transformationMatrix, spriteRendererComponent.Opacity);
            }
        }

        public void Visit(TextNode node)
        {
            var transformationMatrix = TransformHierarchy.Calculate2DTransformationMatrix(node.Entity);
            transformationMatrix = _cameraTransformationMatrix * transformationMatrix;

            _renderingContext2D.DrawTextLayout(node.TextLayout, node.Color, node.Pivot, transformationMatrix, node.ClipToLayoutBox);
        }

        #endregion

        private void EnableAspectRatio(CameraComponent cameraComponent)
        {
            if (cameraComponent.AspectRatioBehavior == AspectRatioBehavior.Underscan)
            {
                _renderingContext2D.Clear(Color.Black);

                var clipDimension = ComputeClipDimensions(cameraComponent);
                var clippingRectangle = new AxisAlignedRectangle(clipDimension);
                _renderingContext2D.SetClippingRectangle(clippingRectangle);

                _renderingContext2D.Clear(Color.White);
            }
        }

        private void DisableAspectRatio(CameraComponent cameraComponent)
        {
            if (cameraComponent.AspectRatioBehavior == AspectRatioBehavior.Underscan)
            {
                _renderingContext2D.ClearClipping();
            }
        }

        private void UpdateRenderList()
        {
            _renderList.Clear();
            foreach (var renderNode in _renderingState.GetRenderNodes())
            {
                if (renderNode.Renderer2DComponent.Visible) _renderList.Add(renderNode);
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

        private static Vector2 ComputeClipDimensions(CameraComponent cameraComponent)
        {
            if (cameraComponent.CameraIsWiderThanScreen())
            {
                var scaleFactor = cameraComponent.ScreenWidth / cameraComponent.ViewRectangle.X;
                return new Vector2(cameraComponent.ScreenWidth, cameraComponent.ViewRectangle.Y * scaleFactor);
            }
            else
            {
                var scaleFactor = cameraComponent.ScreenHeight / cameraComponent.ViewRectangle.Y;
                return new Vector2(cameraComponent.ViewRectangle.X * scaleFactor, cameraComponent.ScreenHeight);
            }
        }
    }
}