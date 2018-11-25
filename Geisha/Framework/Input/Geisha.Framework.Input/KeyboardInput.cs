using System;
using System.Collections.Generic;

namespace Geisha.Framework.Input
{
    /// <summary>
    ///     Represents state of keyboard.
    /// </summary>
    public struct KeyboardInput
    {
        public KeyboardInput(IReadOnlyDictionary<Key, bool> keyStates)
        {
            Backspace = keyStates[Key.Backspace];
            Tab = keyStates[Key.Tab];
            Enter = keyStates[Key.Enter];
            LeftShift = keyStates[Key.LeftShift];
            RightShift = keyStates[Key.RightShift];
            LeftCtrl = keyStates[Key.LeftCtrl];
            RightCtrl = keyStates[Key.RightCtrl];
            LeftAlt = keyStates[Key.LeftAlt];
            RightAlt = keyStates[Key.RightAlt];
            Pause = keyStates[Key.Pause];
            CapsLock = keyStates[Key.CapsLock];
            Escape = keyStates[Key.Escape];
            Space = keyStates[Key.Space];
            PageUp = keyStates[Key.PageUp];
            PageDown = keyStates[Key.PageDown];
            End = keyStates[Key.End];
            Home = keyStates[Key.Home];
            Left = keyStates[Key.Left];
            Up = keyStates[Key.Up];
            Right = keyStates[Key.Right];
            Down = keyStates[Key.Down];
            Insert = keyStates[Key.Insert];
            Delete = keyStates[Key.Delete];
            D0 = keyStates[Key.D0];
            D1 = keyStates[Key.D1];
            D2 = keyStates[Key.D2];
            D3 = keyStates[Key.D3];
            D4 = keyStates[Key.D4];
            D5 = keyStates[Key.D5];
            D6 = keyStates[Key.D6];
            D7 = keyStates[Key.D7];
            D8 = keyStates[Key.D8];
            D9 = keyStates[Key.D9];
            A = keyStates[Key.A];
            B = keyStates[Key.B];
            C = keyStates[Key.C];
            D = keyStates[Key.D];
            E = keyStates[Key.E];
            F = keyStates[Key.F];
            G = keyStates[Key.G];
            H = keyStates[Key.H];
            I = keyStates[Key.I];
            J = keyStates[Key.J];
            K = keyStates[Key.K];
            L = keyStates[Key.L];
            M = keyStates[Key.M];
            N = keyStates[Key.N];
            O = keyStates[Key.O];
            P = keyStates[Key.P];
            Q = keyStates[Key.Q];
            R = keyStates[Key.R];
            S = keyStates[Key.S];
            T = keyStates[Key.T];
            U = keyStates[Key.U];
            V = keyStates[Key.V];
            W = keyStates[Key.W];
            X = keyStates[Key.X];
            Y = keyStates[Key.Y];
            Z = keyStates[Key.Z];
            NumPad0 = keyStates[Key.NumPad0];
            NumPad1 = keyStates[Key.NumPad1];
            NumPad2 = keyStates[Key.NumPad2];
            NumPad3 = keyStates[Key.NumPad3];
            NumPad4 = keyStates[Key.NumPad4];
            NumPad5 = keyStates[Key.NumPad5];
            NumPad6 = keyStates[Key.NumPad6];
            NumPad7 = keyStates[Key.NumPad7];
            NumPad8 = keyStates[Key.NumPad8];
            NumPad9 = keyStates[Key.NumPad9];
            Multiply = keyStates[Key.Multiply];
            Add = keyStates[Key.Add];
            Subtract = keyStates[Key.Subtract];
            Decimal = keyStates[Key.Decimal];
            Divide = keyStates[Key.Divide];
            F1 = keyStates[Key.F1];
            F2 = keyStates[Key.F2];
            F3 = keyStates[Key.F3];
            F4 = keyStates[Key.F4];
            F5 = keyStates[Key.F5];
            F6 = keyStates[Key.F6];
            F7 = keyStates[Key.F7];
            F8 = keyStates[Key.F8];
            F9 = keyStates[Key.F9];
            F10 = keyStates[Key.F10];
            F11 = keyStates[Key.F11];
            F12 = keyStates[Key.F12];
            NumLock = keyStates[Key.NumLock];
            ScrollLock = keyStates[Key.ScrollLock];
            Semicolon = keyStates[Key.Semicolon];
            EqualsSign = keyStates[Key.EqualsSign];
            Comma = keyStates[Key.Comma];
            Dash = keyStates[Key.Dash];
            Period = keyStates[Key.Period];
            Slash = keyStates[Key.Slash];
            Tilde = keyStates[Key.Tilde];
            OpenBrackets = keyStates[Key.OpenBrackets];
            Backslash = keyStates[Key.Backslash];
            CloseBrackets = keyStates[Key.CloseBrackets];
            Quotes = keyStates[Key.Quotes];
        }

        /// <summary>
        ///     Returns state of single keyboard key.
        /// </summary>
        /// <param name="key">Key whose state is requested.</param>
        /// <returns>True, if key is pressed; otherwise false.</returns>
        public bool this[Key key]
        {
            get
            {
                switch (key)
                {
                    case Key.Backspace:
                        return Backspace;
                    case Key.Tab:
                        return Tab;
                    case Key.Enter:
                        return Enter;
                    case Key.LeftShift:
                        return LeftShift;
                    case Key.RightShift:
                        return RightShift;
                    case Key.LeftCtrl:
                        return LeftCtrl;
                    case Key.RightCtrl:
                        return RightCtrl;
                    case Key.LeftAlt:
                        return LeftAlt;
                    case Key.RightAlt:
                        return RightAlt;
                    case Key.Pause:
                        return Pause;
                    case Key.CapsLock:
                        return CapsLock;
                    case Key.Escape:
                        return Escape;
                    case Key.Space:
                        return Space;
                    case Key.PageUp:
                        return PageUp;
                    case Key.PageDown:
                        return PageDown;
                    case Key.End:
                        return End;
                    case Key.Home:
                        return Home;
                    case Key.Left:
                        return Left;
                    case Key.Up:
                        return Up;
                    case Key.Right:
                        return Right;
                    case Key.Down:
                        return Down;
                    case Key.Insert:
                        return Insert;
                    case Key.Delete:
                        return Delete;
                    case Key.D0:
                        return D0;
                    case Key.D1:
                        return D1;
                    case Key.D2:
                        return D2;
                    case Key.D3:
                        return D3;
                    case Key.D4:
                        return D4;
                    case Key.D5:
                        return D5;
                    case Key.D6:
                        return D6;
                    case Key.D7:
                        return D7;
                    case Key.D8:
                        return D8;
                    case Key.D9:
                        return D9;
                    case Key.A:
                        return A;
                    case Key.B:
                        return B;
                    case Key.C:
                        return C;
                    case Key.D:
                        return D;
                    case Key.E:
                        return E;
                    case Key.F:
                        return F;
                    case Key.G:
                        return G;
                    case Key.H:
                        return H;
                    case Key.I:
                        return I;
                    case Key.J:
                        return J;
                    case Key.K:
                        return K;
                    case Key.L:
                        return L;
                    case Key.M:
                        return M;
                    case Key.N:
                        return N;
                    case Key.O:
                        return O;
                    case Key.P:
                        return P;
                    case Key.Q:
                        return Q;
                    case Key.R:
                        return R;
                    case Key.S:
                        return S;
                    case Key.T:
                        return T;
                    case Key.U:
                        return U;
                    case Key.V:
                        return V;
                    case Key.W:
                        return W;
                    case Key.X:
                        return X;
                    case Key.Y:
                        return Y;
                    case Key.Z:
                        return Z;
                    case Key.NumPad0:
                        return NumPad0;
                    case Key.NumPad1:
                        return NumPad1;
                    case Key.NumPad2:
                        return NumPad2;
                    case Key.NumPad3:
                        return NumPad3;
                    case Key.NumPad4:
                        return NumPad4;
                    case Key.NumPad5:
                        return NumPad5;
                    case Key.NumPad6:
                        return NumPad6;
                    case Key.NumPad7:
                        return NumPad7;
                    case Key.NumPad8:
                        return NumPad8;
                    case Key.NumPad9:
                        return NumPad9;
                    case Key.Multiply:
                        return Multiply;
                    case Key.Add:
                        return Add;
                    case Key.Subtract:
                        return Subtract;
                    case Key.Decimal:
                        return Decimal;
                    case Key.Divide:
                        return Divide;
                    case Key.F1:
                        return F1;
                    case Key.F2:
                        return F2;
                    case Key.F3:
                        return F3;
                    case Key.F4:
                        return F4;
                    case Key.F5:
                        return F5;
                    case Key.F6:
                        return F6;
                    case Key.F7:
                        return F7;
                    case Key.F8:
                        return F8;
                    case Key.F9:
                        return F9;
                    case Key.F10:
                        return F10;
                    case Key.F11:
                        return F11;
                    case Key.F12:
                        return F12;
                    case Key.NumLock:
                        return NumLock;
                    case Key.ScrollLock:
                        return ScrollLock;
                    case Key.Semicolon:
                        return Semicolon;
                    case Key.EqualsSign:
                        return EqualsSign;
                    case Key.Comma:
                        return Comma;
                    case Key.Dash:
                        return Dash;
                    case Key.Period:
                        return Period;
                    case Key.Slash:
                        return Slash;
                    case Key.Tilde:
                        return Tilde;
                    case Key.OpenBrackets:
                        return OpenBrackets;
                    case Key.Backslash:
                        return Backslash;
                    case Key.CloseBrackets:
                        return CloseBrackets;
                    case Key.Quotes:
                        return Quotes;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(key), key, null);
                }
            }
        }

        public bool Backspace { get; }
        public bool Tab { get; }
        public bool Enter { get; }
        public bool LeftShift { get; }
        public bool RightShift { get; }
        public bool LeftCtrl { get; }
        public bool RightCtrl { get; }
        public bool LeftAlt { get; }
        public bool RightAlt { get; }
        public bool Pause { get; }
        public bool CapsLock { get; }
        public bool Escape { get; }
        public bool Space { get; }
        public bool PageUp { get; }
        public bool PageDown { get; }
        public bool End { get; }
        public bool Home { get; }
        public bool Left { get; }
        public bool Up { get; }
        public bool Right { get; }
        public bool Down { get; }
        public bool Insert { get; }
        public bool Delete { get; }
        public bool D0 { get; }
        public bool D1 { get; }
        public bool D2 { get; }
        public bool D3 { get; }
        public bool D4 { get; }
        public bool D5 { get; }
        public bool D6 { get; }
        public bool D7 { get; }
        public bool D8 { get; }
        public bool D9 { get; }
        public bool A { get; }
        public bool B { get; }
        public bool C { get; }
        public bool D { get; }
        public bool E { get; }
        public bool F { get; }
        public bool G { get; }
        public bool H { get; }
        public bool I { get; }
        public bool J { get; }
        public bool K { get; }
        public bool L { get; }
        public bool M { get; }
        public bool N { get; }
        public bool O { get; }
        public bool P { get; }
        public bool Q { get; }
        public bool R { get; }
        public bool S { get; }
        public bool T { get; }
        public bool U { get; }
        public bool V { get; }
        public bool W { get; }
        public bool X { get; }
        public bool Y { get; }
        public bool Z { get; }
        public bool NumPad0 { get; }
        public bool NumPad1 { get; }
        public bool NumPad2 { get; }
        public bool NumPad3 { get; }
        public bool NumPad4 { get; }
        public bool NumPad5 { get; }
        public bool NumPad6 { get; }
        public bool NumPad7 { get; }
        public bool NumPad8 { get; }
        public bool NumPad9 { get; }
        public bool Multiply { get; }
        public bool Add { get; }
        public bool Subtract { get; }
        public bool Decimal { get; }
        public bool Divide { get; }
        public bool F1 { get; }
        public bool F2 { get; }
        public bool F3 { get; }
        public bool F4 { get; }
        public bool F5 { get; }
        public bool F6 { get; }
        public bool F7 { get; }
        public bool F8 { get; }
        public bool F9 { get; }
        public bool F10 { get; }
        public bool F11 { get; }
        public bool F12 { get; }
        public bool NumLock { get; }
        public bool ScrollLock { get; }
        public bool Semicolon { get; }
        public bool EqualsSign { get; }
        public bool Comma { get; }
        public bool Dash { get; }
        public bool Period { get; }
        public bool Slash { get; }
        public bool Tilde { get; }
        public bool OpenBrackets { get; }
        public bool Backslash { get; }
        public bool CloseBrackets { get; }
        public bool Quotes { get; }

        public static KeyboardInput CreateFromLimitedState(IReadOnlyDictionary<Key, bool> keyStates)
        {
            var allKeyStates = new Dictionary<Key, bool>
            {
                [Key.Backspace] = false,
                [Key.Tab] = false,
                [Key.Enter] = false,
                [Key.LeftShift] = false,
                [Key.RightShift] = false,
                [Key.LeftCtrl] = false,
                [Key.RightCtrl] = false,
                [Key.LeftAlt] = false,
                [Key.RightAlt] = false,
                [Key.Pause] = false,
                [Key.CapsLock] = false,
                [Key.Escape] = false,
                [Key.Space] = false,
                [Key.PageUp] = false,
                [Key.PageDown] = false,
                [Key.End] = false,
                [Key.Home] = false,
                [Key.Left] = false,
                [Key.Up] = false,
                [Key.Right] = false,
                [Key.Down] = false,
                [Key.Insert] = false,
                [Key.Delete] = false,
                [Key.D0] = false,
                [Key.D1] = false,
                [Key.D2] = false,
                [Key.D3] = false,
                [Key.D4] = false,
                [Key.D5] = false,
                [Key.D6] = false,
                [Key.D7] = false,
                [Key.D8] = false,
                [Key.D9] = false,
                [Key.A] = false,
                [Key.B] = false,
                [Key.C] = false,
                [Key.D] = false,
                [Key.E] = false,
                [Key.F] = false,
                [Key.G] = false,
                [Key.H] = false,
                [Key.I] = false,
                [Key.J] = false,
                [Key.K] = false,
                [Key.L] = false,
                [Key.M] = false,
                [Key.N] = false,
                [Key.O] = false,
                [Key.P] = false,
                [Key.Q] = false,
                [Key.R] = false,
                [Key.S] = false,
                [Key.T] = false,
                [Key.U] = false,
                [Key.V] = false,
                [Key.W] = false,
                [Key.X] = false,
                [Key.Y] = false,
                [Key.Z] = false,
                [Key.NumPad0] = false,
                [Key.NumPad1] = false,
                [Key.NumPad2] = false,
                [Key.NumPad3] = false,
                [Key.NumPad4] = false,
                [Key.NumPad5] = false,
                [Key.NumPad6] = false,
                [Key.NumPad7] = false,
                [Key.NumPad8] = false,
                [Key.NumPad9] = false,
                [Key.Multiply] = false,
                [Key.Add] = false,
                [Key.Subtract] = false,
                [Key.Decimal] = false,
                [Key.Divide] = false,
                [Key.F1] = false,
                [Key.F2] = false,
                [Key.F3] = false,
                [Key.F4] = false,
                [Key.F5] = false,
                [Key.F6] = false,
                [Key.F7] = false,
                [Key.F8] = false,
                [Key.F9] = false,
                [Key.F10] = false,
                [Key.F11] = false,
                [Key.F12] = false,
                [Key.NumLock] = false,
                [Key.ScrollLock] = false,
                [Key.Semicolon] = false,
                [Key.EqualsSign] = false,
                [Key.Comma] = false,
                [Key.Dash] = false,
                [Key.Period] = false,
                [Key.Slash] = false,
                [Key.Tilde] = false,
                [Key.OpenBrackets] = false,
                [Key.Backslash] = false,
                [Key.CloseBrackets] = false,
                [Key.Quotes] = false
            };

            foreach (var element in keyStates)
            {
                allKeyStates[element.Key] = element.Value;
            }

            return new KeyboardInput(allKeyStates);
        }
    }
}