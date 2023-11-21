using Geisha.Engine;
using System.Reflection;
using Geisha.Demo.Common;
using Geisha.Demo.Screens;

namespace Geisha.Demo
{
    internal sealed class DemoApp : Game
    {
        public override string WindowTitle =>
            $"Geisha Engine Demo {Assembly.GetAssembly(typeof(DemoApp))?.GetName().Version?.ToString(3)}";

        public override void RegisterComponents(IComponentsRegistry componentsRegistry)
        {
            componentsRegistry.RegisterSingleInstance<CommonScreenFactory>();
            componentsRegistry.RegisterSingleInstance<ScreenManager>();
            componentsRegistry.RegisterComponentFactory<MenuControlsComponentFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<MainSceneBehaviorFactory>();
            componentsRegistry.RegisterComponentFactory<GoToHelloScreenComponentFactory>();

            // Hello
            componentsRegistry.RegisterSceneBehaviorFactory<HelloSceneBehaviorFactory>();

            // Instructions
            componentsRegistry.RegisterSceneBehaviorFactory<InstructionsSceneBehaviorFactory>();

            // Entity
            componentsRegistry.RegisterSceneBehaviorFactory<EntitySceneBehaviorFactory>();

            // Primitives
            componentsRegistry.RegisterSceneBehaviorFactory<PrimitivesSceneBehaviorFactory>();

            // Sprite
            componentsRegistry.RegisterSceneBehaviorFactory<SpriteSceneBehaviorFactory>();

            // Text
            componentsRegistry.RegisterSceneBehaviorFactory<TextSceneBehaviorFactory>();

            // Transform
            componentsRegistry.RegisterSceneBehaviorFactory<TransformSceneBehaviorFactory>();

            // RenderingSortingLayers
            componentsRegistry.RegisterSceneBehaviorFactory<RenderingSortingLayersSceneBehaviorFactory>();

            // RenderingOrderInLayer
            componentsRegistry.RegisterSceneBehaviorFactory<RenderingOrderInLayerSceneBehaviorFactory>();

            // RenderingEntityHierarchy
            componentsRegistry.RegisterSceneBehaviorFactory<RenderingEntityHierarchySceneBehaviorFactory>();

            // Camera
            componentsRegistry.RegisterSceneBehaviorFactory<CameraSceneBehaviorFactory>();

            // SpriteAnimation
            componentsRegistry.RegisterSceneBehaviorFactory<SpriteAnimationSceneBehaviorFactory>();

            // KeyboardInput
            componentsRegistry.RegisterSceneBehaviorFactory<KeyboardInputSceneBehaviorFactory>();
            componentsRegistry.RegisterComponentFactory<SetTextToKeyboardInputComponentFactory>();

            // MouseInput
            componentsRegistry.RegisterSceneBehaviorFactory<MouseInputSceneBehaviorFactory>();
            componentsRegistry.RegisterComponentFactory<SetTextToMouseInputComponentFactory>();
            componentsRegistry.RegisterComponentFactory<FollowMousePositionComponentFactory>();

            // InputComponent
            componentsRegistry.RegisterSceneBehaviorFactory<InputComponentSceneBehaviorFactory>();
            componentsRegistry.RegisterComponentFactory<TODOSetTextToKeyboardInputComponentFactory>();
        }
    }
}