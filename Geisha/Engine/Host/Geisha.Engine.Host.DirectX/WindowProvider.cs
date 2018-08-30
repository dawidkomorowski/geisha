using Geisha.Framework.Rendering;

namespace Geisha.Engine.Host.DirectX
{
    internal sealed class WindowProvider : IWindowProvider
    {
        public WindowProvider(IWindow window)
        {
            Window = window;
        }

        public IWindow Window { get; }
    }
}