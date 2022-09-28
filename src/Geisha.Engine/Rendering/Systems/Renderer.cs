using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.Logging;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Engine.Rendering.Systems
{
    internal sealed class Renderer
    {
        private static readonly ILog Log = LogFactory.Create(typeof(RenderingSystem));
        private readonly IRenderer2D _renderer2D;
        private readonly RenderingConfiguration _renderingConfiguration;
        private readonly IAggregatedDiagnosticInfoProvider _aggregatedDiagnosticInfoProvider;
        private readonly IDebugRendererForRenderingSystem _debugRendererForRenderingSystem;
        private readonly List<string> _sortingLayersOrder;
        private readonly RenderingState _renderingState;
        private readonly List<RenderNode> _renderList;

        public Renderer(IRenderer2D renderer2D, RenderingConfiguration renderingConfiguration,
            IAggregatedDiagnosticInfoProvider aggregatedDiagnosticInfoProvider, IDebugRendererForRenderingSystem debugRendererForRenderingSystem,
            RenderingState renderingState)
        {
            _renderer2D = renderer2D;
            _renderingConfiguration = renderingConfiguration;
            _aggregatedDiagnosticInfoProvider = aggregatedDiagnosticInfoProvider;
            _debugRendererForRenderingSystem = debugRendererForRenderingSystem;
            _renderingState = renderingState;

            _renderingConfiguration = renderingConfiguration;
            _sortingLayersOrder = _renderingConfiguration.SortingLayersOrder.ToList();
            _renderList = new List<RenderNode>();
        }

        public void RenderScene()
        {
            _renderer2D.BeginRendering();

            _renderer2D.Clear(Color.FromArgb(255, 255, 255, 255));

            if (_renderingState.CameraNode != null)
            {
                var cameraComponent = _renderingState.CameraNode.Camera;
                cameraComponent.ScreenWidth = _renderer2D.ScreenWidth;
                cameraComponent.ScreenHeight = _renderer2D.ScreenHeight;
                var cameraTransformationMatrix = _renderingState.CameraNode.Entity.Create2DWorldToScreenMatrix();

                EnableAspectRatio(cameraComponent);
                UpdateRenderList();
                RenderEntities(cameraTransformationMatrix);

                _debugRendererForRenderingSystem.DrawDebugInformation(_renderer2D, cameraTransformationMatrix);

                DisableAspectRatio(cameraComponent);
            }
            else
            {
                Log.Warn("No camera component found in scene.");
            }

            RenderDiagnosticInfo();

            _renderer2D.EndRendering(_renderingConfiguration.EnableVSync);
        }

        private void EnableAspectRatio(CameraComponent cameraComponent)
        {
            if (cameraComponent.AspectRatioBehavior == AspectRatioBehavior.Underscan)
            {
                _renderer2D.Clear(Color.FromArgb(255, 0, 0, 0));

                var clipDimension = ComputeClipDimension(cameraComponent);
                var clippingRectangle = new AxisAlignedRectangle(clipDimension);
                _renderer2D.SetClippingRectangle(clippingRectangle);

                _renderer2D.Clear(Color.FromArgb(255, 255, 255, 255));
            }
        }

        private void DisableAspectRatio(CameraComponent cameraComponent)
        {
            if (cameraComponent.AspectRatioBehavior == AspectRatioBehavior.Underscan)
            {
                _renderer2D.ClearClipping();
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

        private void RenderEntities(Matrix3x3 cameraTransformationMatrix)
        {
            foreach (var renderNode in _renderList)
            {
                var transformationMatrix = TransformHierarchy.Calculate2DTransformationMatrix(renderNode.Entity);
                transformationMatrix = cameraTransformationMatrix * transformationMatrix;

                if (renderNode.Renderer2DComponent is SpriteRendererComponent spriteRendererComponent)
                {
                    var sprite = spriteRendererComponent.Sprite;
                    if (sprite != null) _renderer2D.RenderSprite(sprite, transformationMatrix);
                }

                if (renderNode.Renderer2DComponent is TextRendererComponent textRendererComponent)
                {
                    _renderer2D.RenderText(textRendererComponent.Text, textRendererComponent.FontSize, textRendererComponent.Color, transformationMatrix);
                }

                if (renderNode.Renderer2DComponent is RectangleRendererComponent rectangleRendererComponent)
                {
                    var rectangle = new AxisAlignedRectangle(rectangleRendererComponent.Dimension);
                    _renderer2D.RenderRectangle(rectangle, rectangleRendererComponent.Color, rectangleRendererComponent.FillInterior, transformationMatrix);
                }

                if (renderNode.Renderer2DComponent is EllipseRendererComponent ellipseRendererComponent)
                {
                    var ellipse = new Ellipse(ellipseRendererComponent.RadiusX, ellipseRendererComponent.RadiusY);
                    _renderer2D.RenderEllipse(ellipse, ellipseRendererComponent.Color, ellipseRendererComponent.FillInterior, transformationMatrix);
                }
            }
        }

        private void RenderDiagnosticInfo()
        {
            var width = _renderer2D.ScreenWidth;
            var height = _renderer2D.ScreenHeight;
            var color = Color.FromArgb(255, 0, 255, 0);

            var translation = new Vector2(-(width / 2d) + 1, height / 2d - 1);

            foreach (var diagnosticInfo in _aggregatedDiagnosticInfoProvider.GetAllDiagnosticInfo())
            {
                _renderer2D.RenderText(diagnosticInfo.ToString(), FontSize.FromDips(14), color, Matrix3x3.CreateTranslation(translation));
                translation -= new Vector2(0, 14);
            }
        }

        private static Vector2 ComputeClipDimension(CameraComponent cameraComponent)
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