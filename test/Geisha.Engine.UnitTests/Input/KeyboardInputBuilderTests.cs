using System;
using System.Collections.Generic;
using Geisha.Engine.Input;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Input
{
    [TestFixture]
    public class KeyboardInputBuilderTests
    {
        private static IEnumerable<TestCase> TestCases => new[]
        {
            new TestCase(Key.Backspace, ki => ki.Backspace, (b, v) => b.Backspace = v),
            new TestCase(Key.Tab, ki => ki.Tab, (b, v) => b.Tab = v),
            new TestCase(Key.Enter, ki => ki.Enter, (b, v) => b.Enter = v),
            new TestCase(Key.LeftShift, ki => ki.LeftShift, (b, v) => b.LeftShift = v),
            new TestCase(Key.RightShift, ki => ki.RightShift, (b, v) => b.RightShift = v),
            new TestCase(Key.LeftCtrl, ki => ki.LeftCtrl, (b, v) => b.LeftCtrl = v),
            new TestCase(Key.RightCtrl, ki => ki.RightCtrl, (b, v) => b.RightCtrl = v),
            new TestCase(Key.LeftAlt, ki => ki.LeftAlt, (b, v) => b.LeftAlt = v),
            new TestCase(Key.RightAlt, ki => ki.RightAlt, (b, v) => b.RightAlt = v),
            new TestCase(Key.Pause, ki => ki.Pause, (b, v) => b.Pause = v),
            new TestCase(Key.CapsLock, ki => ki.CapsLock, (b, v) => b.CapsLock = v),
            new TestCase(Key.Escape, ki => ki.Escape, (b, v) => b.Escape = v),
            new TestCase(Key.Space, ki => ki.Space, (b, v) => b.Space = v),
            new TestCase(Key.PageUp, ki => ki.PageUp, (b, v) => b.PageUp = v),
            new TestCase(Key.PageDown, ki => ki.PageDown, (b, v) => b.PageDown = v),
            new TestCase(Key.End, ki => ki.End, (b, v) => b.End = v),
            new TestCase(Key.Home, ki => ki.Home, (b, v) => b.Home = v),
            new TestCase(Key.Left, ki => ki.Left, (b, v) => b.Left = v),
            new TestCase(Key.Up, ki => ki.Up, (b, v) => b.Up = v),
            new TestCase(Key.Right, ki => ki.Right, (b, v) => b.Right = v),
            new TestCase(Key.Down, ki => ki.Down, (b, v) => b.Down = v),
            new TestCase(Key.Insert, ki => ki.Insert, (b, v) => b.Insert = v),
            new TestCase(Key.Delete, ki => ki.Delete, (b, v) => b.Delete = v),
            new TestCase(Key.D0, ki => ki.D0, (b, v) => b.D0 = v),
            new TestCase(Key.D1, ki => ki.D1, (b, v) => b.D1 = v),
            new TestCase(Key.D2, ki => ki.D2, (b, v) => b.D2 = v),
            new TestCase(Key.D3, ki => ki.D3, (b, v) => b.D3 = v),
            new TestCase(Key.D4, ki => ki.D4, (b, v) => b.D4 = v),
            new TestCase(Key.D5, ki => ki.D5, (b, v) => b.D5 = v),
            new TestCase(Key.D6, ki => ki.D6, (b, v) => b.D6 = v),
            new TestCase(Key.D7, ki => ki.D7, (b, v) => b.D7 = v),
            new TestCase(Key.D8, ki => ki.D8, (b, v) => b.D8 = v),
            new TestCase(Key.D9, ki => ki.D9, (b, v) => b.D9 = v),
            new TestCase(Key.A, ki => ki.A, (b, v) => b.A = v),
            new TestCase(Key.B, ki => ki.B, (b, v) => b.B = v),
            new TestCase(Key.C, ki => ki.C, (b, v) => b.C = v),
            new TestCase(Key.D, ki => ki.D, (b, v) => b.D = v),
            new TestCase(Key.E, ki => ki.E, (b, v) => b.E = v),
            new TestCase(Key.F, ki => ki.F, (b, v) => b.F = v),
            new TestCase(Key.G, ki => ki.G, (b, v) => b.G = v),
            new TestCase(Key.H, ki => ki.H, (b, v) => b.H = v),
            new TestCase(Key.I, ki => ki.I, (b, v) => b.I = v),
            new TestCase(Key.J, ki => ki.J, (b, v) => b.J = v),
            new TestCase(Key.K, ki => ki.K, (b, v) => b.K = v),
            new TestCase(Key.L, ki => ki.L, (b, v) => b.L = v),
            new TestCase(Key.M, ki => ki.M, (b, v) => b.M = v),
            new TestCase(Key.N, ki => ki.N, (b, v) => b.N = v),
            new TestCase(Key.O, ki => ki.O, (b, v) => b.O = v),
            new TestCase(Key.P, ki => ki.P, (b, v) => b.P = v),
            new TestCase(Key.Q, ki => ki.Q, (b, v) => b.Q = v),
            new TestCase(Key.R, ki => ki.R, (b, v) => b.R = v),
            new TestCase(Key.S, ki => ki.S, (b, v) => b.S = v),
            new TestCase(Key.T, ki => ki.T, (b, v) => b.T = v),
            new TestCase(Key.U, ki => ki.U, (b, v) => b.U = v),
            new TestCase(Key.V, ki => ki.V, (b, v) => b.V = v),
            new TestCase(Key.W, ki => ki.W, (b, v) => b.W = v),
            new TestCase(Key.X, ki => ki.X, (b, v) => b.X = v),
            new TestCase(Key.Y, ki => ki.Y, (b, v) => b.Y = v),
            new TestCase(Key.Z, ki => ki.Z, (b, v) => b.Z = v),
            new TestCase(Key.NumPad0, ki => ki.NumPad0, (b, v) => b.NumPad0 = v),
            new TestCase(Key.NumPad1, ki => ki.NumPad1, (b, v) => b.NumPad1 = v),
            new TestCase(Key.NumPad2, ki => ki.NumPad2, (b, v) => b.NumPad2 = v),
            new TestCase(Key.NumPad3, ki => ki.NumPad3, (b, v) => b.NumPad3 = v),
            new TestCase(Key.NumPad4, ki => ki.NumPad4, (b, v) => b.NumPad4 = v),
            new TestCase(Key.NumPad5, ki => ki.NumPad5, (b, v) => b.NumPad5 = v),
            new TestCase(Key.NumPad6, ki => ki.NumPad6, (b, v) => b.NumPad6 = v),
            new TestCase(Key.NumPad7, ki => ki.NumPad7, (b, v) => b.NumPad7 = v),
            new TestCase(Key.NumPad8, ki => ki.NumPad8, (b, v) => b.NumPad8 = v),
            new TestCase(Key.NumPad9, ki => ki.NumPad9, (b, v) => b.NumPad9 = v),
            new TestCase(Key.Multiply, ki => ki.Multiply, (b, v) => b.Multiply = v),
            new TestCase(Key.Add, ki => ki.Add, (b, v) => b.Add = v),
            new TestCase(Key.Subtract, ki => ki.Subtract, (b, v) => b.Subtract = v),
            new TestCase(Key.Decimal, ki => ki.Decimal, (b, v) => b.Decimal = v),
            new TestCase(Key.Divide, ki => ki.Divide, (b, v) => b.Divide = v),
            new TestCase(Key.F1, ki => ki.F1, (b, v) => b.F1 = v),
            new TestCase(Key.F2, ki => ki.F2, (b, v) => b.F2 = v),
            new TestCase(Key.F3, ki => ki.F3, (b, v) => b.F3 = v),
            new TestCase(Key.F4, ki => ki.F4, (b, v) => b.F4 = v),
            new TestCase(Key.F5, ki => ki.F5, (b, v) => b.F5 = v),
            new TestCase(Key.F6, ki => ki.F6, (b, v) => b.F6 = v),
            new TestCase(Key.F7, ki => ki.F7, (b, v) => b.F7 = v),
            new TestCase(Key.F8, ki => ki.F8, (b, v) => b.F8 = v),
            new TestCase(Key.F9, ki => ki.F9, (b, v) => b.F9 = v),
            new TestCase(Key.F10, ki => ki.F10, (b, v) => b.F10 = v),
            new TestCase(Key.F11, ki => ki.F11, (b, v) => b.F11 = v),
            new TestCase(Key.F12, ki => ki.F12, (b, v) => b.F12 = v),
            new TestCase(Key.NumLock, ki => ki.NumLock, (b, v) => b.NumLock = v),
            new TestCase(Key.ScrollLock, ki => ki.ScrollLock, (b, v) => b.ScrollLock = v),
            new TestCase(Key.Semicolon, ki => ki.Semicolon, (b, v) => b.Semicolon = v),
            new TestCase(Key.EqualsSign, ki => ki.EqualsSign, (b, v) => b.EqualsSign = v),
            new TestCase(Key.Comma, ki => ki.Comma, (b, v) => b.Comma = v),
            new TestCase(Key.Dash, ki => ki.Dash, (b, v) => b.Dash = v),
            new TestCase(Key.Period, ki => ki.Period, (b, v) => b.Period = v),
            new TestCase(Key.Slash, ki => ki.Slash, (b, v) => b.Slash = v),
            new TestCase(Key.Tilde, ki => ki.Tilde, (b, v) => b.Tilde = v),
            new TestCase(Key.OpenBrackets, ki => ki.OpenBrackets, (b, v) => b.OpenBrackets = v),
            new TestCase(Key.Backslash, ki => ki.Backslash, (b, v) => b.Backslash = v),
            new TestCase(Key.CloseBrackets, ki => ki.CloseBrackets, (b, v) => b.CloseBrackets = v),
            new TestCase(Key.Quotes, ki => ki.Quotes, (b, v) => b.Quotes = v)
        };

        [TestCaseSource(nameof(TestCases))]
        public void Build_CreatesKeyboardInputWithAllKeysSetAsSpecified(TestCase testCase)
        {
            // Arrange
            var builder = new KeyboardInputBuilder();
            testCase.SetBuilderProperty(builder, false);

            // Act
            var keyboardInput = builder.Build();

            // Assert
            Assert.That(testCase.KeyProperty(keyboardInput), Is.False);
            Assert.That(keyboardInput[testCase.KeyUnderTest], Is.False);

            //-------------------------------------------------------------------------------------------------

            // Arrange
            builder = new KeyboardInputBuilder();
            testCase.SetBuilderProperty(builder, true);

            // Act
            keyboardInput = new KeyboardInput(builder);

            // Assert
            Assert.That(testCase.KeyProperty(keyboardInput), Is.True);
            Assert.That(keyboardInput[testCase.KeyUnderTest], Is.True);
        }

        public sealed class TestCase
        {
            public TestCase(Key keyUnderTest, Func<KeyboardInput, bool> keyProperty, Action<KeyboardInputBuilder, bool> setBuilderProperty)
            {
                KeyUnderTest = keyUnderTest;
                KeyProperty = keyProperty;
                SetBuilderProperty = setBuilderProperty;
            }

            public Key KeyUnderTest { get; }
            public Func<KeyboardInput, bool> KeyProperty { get; }
            public Action<KeyboardInputBuilder, bool> SetBuilderProperty { get; }

            public override string ToString()
            {
                return $"{nameof(KeyUnderTest)}: {KeyUnderTest}";
            }
        }
    }
}