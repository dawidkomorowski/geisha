using System;
using System.Collections.Generic;

namespace Geisha.Framework.Input
{
    /// <summary>
    ///     Represents state of keyboard.
    /// </summary>
    /// <remarks>
    ///     Value <c>true</c> of key state means the key is in pressed state. Value <c>false</c> of key state means the
    ///     key is in released state.
    /// </remarks>
    public struct KeyboardInput
    {
        /// <summary>
        ///     Creates new instance of <see cref="KeyboardInput" /> struct initialized as in specified dictionary of all key
        ///     states.
        /// </summary>
        /// <param name="keyStates">
        ///     Dictionary of keys and their states. State value <c>true</c> represents pressed key, while
        ///     <c>false</c> represents released key. It must contain states of all keyboard keys supported by Geisha Engine.
        /// </param>
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

        /// <summary>
        ///     State of the Backspace key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Backspace { get; }

        /// <summary>
        ///     State of the Tab key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Tab { get; }

        /// <summary>
        ///     State of the Enter key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Enter { get; }

        /// <summary>
        ///     State of the left Shift key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool LeftShift { get; }

        /// <summary>
        ///     State of the right Shift key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool RightShift { get; }

        /// <summary>
        ///     State of the left CTRL key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool LeftCtrl { get; }

        /// <summary>
        ///     State of the right CTRL key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool RightCtrl { get; }

        /// <summary>
        ///     State of the left ALT key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool LeftAlt { get; }

        /// <summary>
        ///     State of the right ALT key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool RightAlt { get; }

        /// <summary>
        ///     State of the Pause key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Pause { get; }

        /// <summary>
        ///     State of the Caps Lock key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool CapsLock { get; }

        /// <summary>
        ///     State of the ESC key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Escape { get; }

        /// <summary>
        ///     State of the Spacebar key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Space { get; }

        /// <summary>
        ///     State of the Page Up key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool PageUp { get; }

        /// <summary>
        ///     State of the Page Down key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool PageDown { get; }

        /// <summary>
        ///     State of the End key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool End { get; }

        /// <summary>
        ///     State of the Home key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Home { get; }

        /// <summary>
        ///     State of the Left Arrow key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Left { get; }

        /// <summary>
        ///     State of the Up Arrow key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Up { get; }

        /// <summary>
        ///     State of the Right Arrow key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Right { get; }

        /// <summary>
        ///     State of the Down Arrow key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Down { get; }

        /// <summary>
        ///     State of the Insert key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Insert { get; }

        /// <summary>
        ///     State of the Delete key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Delete { get; }

        /// <summary>
        ///     State of the 0 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool D0 { get; }

        /// <summary>
        ///     State of the 1 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool D1 { get; }

        /// <summary>
        ///     State of the 2 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool D2 { get; }

        /// <summary>
        ///     State of the 3 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool D3 { get; }

        /// <summary>
        ///     State of the 4 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool D4 { get; }

        /// <summary>
        ///     State of the 5 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool D5 { get; }

        /// <summary>
        ///     State of the 6 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool D6 { get; }

        /// <summary>
        ///     State of the 7 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool D7 { get; }

        /// <summary>
        ///     State of the 8 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool D8 { get; }

        /// <summary>
        ///     State of the 9 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool D9 { get; }

        /// <summary>
        ///     State of the A key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool A { get; }

        /// <summary>
        ///     State of the B key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool B { get; }

        /// <summary>
        ///     State of the C key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool C { get; }

        /// <summary>
        ///     State of the D key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool D { get; }

        /// <summary>
        ///     State of the E key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool E { get; }

        /// <summary>
        ///     State of the F key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool F { get; }

        /// <summary>
        ///     State of the G key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool G { get; }

        /// <summary>
        ///     State of the H key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool H { get; }

        /// <summary>
        ///     State of the I key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool I { get; }

        /// <summary>
        ///     State of the J key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool J { get; }

        /// <summary>
        ///     State of the K key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool K { get; }

        /// <summary>
        ///     State of the L key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool L { get; }

        /// <summary>
        ///     State of the M key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool M { get; }

        /// <summary>
        ///     State of the N key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool N { get; }

        /// <summary>
        ///     State of the O key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool O { get; }

        /// <summary>
        ///     State of the P key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool P { get; }

        /// <summary>
        ///     State of the Q key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Q { get; }

        /// <summary>
        ///     State of the R key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool R { get; }

        /// <summary>
        ///     State of the S key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool S { get; }

        /// <summary>
        ///     State of the T key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool T { get; }

        /// <summary>
        ///     State of the U key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool U { get; }

        /// <summary>
        ///     State of the V key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool V { get; }

        /// <summary>
        ///     State of the W key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool W { get; }

        /// <summary>
        ///     State of the X key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool X { get; }

        /// <summary>
        ///     State of the Y key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Y { get; }

        /// <summary>
        ///     State of the Z key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Z { get; }

        /// <summary>
        ///     State of the 0 key on the numeric keypad. <c>true</c> indicates pressed state, while <c>false</c> indicates
        ///     released state.
        /// </summary>
        public bool NumPad0 { get; }

        /// <summary>
        ///     State of the 1 key on the numeric keypad. <c>true</c> indicates pressed state, while <c>false</c> indicates
        ///     released state.
        /// </summary>
        public bool NumPad1 { get; }

        /// <summary>
        ///     State of the 2 key on the numeric keypad. <c>true</c> indicates pressed state, while <c>false</c> indicates
        ///     released state.
        /// </summary>
        public bool NumPad2 { get; }

        /// <summary>
        ///     State of the 3 key on the numeric keypad. <c>true</c> indicates pressed state, while <c>false</c> indicates
        ///     released state.
        /// </summary>
        public bool NumPad3 { get; }

        /// <summary>
        ///     State of the 4 key on the numeric keypad. <c>true</c> indicates pressed state, while <c>false</c> indicates
        ///     released state.
        /// </summary>
        public bool NumPad4 { get; }

        /// <summary>
        ///     State of the 5 key on the numeric keypad. <c>true</c> indicates pressed state, while <c>false</c> indicates
        ///     released state.
        /// </summary>
        public bool NumPad5 { get; }

        /// <summary>
        ///     State of the 6 key on the numeric keypad. <c>true</c> indicates pressed state, while <c>false</c> indicates
        ///     released state.
        /// </summary>
        public bool NumPad6 { get; }

        /// <summary>
        ///     State of the 7 key on the numeric keypad. <c>true</c> indicates pressed state, while <c>false</c> indicates
        ///     released state.
        /// </summary>
        public bool NumPad7 { get; }

        /// <summary>
        ///     State of the 8 key on the numeric keypad. <c>true</c> indicates pressed state, while <c>false</c> indicates
        ///     released state.
        /// </summary>
        public bool NumPad8 { get; }

        /// <summary>
        ///     State of the 9 key on the numeric keypad. <c>true</c> indicates pressed state, while <c>false</c> indicates
        ///     released state.
        /// </summary>
        public bool NumPad9 { get; }

        /// <summary>
        ///     State of the Multiply key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Multiply { get; }

        /// <summary>
        ///     State of the Add key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Add { get; }

        /// <summary>
        ///     State of the Subtract key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Subtract { get; }

        /// <summary>
        ///     State of the Decimal key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Decimal { get; }

        /// <summary>
        ///     State of the Divide key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Divide { get; }

        /// <summary>
        ///     State of the F1 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool F1 { get; }

        /// <summary>
        ///     State of the F2 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool F2 { get; }

        /// <summary>
        ///     State of the F3 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool F3 { get; }

        /// <summary>
        ///     State of the F4 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool F4 { get; }

        /// <summary>
        ///     State of the F5 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool F5 { get; }

        /// <summary>
        ///     State of the F6 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool F6 { get; }

        /// <summary>
        ///     State of the F7 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool F7 { get; }

        /// <summary>
        ///     State of the F8 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool F8 { get; }

        /// <summary>
        ///     State of the F9 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool F9 { get; }

        /// <summary>
        ///     State of the F10 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool F10 { get; }

        /// <summary>
        ///     State of the F11 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool F11 { get; }

        /// <summary>
        ///     State of the F12 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool F12 { get; }

        /// <summary>
        ///     State of the Num Lock key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool NumLock { get; }

        /// <summary>
        ///     State of the Scroll Lock key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool ScrollLock { get; }

        /// <summary>
        ///     State of the Semicolon key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Semicolon { get; }

        /// <summary>
        ///     State of the Equals Sign key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool EqualsSign { get; }

        /// <summary>
        ///     State of the Comma key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Comma { get; }

        /// <summary>
        ///     State of the Dash key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Dash { get; }

        /// <summary>
        ///     State of the Period key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Period { get; }

        /// <summary>
        ///     State of the Slash key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Slash { get; }

        /// <summary>
        ///     State of the Tilde key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Tilde { get; }

        /// <summary>
        ///     State of the Open Brackets key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool OpenBrackets { get; }

        /// <summary>
        ///     State of the Backslash key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Backslash { get; }

        /// <summary>
        ///     State of the Close Brackets key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool CloseBrackets { get; }

        /// <summary>
        ///     State of the Quotes key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Quotes { get; }

        /// <summary>
        ///     Creates new instance of <see cref="KeyboardInput" /> struct initialized as in specified dictionary of selected key
        ///     states.
        /// </summary>
        /// <param name="keyStates">
        ///     Dictionary of keys and their states. State value <c>true</c> represents pressed key, while
        ///     <c>false</c> represents released key. It allows to specify only selected keys states, while rest will be
        ///     <c>false</c> by default.
        /// </param>
        /// <returns></returns>
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