using System;
using System.Drawing;
using System.Windows.Forms;
using Geisha.Engine.Windowing.Backend;
using SharpDX.Windows;
using Size = Geisha.Engine.Core.Math.Size;

namespace Geisha.Engine.Windowing.Windows;

/// <summary>
///     Windowing backend implementation for Windows using WinForms.
/// </summary>
public sealed class WindowsWindowingBackend : IWindowingBackend, IDisposable
{
    private readonly RenderForm _renderForm;
    private DisplayMode _displayMode = DisplayMode.Windowed;
    private bool _cursorVisible = true;
    private CursorLockMode _cursorLockMode = CursorLockMode.None;
    private WindowState _windowState;

    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowsWindowingBackend" /> class.
    /// </summary>
    public WindowsWindowingBackend()
    {
        Application.SetHighDpiMode(HighDpiMode.SystemAware);

        _renderForm = new RenderForm();
        _renderForm.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

        _renderForm.ClientSizeChanged += RenderForm_ClientSizeChanged;
        _renderForm.LocationChanged += RenderForm_LocationChanged;
        _renderForm.GotFocus += RenderForm_GotFocus;
        _renderForm.LostFocus += RenderForm_LostFocus;

        SaveWindowState();
    }

    /// <summary>
    ///     Gets the WinForms window managed by this backend.
    /// </summary>
    public Form Window => _renderForm;

    /// <inheritdoc />
    public string WindowTitle
    {
        get => _renderForm.Text;
        set => _renderForm.Text = value;
    }

    /// <inheritdoc />
    public Size WindowClientSize
    {
        get
        {
            var clientSize = _renderForm.ClientSize;
            return new Size(clientSize.Width, clientSize.Height);
        }
        set => _renderForm.ClientSize = new System.Drawing.Size(value.Width, value.Height);
    }

    /// <inheritdoc />
    public DisplayMode DisplayMode
    {
        get => _displayMode;
        set
        {
            if (_displayMode == value)
            {
                return;
            }

            _displayMode = value;

            switch (value)
            {
                case DisplayMode.Windowed:
                    _renderForm.IsFullscreen = false;

                    // Force RenderForm to update window style.
                    _renderForm.AllowUserResizing = !_renderForm.AllowUserResizing;

                    RestoreWindowState();
                    break;
                case DisplayMode.Fullscreen:
                    SaveWindowState();
                    _renderForm.IsFullscreen = true;

                    // Force RenderForm to update window style.
                    _renderForm.AllowUserResizing = !_renderForm.AllowUserResizing;

                    _renderForm.AllowUserResizing = false;
                    _renderForm.WindowState = FormWindowState.Normal;
                    _renderForm.ClientSize = Screen.PrimaryScreen.Bounds.Size;
                    _renderForm.Location = new Point(0, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported display mode.");
            }
        }
    }

    /// <inheritdoc />
    public bool AllowWindowResizing
    {
        get => _renderForm.AllowUserResizing;
        set => _renderForm.AllowUserResizing = value;
    }

    /// <inheritdoc />
    public bool CursorVisible
    {
        get => _cursorVisible;
        set
        {
            if (_cursorVisible == value)
            {
                return;
            }

            _cursorVisible = value;

            if (_cursorVisible)
            {
                Cursor.Show();
            }
            else
            {
                Cursor.Hide();
            }
        }
    }

    // TODO: Add documentation.
    public CursorLockMode CursorLockMode
    {
        get => _cursorLockMode;
        set
        {
            if (_cursorLockMode == value)
            {
                return;
            }

            _cursorLockMode = value;

            HandleCursorConfinedMode();
        }
    }

    /// <inheritdoc />
    public void RunUpdateLoop(Func<bool> updateCallback)
    {
        RenderLoop.Run(_renderForm, () =>
        {
            HandleCursorLockedMode();

            var shouldContinue = updateCallback();
            if (!shouldContinue) _renderForm.Close();
        });
    }

    /// <summary>
    ///     Releases the window resources.
    /// </summary>
    public void Dispose()
    {
        NativeCursor.ReleaseCursorClip();

        _renderForm.ClientSizeChanged -= RenderForm_ClientSizeChanged;
        _renderForm.LocationChanged -= RenderForm_LocationChanged;
        _renderForm.GotFocus -= RenderForm_GotFocus;
        _renderForm.LostFocus -= RenderForm_LostFocus;

        _renderForm.Dispose();
    }

    private void SaveWindowState()
    {
        _windowState = new WindowState
        {
            AllowWindowResizing = _renderForm.AllowUserResizing,
            State = _renderForm.WindowState,
            ClientSize = _renderForm.ClientSize,
            Location = _renderForm.Location
        };
    }

    private void RestoreWindowState()
    {
        _renderForm.AllowUserResizing = _windowState.AllowWindowResizing;
        _renderForm.WindowState = _windowState.State;
        _renderForm.ClientSize = _windowState.ClientSize;
        _renderForm.Location = _windowState.Location;
    }

    private readonly record struct WindowState
    {
        public bool AllowWindowResizing { get; init; }
        public FormWindowState State { get; init; }
        public System.Drawing.Size ClientSize { get; init; }
        public Point Location { get; init; }
    }

    private void HandleCursorLockedMode()
    {
        if (!_renderForm.Focused) return;

        if (CursorLockMode is CursorLockMode.Locked)
        {
            var clientSize = _renderForm.ClientSize;
            Cursor.Position = _renderForm.PointToScreen(new Point(clientSize.Width / 2, clientSize.Height / 2));
        }
    }

    private void HandleCursorConfinedMode()
    {
        if (_cursorLockMode is CursorLockMode.Confined && _renderForm.Focused)
        {
            NativeCursor.ClipCursorToWindow(_renderForm);
        }
        else
        {
            NativeCursor.ReleaseCursorClip();
        }
    }

    private void RenderForm_ClientSizeChanged(object? sender, EventArgs e)
    {
        HandleCursorConfinedMode();
    }

    private void RenderForm_LocationChanged(object? sender, EventArgs e)
    {
        HandleCursorConfinedMode();
    }

    private void RenderForm_GotFocus(object? sender, EventArgs e)
    {
        HandleCursorConfinedMode();
    }

    private void RenderForm_LostFocus(object? sender, EventArgs e)
    {
        HandleCursorConfinedMode();
    }
}