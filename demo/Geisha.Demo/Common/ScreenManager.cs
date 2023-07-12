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
                "Screen08_RenderingOrderInLayer"
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