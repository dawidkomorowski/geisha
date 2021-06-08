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

            componentsRegistry.RegisterComponent<BoxMovementComponentFactory>();
            componentsRegistry.RegisterComponent<CloseGameOnEscapeKeyComponentFactory>();
            componentsRegistry.RegisterComponent<DieFromBoxComponentFactory>();
            componentsRegistry.RegisterComponent<DoMagicWithTextComponentFactory>();
            componentsRegistry.RegisterComponent<FollowEllipseComponentFactory>();
            componentsRegistry.RegisterComponent<MousePointerComponentFactory>();
            componentsRegistry.RegisterComponent<MusicControllerComponentFactory>();
            componentsRegistry.RegisterComponent<RotateComponentFactory>();
            componentsRegistry.RegisterComponent<SetTextForCurrentKeyComponentFactory>();
            componentsRegistry.RegisterComponent<SetTextForMouseInfoComponentFactory>();
            componentsRegistry.RegisterComponent<TopDownCameraForBoxComponentFactory>();
        }
    }
}