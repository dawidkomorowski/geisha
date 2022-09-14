using System.Drawing;
using System.Windows.Forms;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Input.Backend;

namespace Geisha.Engine.Input.Windows
{
    internal sealed class InputProvider : IInputProvider
    {
        private readonly Form _form;

        private readonly KeyboardInputBuilder _keyboardInputBuilder = new KeyboardInputBuilder();
        private readonly MouseInputBuilder _mouseInputBuilder = new MouseInputBuilder();
        private Vector2 _lastCapturedMousePosition;

        public InputProvider(Form form)
        {
            _form = form;
            _form.MouseDown += FormOnMouseDown;
            _form.MouseUp += FormOnMouseUp;
            _form.MouseMove += FormOnMouseMove;
            _form.MouseWheel += FormOnMouseWheel;
        }

        public bool LockCursorPosition { get; set; }

        public HardwareInput Capture()
        {
            var keyboardInput = CaptureKeyboardInput();
            var mouseInput = CaptureMouseInput();
            return new HardwareInput(keyboardInput, mouseInput);
        }

        private KeyboardInput CaptureKeyboardInput()
        {
            NativeKeyboard.SetBuilderToCurrentState(_keyboardInputBuilder);
            return _keyboardInputBuilder.Build();
        }

        private MouseInput CaptureMouseInput()
        {
            _mouseInputBuilder.PositionDelta = _mouseInputBuilder.Position - _lastCapturedMousePosition;
            var mouseInput = _mouseInputBuilder.Build();
            _mouseInputBuilder.ScrollDelta = 0;
            _lastCapturedMousePosition = mouseInput.Position;

            if (LockCursorPosition)
            {
                SetMousePosition(new Vector2(_form.Width / 2d, _form.Height / 2d));
            }

            return mouseInput;
        }

        private void SetMousePosition(Vector2 position)
        {
            Cursor.Position = _form.PointToScreen(new Point((int) position.X, (int) position.Y));
            _lastCapturedMousePosition = position;
        }

        #region Event handlers

        private void FormOnMouseDown(object sender, MouseEventArgs e)
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

        private void FormOnMouseUp(object sender, MouseEventArgs e)
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

        private void FormOnMouseMove(object sender, MouseEventArgs e)
        {
            _mouseInputBuilder.Position = new Vector2(e.X, e.Y);
        }

        private void FormOnMouseWheel(object sender, MouseEventArgs e)
        {
            _mouseInputBuilder.ScrollDelta = e.Delta;
        }

        #endregion
    }
}