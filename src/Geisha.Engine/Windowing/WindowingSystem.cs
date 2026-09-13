using System.Diagnostics;
using Geisha.Engine.Core.GameLoop;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Windowing.Backend;

namespace Geisha.Engine.Windowing;

internal sealed class WindowingSystem : IWindowingGameLoopStep
{
    private readonly IWindowingBackend _windowingBackend;
    private readonly IRenderingBackend _renderingBackend;
    private Size _windowClientSize;

    public WindowingSystem(IWindowingBackend windowingBackend, IRenderingBackend renderingBackend)
    {
        _windowingBackend = windowingBackend;
        _renderingBackend = renderingBackend;

        _windowClientSize = _windowingBackend.WindowClientSize;
    }

    public void HandleWindowResize()
    {
        var currentSize = _windowingBackend.WindowClientSize;

        if (currentSize != _windowClientSize)
        {
            _windowClientSize = currentSize;
            _renderingBackend.ResizeBuffers(_windowClientSize);
        }
    }
}