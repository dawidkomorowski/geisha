using System;
using System.Drawing;
using System.Windows.Forms;
using Geisha.Engine.Windowing.Backend;
using SharpDX.Windows;
using Size = Geisha.Engine.Core.Math.Size;

namespace Geisha.Engine.Windowing.Windows;

// TODO: Add documentation.
public sealed class WindowsWindowingBackend : IWindowingBackend, IDisposable
{
    private readonly RenderForm _renderForm;
    private DisplayMode _displayMode;
    private WindowState _windowState;

    public WindowsWindowingBackend()
    {
        Application.SetHighDpiMode(HighDpiMode.SystemAware);

        _renderForm = new RenderForm();
        _renderForm.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

        SaveWindowState();
    }

    public Form Window => _renderForm;

    public string WindowTitle
    {
        get => _renderForm.Text;
        set => _renderForm.Text = value;
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

    public DisplayMode DisplayMode
    {
        get => _displayMode;
        set
        {
            _displayMode = value;

            switch (value)
            {
                case DisplayMode.Windowed:
                    _renderForm.IsFullscreen = false;
                    _renderForm.WindowState = FormWindowState.Normal;
                    RestoreWindowState();
                    break;
                case DisplayMode.Fullscreen:
                    SaveWindowState();
                    _renderForm.IsFullscreen = true;
                    _renderForm.AllowUserResizing = false;
                    // TODO: If window is already maximized but not borderless it does not work properly.
                    _renderForm.WindowState = FormWindowState.Maximized;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported display mode.");
            }
        }
    }

    public bool AllowWindowResizing
    {
        get => _renderForm.AllowUserResizing;
        set => _renderForm.AllowUserResizing = value;
    }

    public void RunUpdateLoop(Func<bool> updateCallback)
    {
        RenderLoop.Run(_renderForm, () =>
        {
            var shouldContinue = updateCallback();
            if (!shouldContinue) _renderForm.Close();
        });
    }

    public void Dispose()
    {
        _renderForm.Dispose();
    }

    private void SaveWindowState()
    {
        _windowState = new WindowState
        {
            AllowWindowResizing = AllowWindowResizing
        };
    }

    private void RestoreWindowState()
    {
        AllowWindowResizing = _windowState.AllowWindowResizing;
    }

    private readonly record struct WindowState
    {
        public bool AllowWindowResizing { get; init; }
    }
}