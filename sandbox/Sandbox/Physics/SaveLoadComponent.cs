using System.Diagnostics;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;

namespace Sandbox.Physics;

public sealed class SaveLoadComponent : BehaviorComponent
{
    private readonly ISceneManager _sceneManager;
    private readonly ISceneLoader _sceneLoader;

    public SaveLoadComponent(Entity entity, ISceneManager sceneManager, ISceneLoader sceneLoader) : base(entity)
    {
        _sceneManager = sceneManager;
        _sceneLoader = sceneLoader;
    }

    public override void OnStart()
    {
        if (!Entity.HasComponent<InputComponent>())
        {
            var inputComponent = Entity.CreateComponent<InputComponent>();
            inputComponent.InputMapping = new InputMapping
            {
                ActionMappings =
                {
                    new ActionMapping
                    {
                        ActionName = "Save",
                        HardwareActions =
                        {
                            new HardwareAction
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.F9)
                            }
                        }
                    },
                    new ActionMapping
                    {
                        ActionName = "Load",
                        HardwareActions =
                        {
                            new HardwareAction
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.F12)
                            }
                        }
                    }
                }
            };

            inputComponent.BindAction("Save", SaveScene);
            inputComponent.BindAction("Load", LoadScene);
        }
    }

    private void SaveScene()
    {
        Debug.WriteLine("SaveScene");
    }

    private void LoadScene()
    {
        Debug.WriteLine("LoadScene");
    }
}

public sealed class SaveLoadComponentFactory : ComponentFactory<SaveLoadComponent>
{
    private readonly ISceneManager _sceneManager;
    private readonly ISceneLoader _sceneLoader;

    public SaveLoadComponentFactory(ISceneManager sceneManager, ISceneLoader sceneLoader)
    {
        _sceneManager = sceneManager;
        _sceneLoader = sceneLoader;
    }

    protected override SaveLoadComponent CreateComponent(Entity entity) => new(entity, _sceneManager, _sceneLoader);
}