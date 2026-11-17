using Geisha.Engine.Core.Math;
using Geisha.Engine.Input.Backend;
using System.Drawing;
using System.Windows.Forms;

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
    public bool LockCursorPosition { get; set; }

    // TODO: Add documentation.
    public HardwareInput Capture()
    {
        var keyboardInput = GetKeyboardInput();
        var mouseInput = GetMouseInput();
        return new HardwareInput(keyboardInput, mouseInput);
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
        _mouseInputBuilder.PositionDelta = _mouseInputBuilder.Position - _lastCapturedMousePosition;
        var mouseInput = _mouseInputBuilder.Build();
        _mouseInputBuilder.ScrollDelta = 0;
        _lastCapturedMousePosition = mouseInput.Position;

        if (LockCursorPosition)
        {
            var clientSize = _form.ClientSize;
            SetMousePosition(new Vector2(clientSize.Width / 2d, clientSize.Height / 2d));
        }

        return mouseInput;
    }

    // TODO: Add documentation.
    public ControllerState GetControllerState(ControllerIndex controllerIndex) => XInputController.GetState(controllerIndex);

    // TODO: Add documentation.
    public void SetControllerVibration(ControllerIndex controllerIndex, Vibration vibration) => XInputController.SetVibration(controllerIndex, vibration);

    private void SetMousePosition(Vector2 position)
    {
        Cursor.Position = _form.PointToScreen(new Point((int)position.X, (int)position.Y));
        _lastCapturedMousePosition = position;
    }

    #region Event handlers

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