using System;
using System.ComponentModel.Composition;
using System.Linq;
using Geisha.Engine.Audio.Components;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Physics.Components;
using Geisha.Framework.Audio;
using Geisha.Framework.Input;
using Geisha.TestGame.Behaviors;

namespace Geisha.TestGame
{
    // TODO Systems should only iterate over entities of interest - some event based (component added/removed etc.) internal list of entities (of interest) should be introduced?
    [Export(typeof(IFixedTimeStepSystem))]
    public class TestSystem : IFixedTimeStepSystem
    {
        private readonly IAssetStore _assetStore;
        private readonly IEngineManager _engineManager;

        [ImportingConstructor]
        public TestSystem(IAssetStore assetStore, IEngineManager engineManager)
        {
            _assetStore = assetStore;
            _engineManager = engineManager;
        }

        public string Name => GetType().FullName;

        public void FixedUpdate(Scene scene)
        {
            foreach (var entity in scene.AllEntities.ToList())
            {
                if (entity.HasComponent<DieFromBox>())
                {
                    var box = scene.AllEntities.Single(e => e.HasComponent<BoxMovement>());
                    var collider = entity.GetComponent<CircleCollider>();

                    if (collider.IsColliding && collider.CollidingEntities.Contains(box))
                    {
                        var soundEntity = new Entity();
                        soundEntity.AddComponent(new AudioSource {Sound = _assetStore.GetAsset<ISound>(new Guid("205F7A78-E8FA-49D5-BCF4-3174EBB728FF"))});
                        scene.AddEntity(soundEntity);

                        entity.Destroy();
                    }
                }

                if (entity.HasComponent<CloseGameOnEscapeKey>())
                {
                    var inputComponent = entity.GetComponent<InputComponent>();
                    if (inputComponent.HardwareInput.KeyboardInput[Key.Escape]) _engineManager.ScheduleEngineShutdown();
                }
            }
        }
    }
}