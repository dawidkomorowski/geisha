using System.Collections.Generic;
using System.Windows.Forms;

namespace Geisha.Engine.Input.Windows
{
    internal class InputProvider : IInputProvider
    {
        private readonly Form _form;

        private bool _up;
        private bool _down;
        private bool _left;
        private bool _right;


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
                _up = true;
            }

            if (e.KeyCode == Keys.Down)
            {
                _down = true;
            }

            if (e.KeyCode == Keys.Left)
            {
                _left = true;
            }

            if (e.KeyCode == Keys.Right)
            {
                _right = true;
            }
        }

        private void FormOnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                _up = false;
            }

            if (e.KeyCode == Keys.Down)
            {
                _down = false;
            }

            if (e.KeyCode == Keys.Left)
            {
                _left = false;
            }

            if (e.KeyCode == Keys.Right)
            {
                _right = false;
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
            //var keyStates = Enum.GetValues(typeof(Key)).Cast<Key>().ToDictionary(k => k, k => Keyboard.IsKeyDown(_keyMapper.Map(k)));
            //var keyboardInput = new KeyboardInput(keyStates);

            //return keyboardInput;

            return KeyboardInput.CreateFromLimitedState(new Dictionary<Key, bool>
            {
                [Key.Up] = _up,
                [Key.Down] = _down,
                [Key.Left] = _left,
                [Key.Right] = _right
            });
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