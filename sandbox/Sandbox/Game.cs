using Geisha.Engine;
using Sandbox.Behaviors;

namespace Sandbox
{
    public sealed class Game : IGame
    {
        public string WindowTitle => "Geisha Engine Sandbox";

        public void RegisterComponents(IComponentsRegistry componentsRegistry)
        {
            componentsRegistry.RegisterSystem<SandboxSystem>();
            componentsRegistry.RegisterSceneBehaviorFactory<SandboxSceneBehaviorFactory>();

            componentsRegistry.RegisterComponentFactory<BoxMovementComponentFactory>();
            componentsRegistry.RegisterComponentFactory<CloseGameOnEscapeKeyComponentFactory>();
            componentsRegistry.RegisterComponentFactory<DieFromBoxComponentFactory>();
            componentsRegistry.RegisterComponentFactory<DoMagicWithTextComponentFactory>();
            componentsRegistry.RegisterComponentFactory<FollowEllipseComponentFactory>();
            componentsRegistry.RegisterComponentFactory<MousePointerComponentFactory>();
            componentsRegistry.RegisterComponentFactory<MusicControllerComponentFactory>();
            componentsRegistry.RegisterComponentFactory<RotateComponentFactory>();
            componentsRegistry.RegisterComponentFactory<SetTextForCurrentKeyComponentFactory>();
            componentsRegistry.RegisterComponentFactory<SetTextForMouseInfoComponentFactory>();
            componentsRegistry.RegisterComponentFactory<TopDownCameraForBoxComponentFactory>();
        }
    }
}