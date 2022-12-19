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
                "Hello",
                "Tmp"
            };
        }

        public void Next()
        {
            if (_currentScreen < _screens.Length - 1)
            {
                _currentScreen++;
            }

            _sceneManager.LoadEmptyScene(_screens[_currentScreen]);
        }

        public void Previous()
        {
            if (_currentScreen > 0)
            {
                _currentScreen--;
            }

            _sceneManager.LoadEmptyScene(_screens[_currentScreen]);
        }
    }
}