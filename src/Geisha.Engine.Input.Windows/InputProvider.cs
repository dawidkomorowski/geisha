﻿using System.Windows.Forms;

namespace Geisha.Engine.Input.Windows
{
    internal class InputProvider : IInputProvider
    {
        private readonly Form _form;

        public InputProvider(Form form)
        {
            _form = form;
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

            return default;
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