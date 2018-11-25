using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Geisha.Framework.Input.UnitTests
{
    [TestFixture]
    public class KeyboardInputTests
    {
        private Dictionary<Key, bool> _keyStates;

        [SetUp]
        public void SetUp()
        {
            _keyStates = new Dictionary<Key, bool>
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
        }

        private static IEnumerable<TestCase> TestCases => new[]
        {
            new TestCase(Key.Backspace, keyboardInput => keyboardInput.Backspace),
            new TestCase(Key.Tab, keyboardInput => keyboardInput.Tab),
            new TestCase(Key.Enter, keyboardInput => keyboardInput.Enter),
            new TestCase(Key.LeftShift, keyboardInput => keyboardInput.LeftShift),
            new TestCase(Key.RightShift, keyboardInput => keyboardInput.RightShift),
            new TestCase(Key.LeftCtrl, keyboardInput => keyboardInput.LeftCtrl),
            new TestCase(Key.RightCtrl, keyboardInput => keyboardInput.RightCtrl),
            new TestCase(Key.LeftAlt, keyboardInput => keyboardInput.LeftAlt),
            new TestCase(Key.RightAlt, keyboardInput => keyboardInput.RightAlt),
            new TestCase(Key.Pause, keyboardInput => keyboardInput.Pause),
            new TestCase(Key.CapsLock, keyboardInput => keyboardInput.CapsLock),
            new TestCase(Key.Escape, keyboardInput => keyboardInput.Escape),
            new TestCase(Key.Space, keyboardInput => keyboardInput.Space),
            new TestCase(Key.PageUp, keyboardInput => keyboardInput.PageUp),
            new TestCase(Key.PageDown, keyboardInput => keyboardInput.PageDown),
            new TestCase(Key.End, keyboardInput => keyboardInput.End),
            new TestCase(Key.Home, keyboardInput => keyboardInput.Home),
            new TestCase(Key.Left, keyboardInput => keyboardInput.Left),
            new TestCase(Key.Up, keyboardInput => keyboardInput.Up),
            new TestCase(Key.Right, keyboardInput => keyboardInput.Right),
            new TestCase(Key.Down, keyboardInput => keyboardInput.Down),
            new TestCase(Key.Insert, keyboardInput => keyboardInput.Insert),
            new TestCase(Key.Delete, keyboardInput => keyboardInput.Delete),
            new TestCase(Key.D0, keyboardInput => keyboardInput.D0),
            new TestCase(Key.D1, keyboardInput => keyboardInput.D1),
            new TestCase(Key.D2, keyboardInput => keyboardInput.D2),
            new TestCase(Key.D3, keyboardInput => keyboardInput.D3),
            new TestCase(Key.D4, keyboardInput => keyboardInput.D4),
            new TestCase(Key.D5, keyboardInput => keyboardInput.D5),
            new TestCase(Key.D6, keyboardInput => keyboardInput.D6),
            new TestCase(Key.D7, keyboardInput => keyboardInput.D7),
            new TestCase(Key.D8, keyboardInput => keyboardInput.D8),
            new TestCase(Key.D9, keyboardInput => keyboardInput.D9),
            new TestCase(Key.A, keyboardInput => keyboardInput.A),
            new TestCase(Key.B, keyboardInput => keyboardInput.B),
            new TestCase(Key.C, keyboardInput => keyboardInput.C),
            new TestCase(Key.D, keyboardInput => keyboardInput.D),
            new TestCase(Key.E, keyboardInput => keyboardInput.E),
            new TestCase(Key.F, keyboardInput => keyboardInput.F),
            new TestCase(Key.G, keyboardInput => keyboardInput.G),
            new TestCase(Key.H, keyboardInput => keyboardInput.H),
            new TestCase(Key.I, keyboardInput => keyboardInput.I),
            new TestCase(Key.J, keyboardInput => keyboardInput.J),
            new TestCase(Key.K, keyboardInput => keyboardInput.K),
            new TestCase(Key.L, keyboardInput => keyboardInput.L),
            new TestCase(Key.M, keyboardInput => keyboardInput.M),
            new TestCase(Key.N, keyboardInput => keyboardInput.N),
            new TestCase(Key.O, keyboardInput => keyboardInput.O),
            new TestCase(Key.P, keyboardInput => keyboardInput.P),
            new TestCase(Key.Q, keyboardInput => keyboardInput.Q),
            new TestCase(Key.R, keyboardInput => keyboardInput.R),
            new TestCase(Key.S, keyboardInput => keyboardInput.S),
            new TestCase(Key.T, keyboardInput => keyboardInput.T),
            new TestCase(Key.U, keyboardInput => keyboardInput.U),
            new TestCase(Key.V, keyboardInput => keyboardInput.V),
            new TestCase(Key.W, keyboardInput => keyboardInput.W),
            new TestCase(Key.X, keyboardInput => keyboardInput.X),
            new TestCase(Key.Y, keyboardInput => keyboardInput.Y),
            new TestCase(Key.Z, keyboardInput => keyboardInput.Z),
            new TestCase(Key.NumPad0, keyboardInput => keyboardInput.NumPad0),
            new TestCase(Key.NumPad1, keyboardInput => keyboardInput.NumPad1),
            new TestCase(Key.NumPad2, keyboardInput => keyboardInput.NumPad2),
            new TestCase(Key.NumPad3, keyboardInput => keyboardInput.NumPad3),
            new TestCase(Key.NumPad4, keyboardInput => keyboardInput.NumPad4),
            new TestCase(Key.NumPad5, keyboardInput => keyboardInput.NumPad5),
            new TestCase(Key.NumPad6, keyboardInput => keyboardInput.NumPad6),
            new TestCase(Key.NumPad7, keyboardInput => keyboardInput.NumPad7),
            new TestCase(Key.NumPad8, keyboardInput => keyboardInput.NumPad8),
            new TestCase(Key.NumPad9, keyboardInput => keyboardInput.NumPad9),
            new TestCase(Key.Multiply, keyboardInput => keyboardInput.Multiply),
            new TestCase(Key.Add, keyboardInput => keyboardInput.Add),
            new TestCase(Key.Subtract, keyboardInput => keyboardInput.Subtract),
            new TestCase(Key.Decimal, keyboardInput => keyboardInput.Decimal),
            new TestCase(Key.Divide, keyboardInput => keyboardInput.Divide),
            new TestCase(Key.F1, keyboardInput => keyboardInput.F1),
            new TestCase(Key.F2, keyboardInput => keyboardInput.F2),
            new TestCase(Key.F3, keyboardInput => keyboardInput.F3),
            new TestCase(Key.F4, keyboardInput => keyboardInput.F4),
            new TestCase(Key.F5, keyboardInput => keyboardInput.F5),
            new TestCase(Key.F6, keyboardInput => keyboardInput.F6),
            new TestCase(Key.F7, keyboardInput => keyboardInput.F7),
            new TestCase(Key.F8, keyboardInput => keyboardInput.F8),
            new TestCase(Key.F9, keyboardInput => keyboardInput.F9),
            new TestCase(Key.F10, keyboardInput => keyboardInput.F10),
            new TestCase(Key.F11, keyboardInput => keyboardInput.F11),
            new TestCase(Key.F12, keyboardInput => keyboardInput.F12),
            new TestCase(Key.NumLock, keyboardInput => keyboardInput.NumLock),
            new TestCase(Key.ScrollLock, keyboardInput => keyboardInput.ScrollLock),
            new TestCase(Key.Semicolon, keyboardInput => keyboardInput.Semicolon),
            new TestCase(Key.EqualsSign, keyboardInput => keyboardInput.EqualsSign),
            new TestCase(Key.Comma, keyboardInput => keyboardInput.Comma),
            new TestCase(Key.Dash, keyboardInput => keyboardInput.Dash),
            new TestCase(Key.Period, keyboardInput => keyboardInput.Period),
            new TestCase(Key.Slash, keyboardInput => keyboardInput.Slash),
            new TestCase(Key.Tilde, keyboardInput => keyboardInput.Tilde),
            new TestCase(Key.OpenBrackets, keyboardInput => keyboardInput.OpenBrackets),
            new TestCase(Key.Backslash, keyboardInput => keyboardInput.Backslash),
            new TestCase(Key.CloseBrackets, keyboardInput => keyboardInput.CloseBrackets),
            new TestCase(Key.Quotes, keyboardInput => keyboardInput.Quotes)
        };

        [TestCaseSource(nameof(TestCases))]
        public void Constructor_CreatesKeyboardInputWithAllKeysSetAsSpecified(TestCase testCase)
        {
            // Arrange
            _keyStates[testCase.KeyUnderTest] = false;

            // Act
            var keyboardInput = new KeyboardInput(_keyStates);

            // Assert
            Assert.That(testCase.KeyProperty(keyboardInput), Is.EqualTo(_keyStates[testCase.KeyUnderTest]));
            Assert.That(keyboardInput[testCase.KeyUnderTest], Is.EqualTo(_keyStates[testCase.KeyUnderTest]));

            //-------------------------------------------------------------------------------------------------

            // Arrange
            _keyStates[testCase.KeyUnderTest] = true;

            // Act
            keyboardInput = new KeyboardInput(_keyStates);

            // Assert
            Assert.That(testCase.KeyProperty(keyboardInput), Is.EqualTo(_keyStates[testCase.KeyUnderTest]));
            Assert.That(keyboardInput[testCase.KeyUnderTest], Is.EqualTo(_keyStates[testCase.KeyUnderTest]));
        }

        [TestCaseSource(nameof(TestCases))]
        public void CreateFromLimitedState_CreatesKeyboardInputWithAllKeysSetAsSpecified(TestCase testCase)
        {
            // Arrange
            var keyStates = new Dictionary<Key, bool> {[testCase.KeyUnderTest] = false};

            // Act
            var keyboardInput = KeyboardInput.CreateFromLimitedState(keyStates);

            // Assert
            Assert.That(testCase.KeyProperty(keyboardInput), Is.EqualTo(keyStates[testCase.KeyUnderTest]));
            Assert.That(keyboardInput[testCase.KeyUnderTest], Is.EqualTo(keyStates[testCase.KeyUnderTest]));

            //-------------------------------------------------------------------------------------------------

            // Arrange
            keyStates[testCase.KeyUnderTest] = true;

            // Act
            keyboardInput = KeyboardInput.CreateFromLimitedState(keyStates);

            // Assert
            Assert.That(testCase.KeyProperty(keyboardInput), Is.EqualTo(keyStates[testCase.KeyUnderTest]));
            Assert.That(keyboardInput[testCase.KeyUnderTest], Is.EqualTo(keyStates[testCase.KeyUnderTest]));
        }

        public sealed class TestCase
        {
            public TestCase(Key keyUnderTest, Func<KeyboardInput, bool> keyProperty)
            {
                KeyUnderTest = keyUnderTest;
                KeyProperty = keyProperty;
            }

            public Key KeyUnderTest { get; }
            public Func<KeyboardInput, bool> KeyProperty { get; }

            public override string ToString()
            {
                return $"{nameof(KeyUnderTest)}: {KeyUnderTest}";
            }
        }
    }
}