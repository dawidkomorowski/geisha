using System.Linq;
using Geisha.Engine.Audio.Components;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Rendering.Components;
using Geisha.Framework.Audio;
using Geisha.Framework.Input;
using Geisha.Framework.Rendering;
using Geisha.TestGame.Behaviors;

namespace Geisha.TestGame
{
    // TODO Systems should only iterate over entities of interest - some event based (component added/removed etc.) internal list of entities (of interest) should be introduced?
    public class TestSystem : IFixedTimeStepSystem
    {
        private readonly IAssetStore _assetStore;
        private readonly IEngineManager _engineManager;
        private readonly ISceneLoader _sceneLoader;
        private readonly ISceneManager _sceneManager;
        private bool _flag;

        public TestSystem(IAssetStore assetStore, IEngineManager engineManager, ISceneLoader sceneLoader, ISceneManager sceneManager)
        {
            _assetStore = assetStore;
            _engineManager = engineManager;
            _sceneLoader = sceneLoader;
            _sceneManager = sceneManager;
        }

        public string Name => GetType().FullName;

        public void FixedUpdate(Scene scene)
        {
            var box = scene.AllEntities.Single(e => e.HasComponent<BoxMovementComponent>());

            foreach (var entity in scene.AllEntities.ToList())
            {
                if (entity.HasComponent<DieFromBoxComponent>())
                {
                    var collider = entity.GetComponent<CircleColliderComponent>();
                    if (collider.IsColliding && collider.CollidingEntities.Contains(box))
                    {
                        var soundEntity = new Entity();
                        soundEntity.AddComponent(new AudioSourceComponent {Sound = _assetStore.GetAsset<ISound>(AssetsIds.SfxSound)});
                        scene.AddEntity(soundEntity);

                        entity.Destroy();
                    }
                }

                if (entity.HasComponent<CloseGameOnEscapeKeyComponent>())
                {
                    var inputComponent = entity.GetComponent<InputComponent>();
                    if (inputComponent.HardwareInput.KeyboardInput[Key.Escape]) _engineManager.ScheduleEngineShutdown();
                    if (inputComponent.HardwareInput.KeyboardInput[Key.F5]) _sceneLoader.Save(scene, "quicksave.scene");
                    if (inputComponent.HardwareInput.KeyboardInput[Key.F9]) _sceneManager.LoadScene("quicksave.scene");

                    if (inputComponent.HardwareInput.KeyboardInput[Key.F1])
                    {
                        _flag = !_flag;

                        var boxLabel = scene.AllEntities.Single(e => e.Name == "BoxLabel").GetComponent<TextRendererComponent>();
                        var dotRenderingWith = _flag ? nameof(EllipseRendererComponent) : nameof(SpriteRendererComponent);
                        boxLabel.Text = $"Dot rendering with: {dotRenderingWith}";

                        foreach (var dot in scene.AllEntities.Where(e => e.Name == "Dot"))
                        {
                            if (_flag)
                            {
                                var spriteRendererComponent = dot.GetComponent<SpriteRendererComponent>();
                                dot.RemoveComponent(spriteRendererComponent);
                                dot.AddComponent(new EllipseRendererComponent
                                {
                                    Radius = 32,
                                    Color = Color.FromArgb(255, 0, 0, 0),
                                    FillInterior = true
                                });
                            }
                            else
                            {
                                var ellipseRendererComponent = dot.GetComponent<EllipseRendererComponent>();
                                dot.RemoveComponent(ellipseRendererComponent);
                                dot.AddComponent(new SpriteRendererComponent {Sprite = _assetStore.GetAsset<Sprite>(AssetsIds.DotSprite)});
                            }
                        }
                    }
                }
            }
        }
    }
}