using Geisha.Engine.Windowing.Backend;
using SharpDX.Windows;

namespace Geisha.Engine.Windowing.Windows;

// TODO: Add documentation.
public sealed class WindowsWindowingBackend : IWindowingBackend
{
    private readonly RenderForm _renderForm;

    public WindowsWindowingBackend(RenderForm renderForm)
    {
        _renderForm = renderForm;
    }

    public bool AllowWindowResizing
    {
        get => _renderForm.AllowUserResizing;
        set => _renderForm.AllowUserResizing = value;
    }
}