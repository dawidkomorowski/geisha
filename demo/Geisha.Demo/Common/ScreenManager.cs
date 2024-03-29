using Geisha.Engine.Core.SceneModel;

namespace Geisha.Demo.Common
{
    internal sealed class ScreenManager
    {
        private readonly ISceneManager _sceneManager;
        private readonly string[] _screens;
        private int _currentScreen = 0;

        public ScreenManager(ISceneManager sceneManager)
        {
            _sceneManager = sceneManager;
            _screens = new[]
            {
                "Screen00_Hello",
                "Screen01_Instructions",
                "Screen02_Entity",
                "Screen03_Primitives",
                "Screen04_Sprite",
                "Screen05_Text",
                "Screen06_Transform",
                "Screen07_RenderingSortingLayers",
                "Screen08_RenderingOrderInLayer",
                "Screen09_RenderingEntityHierarchy",
                "Screen10_Camera",
                "Screen11_SpriteAnimation",
                "Screen12_KeyboardInput",
                "Screen13_MouseInput",
                "Screen14_InputComponent",
                "Screen15_Audio"
            };
        }

        public void Next()
        {
            if (_currentScreen < _screens.Length - 1)
            {
                _currentScreen++;
                _sceneManager.LoadEmptyScene(_screens[_currentScreen]);
            }
        }

        public void Previous()
        {
            if (_currentScreen > 0)
            {
                _currentScreen--;
                _sceneManager.LoadEmptyScene(_screens[_currentScreen]);
            }
        }
    }
}