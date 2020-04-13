using System;

namespace Geisha.Engine.Input
{
    /// <summary>
    ///     Represents state of the keyboard.
    /// </summary>
    /// <remarks>
    ///     Value <c>true</c> of key state means the key is in pressed state. Value <c>false</c> of key state means the
    ///     key is in released state.
    /// </remarks>
    public readonly struct KeyboardInput
    {
        /// <summary>
        ///     Creates new instance of <see cref="KeyboardInput" /> struct initialized as defined by
        ///     <see cref="KeyboardInputBuilder" /> instance.
        /// </summary>
        /// <param name="keyboardInputBuilder">
        ///     <see cref="KeyboardInputBuilder" /> instance that defines state of all keyboard keys.
        /// </param>
        public KeyboardInput(KeyboardInputBuilder keyboardInputBuilder)
        {
            Backspace = keyboardInputBuilder.Backspace;
            Tab = keyboardInputBuilder.Tab;
            Enter = keyboardInputBuilder.Enter;
            LeftShift = keyboardInputBuilder.LeftShift;
            RightShift = keyboardInputBuilder.RightShift;
            LeftCtrl = keyboardInputBuilder.LeftCtrl;
            RightCtrl = keyboardInputBuilder.RightCtrl;
            LeftAlt = keyboardInputBuilder.LeftAlt;
            RightAlt = keyboardInputBuilder.RightAlt;
            Pause = keyboardInputBuilder.Pause;
            CapsLock = keyboardInputBuilder.CapsLock;
            Escape = keyboardInputBuilder.Escape;
            Space = keyboardInputBuilder.Space;
            PageUp = keyboardInputBuilder.PageUp;
            PageDown = keyboardInputBuilder.PageDown;
            End = keyboardInputBuilder.End;
            Home = keyboardInputBuilder.Home;
            Left = keyboardInputBuilder.Left;
            Up = keyboardInputBuilder.Up;
            Right = keyboardInputBuilder.Right;
            Down = keyboardInputBuilder.Down;
            Insert = keyboardInputBuilder.Insert;
            Delete = keyboardInputBuilder.Delete;
            D0 = keyboardInputBuilder.D0;
            D1 = keyboardInputBuilder.D1;
            D2 = keyboardInputBuilder.D2;
            D3 = keyboardInputBuilder.D3;
            D4 = keyboardInputBuilder.D4;
            D5 = keyboardInputBuilder.D5;
            D6 = keyboardInputBuilder.D6;
            D7 = keyboardInputBuilder.D7;
            D8 = keyboardInputBuilder.D8;
            D9 = keyboardInputBuilder.D9;
            A = keyboardInputBuilder.A;
            B = keyboardInputBuilder.B;
            C = keyboardInputBuilder.C;
            D = keyboardInputBuilder.D;
            E = keyboardInputBuilder.E;
            F = keyboardInputBuilder.F;
            G = keyboardInputBuilder.G;
            H = keyboardInputBuilder.H;
            I = keyboardInputBuilder.I;
            J = keyboardInputBuilder.J;
            K = keyboardInputBuilder.K;
            L = keyboardInputBuilder.L;
            M = keyboardInputBuilder.M;
            N = keyboardInputBuilder.N;
            O = keyboardInputBuilder.O;
            P = keyboardInputBuilder.P;
            Q = keyboardInputBuilder.Q;
            R = keyboardInputBuilder.R;
            S = keyboardInputBuilder.S;
            T = keyboardInputBuilder.T;
            U = keyboardInputBuilder.U;
            V = keyboardInputBuilder.V;
            W = keyboardInputBuilder.W;
            X = keyboardInputBuilder.X;
            Y = keyboardInputBuilder.Y;
            Z = keyboardInputBuilder.Z;
            NumPad0 = keyboardInputBuilder.NumPad0;
            NumPad1 = keyboardInputBuilder.NumPad1;
            NumPad2 = keyboardInputBuilder.NumPad2;
            NumPad3 = keyboardInputBuilder.NumPad3;
            NumPad4 = keyboardInputBuilder.NumPad4;
            NumPad5 = keyboardInputBuilder.NumPad5;
            NumPad6 = keyboardInputBuilder.NumPad6;
            NumPad7 = keyboardInputBuilder.NumPad7;
            NumPad8 = keyboardInputBuilder.NumPad8;
            NumPad9 = keyboardInputBuilder.NumPad9;
            Multiply = keyboardInputBuilder.Multiply;
            Add = keyboardInputBuilder.Add;
            Subtract = keyboardInputBuilder.Subtract;
            Decimal = keyboardInputBuilder.Decimal;
            Divide = keyboardInputBuilder.Divide;
            F1 = keyboardInputBuilder.F1;
            F2 = keyboardInputBuilder.F2;
            F3 = keyboardInputBuilder.F3;
            F4 = keyboardInputBuilder.F4;
            F5 = keyboardInputBuilder.F5;
            F6 = keyboardInputBuilder.F6;
            F7 = keyboardInputBuilder.F7;
            F8 = keyboardInputBuilder.F8;
            F9 = keyboardInputBuilder.F9;
            F10 = keyboardInputBuilder.F10;
            F11 = keyboardInputBuilder.F11;
            F12 = keyboardInputBuilder.F12;
            NumLock = keyboardInputBuilder.NumLock;
            ScrollLock = keyboardInputBuilder.ScrollLock;
            Semicolon = keyboardInputBuilder.Semicolon;
            EqualsSign = keyboardInputBuilder.EqualsSign;
            Comma = keyboardInputBuilder.Comma;
            Dash = keyboardInputBuilder.Dash;
            Period = keyboardInputBuilder.Period;
            Slash = keyboardInputBuilder.Slash;
            Tilde = keyboardInputBuilder.Tilde;
            OpenBrackets = keyboardInputBuilder.OpenBrackets;
            Backslash = keyboardInputBuilder.Backslash;
            CloseBrackets = keyboardInputBuilder.CloseBrackets;
            Quotes = keyboardInputBuilder.Quotes;
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
    }
}