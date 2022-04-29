using System.Collections.Generic;
using System.Linq;
using Geisha.Common.Logging;
using Geisha.Common.Math;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Engine.Rendering.Systems
{
    internal sealed class RenderingSystem : IRenderingSystem, ISceneObserver
    {
        private static readonly ILog Log = LogFactory.Create(typeof(RenderingSystem));
        private readonly IRenderer2D _renderer2D;
        private readonly RenderingConfiguration _renderingConfiguration;
        private readonly IAggregatedDiagnosticInfoProvider _aggregatedDiagnosticInfoProvider;
        private readonly IDebugRendererForRenderingSystem _debugRendererForRenderingSystem;
        private readonly List<string> _sortingLayersOrder;
        private readonly RenderingState _renderingState = new RenderingState();
        private readonly List<Entity> _renderList;

        public RenderingSystem(IRenderingBackend renderingBackend, RenderingConfiguration renderingConfiguration,
            IAggregatedDiagnosticInfoProvider aggregatedDiagnosticInfoProvider, IDebugRendererForRenderingSystem debugRendererForRenderingSystem)
        {
            _renderer2D = renderingBackend.Renderer2D;
            _aggregatedDiagnosticInfoProvider = aggregatedDiagnosticInfoProvider;
            _debugRendererForRenderingSystem = debugRendererForRenderingSystem;

            _renderingConfiguration = renderingConfiguration;
            _sortingLayersOrder = _renderingConfiguration.SortingLayersOrder.ToList();
            _renderList = new List<Entity>();
        }

        #region Implementation of IRenderingSystem

        public void RenderScene()
        {
            _renderer2D.BeginRendering();

            _renderer2D.Clear(Color.FromArgb(255, 255, 255, 255));

            var allEntities = _renderingState.GetRenderNodes().Select(rn => rn.Entity);

            if (_renderingState.CameraNode != null)
            {
                var cameraComponent = _renderingState.CameraNode.Camera;
                cameraComponent.ScreenWidth = _renderer2D.ScreenWidth;
                cameraComponent.ScreenHeight = _renderer2D.ScreenHeight;
                var cameraTransformationMatrix = _renderingState.CameraNode.Entity.Create2DWorldToScreenMatrix();

                if (cameraComponent.AspectRatioBehavior == AspectRatioBehavior.Underscan)
                {
                    _renderer2D.Clear(Color.FromArgb(255, 0, 0, 0));

                    var clipDimension = ComputeClipDimension(cameraComponent);
                    var clippingRectangle = new AxisAlignedRectangle(clipDimension);
                    _renderer2D.SetClippingRectangle(clippingRectangle);

                    _renderer2D.Clear(Color.FromArgb(255, 255, 255, 255));
                }

                UpdateRenderList(allEntities);
                RenderEntities(cameraTransformationMatrix);

                _debugRendererForRenderingSystem.DrawDebugInformation(_renderer2D, cameraTransformationMatrix);

                if (cameraComponent.AspectRatioBehavior == AspectRatioBehavior.Underscan)
                {
                    _renderer2D.ClearClipping();
                }
            }
            else
            {
                Log.Warn("No camera component found in scene.");
            }

            RenderDiagnosticInfo();

            _renderer2D.EndRendering(_renderingConfiguration.EnableVSync);
        }

        #endregion

        #region Implementation of ISceneObserver

        public void OnEntityCreated(Entity entity)
        {
        }

        public void OnEntityRemoved(Entity entity)
        {
        }

        public void OnEntityParentChanged(Entity entity, Entity? oldParent, Entity? newParent)
        {
        }

        public void OnComponentCreated(Component component)
        {
            switch (component)
            {
                case Transform2DComponent transform2DComponent:
                    _renderingState.CreateStateFor(transform2DComponent);
                    break;
                case Renderer2DComponent renderer2DComponent:
                    _renderingState.CreateStateFor(renderer2DComponent);
                    break;
                case CameraComponent cameraComponent:
                    _renderingState.CreateStateFor(cameraComponent);
                    break;
            }
        }

        public void OnComponentRemoved(Component component)
        {
        }

        #endregion

        private void UpdateRenderList(IEnumerable<Entity> allEntities)
        {
            _renderList.Clear();
            foreach (var entity in allEntities)
            {
                if (entity.HasComponent<Renderer2DComponent>() && entity.HasComponent<Transform2DComponent>())
                {
                    var renderer = entity.GetComponent<Renderer2DComponent>();
                    if (renderer.Visible) _renderList.Add(entity);
                }
            }

            _renderList.Sort((entity1, entity2) =>
            {
                var r1 = entity1.GetComponent<Renderer2DComponent>();
                var r2 = entity2.GetComponent<Renderer2DComponent>();

                var layersComparison = _sortingLayersOrder.IndexOf(r1.SortingLayerName) - _sortingLayersOrder.IndexOf(r2.SortingLayerName);

                return layersComparison == 0 ? r1.OrderInLayer - r2.OrderInLayer : layersComparison;
            });
        }

        private void RenderEntities(Matrix3x3 cameraTransformationMatrix)
        {
            foreach (var entity in _renderList)
            {
                var transformationMatrix = TransformHierarchy.Calculate2DTransformationMatrix(entity);
                transformationMatrix = cameraTransformationMatrix * transformationMatrix;

                if (entity.HasComponent<SpriteRendererComponent>())
                {
                    var sprite = entity.GetComponent<SpriteRendererComponent>().Sprite;
                    if (sprite != null) _renderer2D.RenderSprite(sprite, transformationMatrix);
                }

                if (entity.HasComponent<TextRendererComponent>())
                {
                    var textRenderer = entity.GetComponent<TextRendererComponent>();
                    _renderer2D.RenderText(textRenderer.Text, textRenderer.FontSize, textRenderer.Color, transformationMatrix);
                }

                if (entity.HasComponent<RectangleRendererComponent>())
                {
                    var rectangleRenderer = entity.GetComponent<RectangleRendererComponent>();
                    var rectangle = new AxisAlignedRectangle(rectangleRenderer.Dimension);
                    _renderer2D.RenderRectangle(rectangle, rectangleRenderer.Color, rectangleRenderer.FillInterior, transformationMatrix);
                }

                if (entity.HasComponent<EllipseRendererComponent>())
                {
                    var ellipseRenderer = entity.GetComponent<EllipseRendererComponent>();
                    var ellipse = new Ellipse(ellipseRenderer.RadiusX, ellipseRenderer.RadiusY);
                    _renderer2D.RenderEllipse(ellipse, ellipseRenderer.Color, ellipseRenderer.FillInterior, transformationMatrix);
                }
            }
        }

        private void RenderDiagnosticInfo()
        {
            var width = _renderer2D.ScreenWidth;
            var height = _renderer2D.ScreenHeight;
            var color = Color.FromArgb(255, 0, 255, 0);

            var translation = new Vector2(-(width / 2) + 1, height / 2 - 1);

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