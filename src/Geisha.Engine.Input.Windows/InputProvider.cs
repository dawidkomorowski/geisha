using System.Windows.Forms;

namespace Geisha.Engine.Input.Windows
{
    internal class InputProvider : IInputProvider
    {
        private readonly Form _form;

        private readonly KeyboardInputBuilder _keyboardInputBuilder = new KeyboardInputBuilder();


        public InputProvider(Form form)
        {
            _form = form;

            _form.KeyDown += FormOnKeyDown;
            _form.KeyUp += FormOnKeyUp;
        }

        private void FormOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                _keyboardInputBuilder.Up = true;
            }

            if (e.KeyCode == Keys.Down)
            {
                _keyboardInputBuilder.Down = true;
            }

            if (e.KeyCode == Keys.Left)
            {
                _keyboardInputBuilder.Left = true;
            }

            if (e.KeyCode == Keys.Right)
            {
                _keyboardInputBuilder.Right = true;
            }
        }

        private void FormOnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                _keyboardInputBuilder.Up = false;
            }

            if (e.KeyCode == Keys.Down)
            {
                _keyboardInputBuilder.Down = false;
            }

            if (e.KeyCode == Keys.Left)
            {
                _keyboardInputBuilder.Left = false;
            }

            if (e.KeyCode == Keys.Right)
            {
                _keyboardInputBuilder.Right = false;
            }
        }

        public HardwareInput Capture()
        {
            var keyboardInput = CaptureKeyboardInput();
            var mouseInput = CaptureMouseInput();
            return new HardwareInput(keyboardInput, mouseInput);
        }

        private KeyboardInput CaptureKeyboardInput()
        {
            return _keyboardInputBuilder.Build();
        }

        private MouseInput CaptureMouseInput()
        {
            //System.Windows.Forms.Cursor.Position = new System.Drawing.Point(100, 100);
            // TODO Does not work as application runs WinForms event dispatcher while it need WPF one.
            //return new MouseInput(Vector2.Zero,
            //    Mouse.LeftButton == MouseButtonState.Pressed,
            //    Mouse.MiddleButton == MouseButtonState.Pressed,
            //    Mouse.RightButton == MouseButtonState.Pressed,
            //    Mouse.XButton1 == MouseButtonState.Pressed,
            //    Mouse.XButton2 == MouseButtonState.Pressed);

            return default;
        }
    }
}