﻿using System;
using Geisha.Engine.Input;

namespace Geisha.Framework.Input.Wpf
{
    internal class KeyMapper
    {
        public System.Windows.Input.Key Map(Key key)
        {
            switch (key)
            {
                case Key.Backspace:
                    return System.Windows.Input.Key.Back;
                case Key.Tab:
                    return System.Windows.Input.Key.Tab;
                case Key.Enter:
                    return System.Windows.Input.Key.Enter;
                case Key.LeftShift:
                    return System.Windows.Input.Key.LeftShift;
                case Key.RightShift:
                    return System.Windows.Input.Key.RightShift;
                case Key.LeftCtrl:
                    return System.Windows.Input.Key.LeftCtrl;
                case Key.RightCtrl:
                    return System.Windows.Input.Key.RightCtrl;
                case Key.LeftAlt:
                    return System.Windows.Input.Key.LeftAlt;
                case Key.RightAlt:
                    return System.Windows.Input.Key.RightAlt;
                case Key.Pause:
                    return System.Windows.Input.Key.Pause;
                case Key.CapsLock:
                    return System.Windows.Input.Key.CapsLock;
                case Key.Escape:
                    return System.Windows.Input.Key.Escape;
                case Key.Space:
                    return System.Windows.Input.Key.Space;
                case Key.PageUp:
                    return System.Windows.Input.Key.PageUp;
                case Key.PageDown:
                    return System.Windows.Input.Key.PageDown;
                case Key.End:
                    return System.Windows.Input.Key.End;
                case Key.Home:
                    return System.Windows.Input.Key.Home;
                case Key.Left:
                    return System.Windows.Input.Key.Left;
                case Key.Up:
                    return System.Windows.Input.Key.Up;
                case Key.Right:
                    return System.Windows.Input.Key.Right;
                case Key.Down:
                    return System.Windows.Input.Key.Down;
                case Key.Insert:
                    return System.Windows.Input.Key.Insert;
                case Key.Delete:
                    return System.Windows.Input.Key.Delete;
                case Key.D0:
                    return System.Windows.Input.Key.D0;
                case Key.D1:
                    return System.Windows.Input.Key.D1;
                case Key.D2:
                    return System.Windows.Input.Key.D2;
                case Key.D3:
                    return System.Windows.Input.Key.D3;
                case Key.D4:
                    return System.Windows.Input.Key.D4;
                case Key.D5:
                    return System.Windows.Input.Key.D5;
                case Key.D6:
                    return System.Windows.Input.Key.D6;
                case Key.D7:
                    return System.Windows.Input.Key.D7;
                case Key.D8:
                    return System.Windows.Input.Key.D8;
                case Key.D9:
                    return System.Windows.Input.Key.D9;
                case Key.A:
                    return System.Windows.Input.Key.A;
                case Key.B:
                    return System.Windows.Input.Key.B;
                case Key.C:
                    return System.Windows.Input.Key.C;
                case Key.D:
                    return System.Windows.Input.Key.D;
                case Key.E:
                    return System.Windows.Input.Key.E;
                case Key.F:
                    return System.Windows.Input.Key.F;
                case Key.G:
                    return System.Windows.Input.Key.G;
                case Key.H:
                    return System.Windows.Input.Key.H;
                case Key.I:
                    return System.Windows.Input.Key.I;
                case Key.J:
                    return System.Windows.Input.Key.J;
                case Key.K:
                    return System.Windows.Input.Key.K;
                case Key.L:
                    return System.Windows.Input.Key.L;
                case Key.M:
                    return System.Windows.Input.Key.M;
                case Key.N:
                    return System.Windows.Input.Key.N;
                case Key.O:
                    return System.Windows.Input.Key.O;
                case Key.P:
                    return System.Windows.Input.Key.P;
                case Key.Q:
                    return System.Windows.Input.Key.Q;
                case Key.R:
                    return System.Windows.Input.Key.R;
                case Key.S:
                    return System.Windows.Input.Key.S;
                case Key.T:
                    return System.Windows.Input.Key.T;
                case Key.U:
                    return System.Windows.Input.Key.U;
                case Key.V:
                    return System.Windows.Input.Key.V;
                case Key.W:
                    return System.Windows.Input.Key.W;
                case Key.X:
                    return System.Windows.Input.Key.X;
                case Key.Y:
                    return System.Windows.Input.Key.Y;
                case Key.Z:
                    return System.Windows.Input.Key.Z;
                case Key.NumPad0:
                    return System.Windows.Input.Key.NumPad0;
                case Key.NumPad1:
                    return System.Windows.Input.Key.NumPad1;
                case Key.NumPad2:
                    return System.Windows.Input.Key.NumPad2;
                case Key.NumPad3:
                    return System.Windows.Input.Key.NumPad3;
                case Key.NumPad4:
                    return System.Windows.Input.Key.NumPad4;
                case Key.NumPad5:
                    return System.Windows.Input.Key.NumPad5;
                case Key.NumPad6:
                    return System.Windows.Input.Key.NumPad6;
                case Key.NumPad7:
                    return System.Windows.Input.Key.NumPad7;
                case Key.NumPad8:
                    return System.Windows.Input.Key.NumPad8;
                case Key.NumPad9:
                    return System.Windows.Input.Key.NumPad9;
                case Key.Multiply:
                    return System.Windows.Input.Key.Multiply;
                case Key.Add:
                    return System.Windows.Input.Key.Add;
                case Key.Subtract:
                    return System.Windows.Input.Key.Subtract;
                case Key.Decimal:
                    return System.Windows.Input.Key.Decimal;
                case Key.Divide:
                    return System.Windows.Input.Key.Divide;
                case Key.F1:
                    return System.Windows.Input.Key.F1;
                case Key.F2:
                    return System.Windows.Input.Key.F2;
                case Key.F3:
                    return System.Windows.Input.Key.F3;
                case Key.F4:
                    return System.Windows.Input.Key.F4;
                case Key.F5:
                    return System.Windows.Input.Key.F5;
                case Key.F6:
                    return System.Windows.Input.Key.F6;
                case Key.F7:
                    return System.Windows.Input.Key.F7;
                case Key.F8:
                    return System.Windows.Input.Key.F8;
                case Key.F9:
                    return System.Windows.Input.Key.F9;
                case Key.F10:
                    return System.Windows.Input.Key.F10;
                case Key.F11:
                    return System.Windows.Input.Key.F11;
                case Key.F12:
                    return System.Windows.Input.Key.F12;
                case Key.NumLock:
                    return System.Windows.Input.Key.NumLock;
                case Key.ScrollLock:
                    return System.Windows.Input.Key.Scroll;
                case Key.Semicolon:
                    return System.Windows.Input.Key.OemSemicolon;
                case Key.EqualsSign:
                    return System.Windows.Input.Key.OemPlus;
                case Key.Comma:
                    return System.Windows.Input.Key.OemComma;
                case Key.Dash:
                    return System.Windows.Input.Key.OemMinus;
                case Key.Period:
                    return System.Windows.Input.Key.OemPeriod;
                case Key.Slash:
                    return System.Windows.Input.Key.OemQuestion;
                case Key.Tilde:
                    return System.Windows.Input.Key.OemTilde;
                case Key.OpenBrackets:
                    return System.Windows.Input.Key.OemOpenBrackets;
                case Key.Backslash:
                    return System.Windows.Input.Key.OemPipe;
                case Key.CloseBrackets:
                    return System.Windows.Input.Key.OemCloseBrackets;
                case Key.Quotes:
                    return System.Windows.Input.Key.OemQuotes;
                default:
                    throw new ArgumentOutOfRangeException(nameof(key), key, null);
            }
        }
    }
}