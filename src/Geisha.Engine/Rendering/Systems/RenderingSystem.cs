using System.Collections.Generic;
using System.Linq;
using Geisha.Common.Logging;
using Geisha.Common.Math;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Configuration;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.Rendering.Configuration;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Systems
{
    internal class RenderingSystem : IVariableTimeStepSystem
    {
        private static readonly ILog Log = LogFactory.Create(typeof(RenderingSystem));
        private readonly IAggregatedDiagnosticInfoProvider _aggregatedDiagnosticInfoProvider;
        private readonly IRenderer2D _renderer2D;
        private readonly RenderingConfiguration _renderingConfiguration;
        private readonly List<Entity> _renderList;

        public RenderingSystem(IRenderer2D renderer2D, IConfigurationManager configurationManager,
            IAggregatedDiagnosticInfoProvider aggregatedDiagnosticInfoProvider)
        {
            _renderer2D = renderer2D;
            _aggregatedDiagnosticInfoProvider = aggregatedDiagnosticInfoProvider;

            _renderingConfiguration = configurationManager.GetConfiguration<RenderingConfiguration>();
            _renderList = new List<Entity>();
        }

        public string Name => GetType().FullName;

        public void Update(Scene scene, GameTime gameTime)
        {
            _renderer2D.BeginRendering();

            _renderer2D.Clear(Color.FromArgb(255, 255, 255, 255));

            if (TryGetCameraTransformationMatrix(scene, out var cameraTransformationMatrix))
            {
                UpdateRenderList(scene);

                foreach (var entity in _renderList)
                {
                    var transformationMatrix = entity.GetComponent<TransformComponent>().Create2DTransformationMatrix();
                    transformationMatrix = cameraTransformationMatrix * transformationMatrix;

                    if (entity.HasComponent<SpriteRendererComponent>())
                    {
                        var sprite = entity.GetComponent<SpriteRendererComponent>().Sprite;
                        _renderer2D.RenderSprite(sprite, transformationMatrix);
                    }

                    if (entity.HasComponent<TextRendererComponent>())
                    {
                        var textRenderer = entity.GetComponent<TextRendererComponent>();
                        _renderer2D.RenderText(textRenderer.Text, textRenderer.FontSize, textRenderer.Color, transformationMatrix);
                    }

                    if (entity.HasComponent<RectangleRendererComponent>())
                    {
                        var rectangleRenderer = entity.GetComponent<RectangleRendererComponent>();
                        var rectangle = new Rectangle(rectangleRenderer.Dimension);
                        _renderer2D.RenderRectangle(rectangle, rectangleRenderer.Color, rectangleRenderer.FillInterior, transformationMatrix);
                    }
                }
            }

            RenderDiagnosticInfo();

            _renderer2D.EndRendering(_renderingConfiguration.EnableVSync);
        }

        private void UpdateRenderList(Scene scene)
        {
            _renderList.Clear();
            foreach (var entity in scene.AllEntities)
            {
                if (entity.HasComponent<Renderer2DComponent>() && entity.HasComponent<TransformComponent>())
                {
                    var renderer = entity.GetComponent<Renderer2DComponent>();
                    if (renderer.Visible)
                    {
                        _renderList.Add(entity);
                    }
                }
            }

            _renderList.Sort((entity1, entity2) =>
            {
                var r1 = entity1.GetComponent<Renderer2DComponent>();
                var r2 = entity2.GetComponent<Renderer2DComponent>();

                var sortingLayersOrder = _renderingConfiguration.SortingLayersOrder;
                var layersComparison = sortingLayersOrder.IndexOf(r1.SortingLayerName) - sortingLayersOrder.IndexOf(r2.SortingLayerName);

                return layersComparison == 0 ? r1.OrderInLayer - r2.OrderInLayer : layersComparison;
            });
        }

        // TODO It is inefficient to traverse all entities to find a camera each time.
        private static bool TryGetCameraTransformationMatrix(Scene scene, out Matrix3x3 cameraTransformationMatrix)
        {
            cameraTransformationMatrix = Matrix3x3.Identity;
            var cameraEntity = scene.AllEntities.SingleOrDefault(e => e.HasComponent<CameraComponent>() && e.HasComponent<TransformComponent>());
            if (cameraEntity == null)
            {
                Log.Warn("No camera component found in scene.");
                return false;
            }

            var camera = cameraEntity.GetComponent<CameraComponent>();
            var cameraTransform = cameraEntity.GetComponent<TransformComponent>();
            var cameraScale = cameraTransform.Scale.ToVector2();
            cameraTransformationMatrix = Matrix3x3.CreateScale(new Vector2(1 / cameraScale.X, 1 / cameraScale.Y)) *
                                         Matrix3x3.CreateRotation(-cameraTransform.Rotation.Z) *
                                         Matrix3x3.CreateTranslation(-cameraTransform.Translation.ToVector2()) * Matrix3x3.Identity;

            return true;
        }

        private void RenderDiagnosticInfo()
        {
            var width = _renderer2D.Window.ClientAreaWidth;
            var height = _renderer2D.Window.ClientAreaHeight;
            var color = Color.FromArgb(255, 0, 255, 0);
            var transform = new TransformComponent
            {
                Translation = new Vector3(-(width / 2) + 1, height / 2 - 1, 0),
                Rotation = Vector3.Zero,
                Scale = Vector3.One
            };

            foreach (var diagnosticInfo in _aggregatedDiagnosticInfoProvider.GetAllDiagnosticInfo())
            {
                _renderer2D.RenderText(diagnosticInfo.ToString(), FontSize.FromDips(14), color, transform.Create2DTransformationMatrix());
                transform.Translation = transform.Translation - new Vector3(0, 14, 0);
            }
        }
    }
}