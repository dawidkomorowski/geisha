using System;
using NUnit.Framework;

namespace Geisha.TestUtils
{
    public static class AssertEqualityMembers
    {
        public static IWithValues<T> ForValues<T>(T value1, T value2) where T : IEquatable<T> => new WithValues<T>(value1, value2);

        public interface IWithValues<T>
        {
            IWithValuesAndWithEqualityOperator<T> UsingEqualityOperator(Func<T, T, bool> equalityOperator);
        }

        public interface IWithValuesAndWithEqualityOperator<T>
        {
            IWithValuesAndWithEqualityOperatorAndWithInequalityOperator UsingInequalityOperator(Func<T, T, bool> inequalityOperator);
        }

        public interface IWithValuesAndWithEqualityOperatorAndWithInequalityOperator
        {
            void EqualityIsExpectedToBe(bool expectedEquality);
        }

        private sealed class WithValues<T> : IWithValues<T>
            where T : IEquatable<T>
        {
            private readonly T _value1;
            private readonly T _value2;

            public WithValues(T value1, T value2)
            {
                _value1 = value1;
                _value2 = value2;
            }

            public IWithValuesAndWithEqualityOperator<T> UsingEqualityOperator(Func<T, T, bool> equalityOperator) =>
                new WithValuesAndWithEqualityOperator<T>(_value1, _value2, equalityOperator);
        }

        private sealed class WithValuesAndWithEqualityOperator<T> : IWithValuesAndWithEqualityOperator<T>
            where T : IEquatable<T>
        {
            private readonly T _value1;
            private readonly T _value2;
            private readonly Func<T, T, bool> _equalityOperator;

            public WithValuesAndWithEqualityOperator(T value1, T value2, Func<T, T, bool> equalityOperator)
            {
                _value1 = value1;
                _value2 = value2;
                _equalityOperator = equalityOperator;
            }

            public IWithValuesAndWithEqualityOperatorAndWithInequalityOperator UsingInequalityOperator(Func<T, T, bool> inequalityOperator) =>
                new WithValuesAndWithEqualityOperatorAndWithInequalityOperator<T>(_value1, _value2, _equalityOperator, inequalityOperator);
        }

        private sealed class WithValuesAndWithEqualityOperatorAndWithInequalityOperator<T> : IWithValuesAndWithEqualityOperatorAndWithInequalityOperator
            where T : IEquatable<T>
        {
            private readonly T _value1;
            private readonly T _value2;
            private readonly Func<T, T, bool> _equalityOperator;
            private readonly Func<T, T, bool> _inequalityOperator;

            public WithValuesAndWithEqualityOperatorAndWithInequalityOperator(T value1, T value2, Func<T, T, bool> equalityOperator,
                Func<T, T, bool> inequalityOperator)
            {
                _value1 = value1;
                _value2 = value2;
                _equalityOperator = equalityOperator;
                _inequalityOperator = inequalityOperator;
            }

            public void EqualityIsExpectedToBe(bool expectedEquality)
            {
                Assert.That(_value1.Equals(_value2), Is.EqualTo(expectedEquality), "_value1.Equals(_value2)");
                Assert.That(_value2.Equals(_value1), Is.EqualTo(expectedEquality), "_value2.Equals(_value1)");

                Assert.That(_value1.Equals((object)_value2), Is.EqualTo(expectedEquality), "_value1.Equals((object)_value2)");
                Assert.That(_value2.Equals((object)_value1), Is.EqualTo(expectedEquality), "_value2.Equals((object)_value1)");

                Assert.That(_equalityOperator(_value1, _value2), Is.EqualTo(expectedEquality), "_equalityOperator(_value1, _value2)");
                Assert.That(_equalityOperator(_value2, _value1), Is.EqualTo(expectedEquality), "_equalityOperator(_value2, _value1)");

                Assert.That(_inequalityOperator(_value1, _value2), Is.EqualTo(!expectedEquality), "_inequalityOperator(_value1, _value2)");
                Assert.That(_inequalityOperator(_value2, _value1), Is.EqualTo(!expectedEquality), "_inequalityOperator(_value2, _value1)");

                Assert.That(_value1.GetHashCode().Equals(_value2.GetHashCode()), Is.EqualTo(expectedEquality),
                    "_value1.GetHashCode().Equals(_value2.GetHashCode())");

                Assert.That(_value1.Equals(null), Is.False, "_value1.Equals(null)");
                Assert.That(_value2.Equals(null), Is.False, "_value2.Equals(null)");
            }
        }
    }
}