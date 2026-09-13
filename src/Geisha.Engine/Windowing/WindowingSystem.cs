using System.Diagnostics;
using Geisha.Engine.Core.GameLoop;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Windowing.Backend;

namespace Geisha.Engine.Windowing;

internal sealed class WindowingSystem : IWindowingGameLoopStep
{
    private readonly IWindowingBackend _windowingBackend;
    private Size _windowClientSize;

    public WindowingSystem(IWindowingBackend windowingBackend)
    {
        _windowingBackend = windowingBackend;
        _windowClientSize = _windowingBackend.WindowClientSize;
    }

    public void HandleWindowResize()
    {
        var currentSize = _windowingBackend.WindowClientSize;

        if (currentSize != _windowClientSize)
        {
            _windowClientSize = currentSize;

            Debug.WriteLine($"Resize to {currentSize}");
            // TODO: Implement resize.
        }
    }
}