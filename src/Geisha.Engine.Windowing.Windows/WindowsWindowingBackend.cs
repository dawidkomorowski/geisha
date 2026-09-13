using Geisha.Engine.Core.Math;
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

    public Size WindowClientSize
    {
        get
        {
            var clientSize = _renderForm.ClientSize;
            return new Size(clientSize.Width, clientSize.Height);
        }
        set => _renderForm.ClientSize = new System.Drawing.Size(value.Width, value.Height);
    }
}