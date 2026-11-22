using System.Drawing;
using System.Windows.Forms;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Input.Backend;

namespace Geisha.Engine.Input.Windows;

/// <summary>
///     Input backend implementation for Windows. This implementation depends on WinForms, Win32, XInput.
/// </summary>
public sealed class WindowsInputBackend : IInputBackend
{
    private readonly Form _form;

    private readonly KeyboardInputBuilder _keyboardInputBuilder = new();
    private readonly MouseInputBuilder _mouseInputBuilder = new();
    private Vector2 _lastCapturedMousePosition;
    private bool _cursorLocked;

    /// <summary>
    ///     Creates new instance of <see cref="WindowsInputBackend" /> that handles input for specified <see cref="Form" />
    ///     instance.
    /// </summary>
    /// <param name="form"><see cref="Form" /> instance for which input backend handles input.</param>
    public WindowsInputBackend(Form form)
    {
        _form = form;

        _form.KeyDown += FormOnKeyDown;
        _form.MouseDown += FormOnMouseDown;
        _form.MouseUp += FormOnMouseUp;
        _form.MouseMove += FormOnMouseMove;
        _form.MouseWheel += FormOnMouseWheel;
    }

    // TODO: Add documentation.
    public KeyboardInput GetKeyboardInput()
    {
        NativeKeyboard.SetBuilderToCurrentState(_keyboardInputBuilder);
        return _keyboardInputBuilder.Build();
    }

    // TODO: Add documentation.
    public MouseInput GetMouseInput()
    {
        if (_cursorLocked && _form.Focused)
        {
            // TODO: Proper implementation to calculate mouse position delta, especially for locked cursor,
            //       is to use RAW Input API that provides physical HID device packets with hardware delta.
            //       This is only a workaround for current mouse handling based on Cursor/MouseMoveEvent API.
            // TODO: Rebaseline when entering locked mode to avoid a spurious initial delta.
            // TODO: Rebaseline when leaving locked mode to exclude the last cursor warp.
            // TODO: Rebaseline when the window center changes to avoid a spurious delta.
            var clientSize = _form.ClientSize;
            var windowCenter = new Point(clientSize.Width / 2, clientSize.Height / 2);
            var screeMouseTarget = _form.PointToScreen(windowCenter);

            var cursorPosition = _form.PointToClient(Cursor.Position);
            Cursor.Position = screeMouseTarget;

            _mouseInputBuilder.Position = new Vector2(cursorPosition.X, cursorPosition.Y);
            _lastCapturedMousePosition = new Vector2(windowCenter.X, windowCenter.Y);
        }

        _mouseInputBuilder.PositionDelta = _mouseInputBuilder.Position - _lastCapturedMousePosition;
        var mouseInput = _mouseInputBuilder.Build();
        _mouseInputBuilder.ScrollDelta = 0;
        _lastCapturedMousePosition = mouseInput.Position;

        return mouseInput;
    }

    // TODO: Add documentation.
    public ControllerState GetControllerState(ControllerIndex controllerIndex) => XInputController.GetState(controllerIndex);

    // TODO: Add documentation.
    public void SetControllerVibration(ControllerIndex controllerIndex, Vibration vibration) => XInputController.SetVibration(controllerIndex, vibration);

    #region Event handlers

    // TODO: Add documentation.
    // TODO: Document it is current workaround for Locked mode and position delta.
    public bool OnCursorLockedCallback(bool locked)
    {
        _cursorLocked = locked;
        return true;
    }

    private void FormOnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.F10)
        {
            e.SuppressKeyPress = true;
        }

        if (e.Modifiers == Keys.Alt)
        {
            e.SuppressKeyPress = true;
        }
    }

    private void FormOnMouseDown(object? sender, MouseEventArgs e)
    {
        switch (e.Button)
        {
            case MouseButtons.Left:
                _mouseInputBuilder.LeftButton = true;
                break;
            case MouseButtons.Middle:
                _mouseInputBuilder.MiddleButton = true;
                break;
            case MouseButtons.Right:
                _mouseInputBuilder.RightButton = true;
                break;
            case MouseButtons.XButton1:
                _mouseInputBuilder.XButton1 = true;
                break;
            case MouseButtons.XButton2:
                _mouseInputBuilder.XButton2 = true;
                break;
        }
    }

    private void FormOnMouseUp(object? sender, MouseEventArgs e)
    {
        switch (e.Button)
        {
            case MouseButtons.Left:
                _mouseInputBuilder.LeftButton = false;
                break;
            case MouseButtons.Middle:
                _mouseInputBuilder.MiddleButton = false;
                break;
            case MouseButtons.Right:
                _mouseInputBuilder.RightButton = false;
                break;
            case MouseButtons.XButton1:
                _mouseInputBuilder.XButton1 = false;
                break;
            case MouseButtons.XButton2:
                _mouseInputBuilder.XButton2 = false;
                break;
        }
    }

    private void FormOnMouseMove(object? sender, MouseEventArgs e)
    {
        _mouseInputBuilder.Position = new Vector2(e.X, e.Y);
    }

    private void FormOnMouseWheel(object? sender, MouseEventArgs e)
    {
        _mouseInputBuilder.ScrollDelta = e.Delta;
    }

    #endregion
}