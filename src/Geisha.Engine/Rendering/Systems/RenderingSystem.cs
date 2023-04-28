using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.GameLoop;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Engine.Rendering.Systems
{
    internal sealed class RenderingSystem : IRenderingGameLoopStep, ISceneObserver
    {
        private readonly RenderingState _renderingState = new();
        private readonly Renderer _renderer;
        private readonly IRenderingBackend _renderingBackend;
        private readonly RenderingConfiguration _renderingConfiguration;

        public RenderingSystem(IRenderingBackend renderingBackend, RenderingConfiguration renderingConfiguration,
            IAggregatedDiagnosticInfoProvider aggregatedDiagnosticInfoProvider, IDebugRendererForRenderingSystem debugRendererForRenderingSystem)
        {
            _renderingBackend = renderingBackend;
            _renderingConfiguration = renderingConfiguration;

            _renderer = new Renderer(
                renderingBackend.Context2D,
                renderingConfiguration,
                aggregatedDiagnosticInfoProvider,
                debugRendererForRenderingSystem,
                _renderingState
            );
        }

        #region Implementation of IRenderingGameLoopStep

        public void RenderScene()
        {
            _renderer.RenderScene();
            _renderingBackend.Present(_renderingConfiguration.EnableVSync);
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
            switch (component)
            {
                case Transform2DComponent transform2DComponent:
                    _renderingState.RemoveStateFor(transform2DComponent);
                    break;
                case Renderer2DComponent renderer2DComponent:
                    _renderingState.RemoveStateFor(renderer2DComponent);
                    break;
                case CameraComponent cameraComponent:
                    _renderingState.RemoveStateFor(cameraComponent);
                    break;
            }
        }

        #endregion
    }
}