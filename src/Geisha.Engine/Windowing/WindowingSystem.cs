using Geisha.Engine.Core.GameLoop;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Windowing.Backend;

namespace Geisha.Engine.Windowing;

internal sealed class WindowingSystem : IWindowingSystem, IWindowingGameLoopStep
{
    private readonly IWindowingBackend _windowingBackend;
    private readonly IRenderingBackend _renderingBackend;
    private Size _windowClientSize;

    public WindowingSystem(IWindowingBackend windowingBackend, IRenderingBackend renderingBackend)
    {
        _windowingBackend = windowingBackend;
        _renderingBackend = renderingBackend;

        _windowClientSize = _windowingBackend.WindowClientSize;
        CursorVisible = _windowingBackend.CursorVisible;
        DisplayMode = _windowingBackend.DisplayMode;
    }

    public bool CursorVisible { get; set; }
    public DisplayMode DisplayMode { get; set; }

    public void HandleWindowState()
    {
        if (CursorVisible != _windowingBackend.CursorVisible)
        {
            _windowingBackend.CursorVisible = CursorVisible;
        }

        if (DisplayMode != _windowingBackend.DisplayMode)
        {
            _windowingBackend.DisplayMode = DisplayMode;
        }

        var currentSize = _windowingBackend.WindowClientSize;

        if (currentSize != _windowClientSize)
        {
            _windowClientSize = currentSize;

            // When window is minimized it gets zero size but buffer cannot be zero.
            if (_windowClientSize.Width > 0 && _windowClientSize.Height > 0)
            {
                _renderingBackend.ResizeBuffers(_windowClientSize);
            }
        }
    }
}