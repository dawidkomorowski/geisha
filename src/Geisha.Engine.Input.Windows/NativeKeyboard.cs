using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Geisha.Engine.Input.Windows
{
    internal static class NativeKeyboard
    {
        public static void SetBuilderToCurrentState(KeyboardInputBuilder keyboardInputBuilder)
        {
            unsafe
            {
                var keyboardStateBuffer = stackalloc byte[256];
                if (GetKeyboardState(keyboardStateBuffer) == false)
                {
                    var error = Marshal.GetLastWin32Error();
                    throw new Win32Exception(error);
                }

                keyboardInputBuilder.Backspace = GetKeyState(keyboardStateBuffer, Keys.Back);
                keyboardInputBuilder.Tab = GetKeyState(keyboardStateBuffer, Keys.Tab);
                keyboardInputBuilder.Enter = GetKeyState(keyboardStateBuffer, Keys.Enter);
                keyboardInputBuilder.LeftShift = GetKeyState(keyboardStateBuffer, Keys.LShiftKey);
                keyboardInputBuilder.RightShift = GetKeyState(keyboardStateBuffer, Keys.RShiftKey);
                keyboardInputBuilder.LeftCtrl = GetKeyState(keyboardStateBuffer, Keys.LControlKey);
                keyboardInputBuilder.RightCtrl = GetKeyState(keyboardStateBuffer, Keys.RControlKey);
                keyboardInputBuilder.LeftAlt = GetKeyState(keyboardStateBuffer, Keys.LMenu);
                keyboardInputBuilder.RightAlt = GetKeyState(keyboardStateBuffer, Keys.RMenu);
                keyboardInputBuilder.Pause = GetKeyState(keyboardStateBuffer, Keys.Pause);
                keyboardInputBuilder.CapsLock = GetKeyState(keyboardStateBuffer, Keys.CapsLock);
                keyboardInputBuilder.Escape = GetKeyState(keyboardStateBuffer, Keys.Escape);
                keyboardInputBuilder.Space = GetKeyState(keyboardStateBuffer, Keys.Space);
                keyboardInputBuilder.PageUp = GetKeyState(keyboardStateBuffer, Keys.PageUp);
                keyboardInputBuilder.PageDown = GetKeyState(keyboardStateBuffer, Keys.PageDown);
                keyboardInputBuilder.End = GetKeyState(keyboardStateBuffer, Keys.End);
                keyboardInputBuilder.Home = GetKeyState(keyboardStateBuffer, Keys.Home);
                keyboardInputBuilder.Left = GetKeyState(keyboardStateBuffer, Keys.Left);
                keyboardInputBuilder.Up = GetKeyState(keyboardStateBuffer, Keys.Up);
                keyboardInputBuilder.Right = GetKeyState(keyboardStateBuffer, Keys.Right);
                keyboardInputBuilder.Down = GetKeyState(keyboardStateBuffer, Keys.Down);
                keyboardInputBuilder.Insert = GetKeyState(keyboardStateBuffer, Keys.Insert);
                keyboardInputBuilder.Delete = GetKeyState(keyboardStateBuffer, Keys.Delete);
                keyboardInputBuilder.D0 = GetKeyState(keyboardStateBuffer, Keys.D0);
                keyboardInputBuilder.D1 = GetKeyState(keyboardStateBuffer, Keys.D1);
                keyboardInputBuilder.D2 = GetKeyState(keyboardStateBuffer, Keys.D2);
                keyboardInputBuilder.D3 = GetKeyState(keyboardStateBuffer, Keys.D3);
                keyboardInputBuilder.D4 = GetKeyState(keyboardStateBuffer, Keys.D4);
                keyboardInputBuilder.D5 = GetKeyState(keyboardStateBuffer, Keys.D5);
                keyboardInputBuilder.D6 = GetKeyState(keyboardStateBuffer, Keys.D6);
                keyboardInputBuilder.D7 = GetKeyState(keyboardStateBuffer, Keys.D7);
                keyboardInputBuilder.D8 = GetKeyState(keyboardStateBuffer, Keys.D8);
                keyboardInputBuilder.D9 = GetKeyState(keyboardStateBuffer, Keys.D9);
                keyboardInputBuilder.A = GetKeyState(keyboardStateBuffer, Keys.A);
                keyboardInputBuilder.B = GetKeyState(keyboardStateBuffer, Keys.B);
                keyboardInputBuilder.C = GetKeyState(keyboardStateBuffer, Keys.C);
                keyboardInputBuilder.D = GetKeyState(keyboardStateBuffer, Keys.D);
                keyboardInputBuilder.E = GetKeyState(keyboardStateBuffer, Keys.E);
                keyboardInputBuilder.F = GetKeyState(keyboardStateBuffer, Keys.F);
                keyboardInputBuilder.G = GetKeyState(keyboardStateBuffer, Keys.G);
                keyboardInputBuilder.H = GetKeyState(keyboardStateBuffer, Keys.H);
                keyboardInputBuilder.I = GetKeyState(keyboardStateBuffer, Keys.I);
                keyboardInputBuilder.J = GetKeyState(keyboardStateBuffer, Keys.J);
                keyboardInputBuilder.K = GetKeyState(keyboardStateBuffer, Keys.K);
                keyboardInputBuilder.L = GetKeyState(keyboardStateBuffer, Keys.L);
                keyboardInputBuilder.M = GetKeyState(keyboardStateBuffer, Keys.M);
                keyboardInputBuilder.N = GetKeyState(keyboardStateBuffer, Keys.N);
                keyboardInputBuilder.O = GetKeyState(keyboardStateBuffer, Keys.O);
                keyboardInputBuilder.P = GetKeyState(keyboardStateBuffer, Keys.P);
                keyboardInputBuilder.Q = GetKeyState(keyboardStateBuffer, Keys.Q);
                keyboardInputBuilder.R = GetKeyState(keyboardStateBuffer, Keys.R);
                keyboardInputBuilder.S = GetKeyState(keyboardStateBuffer, Keys.S);
                keyboardInputBuilder.T = GetKeyState(keyboardStateBuffer, Keys.T);
                keyboardInputBuilder.U = GetKeyState(keyboardStateBuffer, Keys.U);
                keyboardInputBuilder.V = GetKeyState(keyboardStateBuffer, Keys.V);
                keyboardInputBuilder.W = GetKeyState(keyboardStateBuffer, Keys.W);
                keyboardInputBuilder.X = GetKeyState(keyboardStateBuffer, Keys.X);
                keyboardInputBuilder.Y = GetKeyState(keyboardStateBuffer, Keys.Y);
                keyboardInputBuilder.Z = GetKeyState(keyboardStateBuffer, Keys.Z);
                keyboardInputBuilder.NumPad0 = GetKeyState(keyboardStateBuffer, Keys.NumPad0);
                keyboardInputBuilder.NumPad1 = GetKeyState(keyboardStateBuffer, Keys.NumPad1);
                keyboardInputBuilder.NumPad2 = GetKeyState(keyboardStateBuffer, Keys.NumPad2);
                keyboardInputBuilder.NumPad3 = GetKeyState(keyboardStateBuffer, Keys.NumPad3);
                keyboardInputBuilder.NumPad4 = GetKeyState(keyboardStateBuffer, Keys.NumPad4);
                keyboardInputBuilder.NumPad5 = GetKeyState(keyboardStateBuffer, Keys.NumPad5);
                keyboardInputBuilder.NumPad6 = GetKeyState(keyboardStateBuffer, Keys.NumPad6);
                keyboardInputBuilder.NumPad7 = GetKeyState(keyboardStateBuffer, Keys.NumPad7);
                keyboardInputBuilder.NumPad8 = GetKeyState(keyboardStateBuffer, Keys.NumPad8);
                keyboardInputBuilder.NumPad9 = GetKeyState(keyboardStateBuffer, Keys.NumPad9);
                keyboardInputBuilder.Multiply = GetKeyState(keyboardStateBuffer, Keys.Multiply);
                keyboardInputBuilder.Add = GetKeyState(keyboardStateBuffer, Keys.Add);
                keyboardInputBuilder.Subtract = GetKeyState(keyboardStateBuffer, Keys.Subtract);
                keyboardInputBuilder.Decimal = GetKeyState(keyboardStateBuffer, Keys.Decimal);
                keyboardInputBuilder.Divide = GetKeyState(keyboardStateBuffer, Keys.Divide);
                keyboardInputBuilder.F1 = GetKeyState(keyboardStateBuffer, Keys.F1);
                keyboardInputBuilder.F2 = GetKeyState(keyboardStateBuffer, Keys.F2);
                keyboardInputBuilder.F3 = GetKeyState(keyboardStateBuffer, Keys.F3);
                keyboardInputBuilder.F4 = GetKeyState(keyboardStateBuffer, Keys.F4);
                keyboardInputBuilder.F5 = GetKeyState(keyboardStateBuffer, Keys.F5);
                keyboardInputBuilder.F6 = GetKeyState(keyboardStateBuffer, Keys.F6);
                keyboardInputBuilder.F7 = GetKeyState(keyboardStateBuffer, Keys.F7);
                keyboardInputBuilder.F8 = GetKeyState(keyboardStateBuffer, Keys.F8);
                keyboardInputBuilder.F9 = GetKeyState(keyboardStateBuffer, Keys.F9);
                keyboardInputBuilder.F10 = GetKeyState(keyboardStateBuffer, Keys.F10);
                keyboardInputBuilder.F11 = GetKeyState(keyboardStateBuffer, Keys.F11);
                keyboardInputBuilder.F12 = GetKeyState(keyboardStateBuffer, Keys.F12);
                keyboardInputBuilder.NumLock = GetKeyState(keyboardStateBuffer, Keys.NumLock);
                keyboardInputBuilder.ScrollLock = GetKeyState(keyboardStateBuffer, Keys.Scroll);
                keyboardInputBuilder.Semicolon = GetKeyState(keyboardStateBuffer, Keys.OemSemicolon);
                keyboardInputBuilder.EqualsSign = GetKeyState(keyboardStateBuffer, Keys.Oemplus);
                keyboardInputBuilder.Comma = GetKeyState(keyboardStateBuffer, Keys.Oemcomma);
                keyboardInputBuilder.Dash = GetKeyState(keyboardStateBuffer, Keys.OemMinus);
                keyboardInputBuilder.Period = GetKeyState(keyboardStateBuffer, Keys.OemPeriod);
                keyboardInputBuilder.Slash = GetKeyState(keyboardStateBuffer, Keys.OemQuestion);
                keyboardInputBuilder.Tilde = GetKeyState(keyboardStateBuffer, Keys.Oemtilde);
                keyboardInputBuilder.OpenBrackets = GetKeyState(keyboardStateBuffer, Keys.OemOpenBrackets);
                keyboardInputBuilder.Backslash = GetKeyState(keyboardStateBuffer, Keys.OemPipe);
                keyboardInputBuilder.CloseBrackets = GetKeyState(keyboardStateBuffer, Keys.OemCloseBrackets);
                keyboardInputBuilder.Quotes = GetKeyState(keyboardStateBuffer, Keys.OemQuotes);
            }
        }

        private static unsafe bool GetKeyState(byte* keyboardStateBuffer, Keys key)
        {
            // Bitwise AND clears lower bits to ignore toggle state for NumLock, CapsLock, ScrollLock and only use upper bits for pressed state.
            return (keyboardStateBuffer[(int) key] & 0b11110000) != 0;
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern unsafe bool GetKeyboardState(byte* lpKeyState);
    }
}