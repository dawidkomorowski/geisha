using System;
using Geisha.Editor.Core;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.Core
{
    [TestFixture]
    public class ViewModelTests
    {
        [Test]
        public void Property_ShouldSupportSetAndGet()
        {
            // Arrange
            var viewModel = new TestViewModel();
            const int expected = 12;

            // Act
            viewModel.NullableInt = expected;

            // Assert
            Assert.That(viewModel.NullableInt, Is.EqualTo(expected));
        }

        [Test]
        public void Property_ShouldSupportInitialValue()
        {
            // Arrange
            const int expected = 12;
            var viewModel = new TestViewModel(expected);

            // Act
            // Assert
            Assert.That(viewModel.NullableInt, Is.EqualTo(expected));
        }

        [TestCase(null, null, null, false)]
        [TestCase(null, 1, 1, true)]
        [TestCase(1, null, null, true)]
        [TestCase(1, 1, 1, false)]
        [TestCase(1, 2, 2, true)]
        public void Property_Set_ShouldRaisePropertyChangedOfViewModel(int? initialValue, int? newValue, int? expectedValue,
            bool expectedPropertyChanged)
        {
            // Arrange
            var viewModel = new TestViewModel(initialValue);

            var wasPropertyChangedRaised = false;
            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(TestViewModel.NullableInt))
                {
                    wasPropertyChangedRaised = true;
                }
            };

            // Act
            viewModel.NullableInt = newValue;

            // Assert
            Assert.That(viewModel.NullableInt, Is.EqualTo(expectedValue));
            Assert.That(wasPropertyChangedRaised, Is.EqualTo(expectedPropertyChanged));
        }

        [Test]
        public void Property_Set_ShouldExecuteSubscribedAction()
        {
            // Arrange
            int? actionParameter = null;
            var viewModel = new TestViewModel();
            viewModel.AddNullableIntSubscription(i => actionParameter = i);

            const int expectedValue = 12;

            // Act
            viewModel.NullableInt = expectedValue;

            // Assert
            Assert.That(actionParameter, Is.EqualTo(expectedValue));
        }

        [Test]
        public void ComputedProperty_Get_ShouldReturnValueComputeFromSourceProperty()
        {
            // Arrange
            var viewModel = new TestViewModel(12);

            // Act
            // Assert
            Assert.That(viewModel.Property1ComputedFromNullableInt, Is.EqualTo(13));
            Assert.That(viewModel.PropertyComputedFromProperty1ComputedFromNullableInt, Is.EqualTo(14));
        }

        [Test]
        public void Property_Set_ShouldRaisePropertyChangedForComputedPropertiesRecursively()
        {
            // Arrange
            var viewModel = new TestViewModel();

            var wasNullableIntPropertyChangedRaised = false;
            var wasProperty1PropertyChangedRaised = false;
            var wasProperty2PropertyChangedRaised = false;
            var wasPropertyComputedFromProperty1PropertyChangedRaised = false;

            viewModel.PropertyChanged += (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case nameof(TestViewModel.NullableInt):
                        wasNullableIntPropertyChangedRaised = true;
                        break;
                    case nameof(TestViewModel.Property1ComputedFromNullableInt):
                        wasProperty1PropertyChangedRaised = true;
                        break;
                    case nameof(TestViewModel.Property2ComputedFromNullableInt):
                        wasProperty2PropertyChangedRaised = true;
                        break;
                    case nameof(TestViewModel.PropertyComputedFromProperty1ComputedFromNullableInt):
                        wasPropertyComputedFromProperty1PropertyChangedRaised = true;
                        break;
                    default:
                        Assert.Fail("Test has wrong setup or functionality is broken!");
                        break;
                }
            };

            // Act
            viewModel.NullableInt = 12;

            // Assert
            Assert.That(wasNullableIntPropertyChangedRaised, Is.True);
            Assert.That(wasProperty1PropertyChangedRaised, Is.True);
            Assert.That(wasProperty2PropertyChangedRaised, Is.True);
            Assert.That(wasPropertyComputedFromProperty1PropertyChangedRaised, Is.True);
        }

        [Test]
        public void Property_Set_ShouldExecuteSubscribedActionsOfComputedPropertiesRecursivelyWithValueComputedFromSourceProperty()
        {
            // Arrange
            int? nullableIntSubscriptionParameterValue = null;
            int? property1SubscriptionParameterValue = null;
            int? property2SubscriptionParameterValue = null;
            int? propertyComputedFromProperty1SubscriptionParameterValue = null;

            var viewModel = new TestViewModel();
            viewModel.AddSubscriptions
            (
                i => nullableIntSubscriptionParameterValue = i,
                i => property1SubscriptionParameterValue = i,
                i => property2SubscriptionParameterValue = i,
                i => propertyComputedFromProperty1SubscriptionParameterValue = i
            );

            // Act
            viewModel.NullableInt = 12;

            // Assert
            Assert.That(nullableIntSubscriptionParameterValue, Is.EqualTo(12));
            Assert.That(property1SubscriptionParameterValue, Is.EqualTo(13));
            Assert.That(property2SubscriptionParameterValue, Is.EqualTo(13));
            Assert.That(propertyComputedFromProperty1SubscriptionParameterValue, Is.EqualTo(14));
        }

        private sealed class TestViewModel : ViewModel
        {
            private readonly IProperty<int?> _nullableInt;
            private readonly IComputedProperty<int?> _property1ComputedFromNullableInt;
            private readonly IComputedProperty<int?> _property2ComputedFromNullableInt;
            private readonly IComputedProperty<int?> _propertyComputedFromProperty1ComputedFromNullableInt;

            public int? NullableInt
            {
                get => _nullableInt.Get();
                set => _nullableInt.Set(value);
            }

            public object? Property1ComputedFromNullableInt => _property1ComputedFromNullableInt.Get();

            public object? Property2ComputedFromNullableInt => _property2ComputedFromNullableInt.Get();

            public object? PropertyComputedFromProperty1ComputedFromNullableInt => _propertyComputedFromProperty1ComputedFromNullableInt.Get();

            public TestViewModel(int? @object)
            {
                _nullableInt = CreateProperty(nameof(NullableInt), @object);
                _property1ComputedFromNullableInt = CreateComputedProperty(nameof(Property1ComputedFromNullableInt), _nullableInt, i => i + 1);
                _property2ComputedFromNullableInt = CreateComputedProperty(nameof(Property2ComputedFromNullableInt), _nullableInt, i => i + 1);
                _propertyComputedFromProperty1ComputedFromNullableInt =
                    CreateComputedProperty(nameof(PropertyComputedFromProperty1ComputedFromNullableInt), _property1ComputedFromNullableInt, i => i + 1);
            }

            public TestViewModel() : this(null)
            {
            }

            public void AddNullableIntSubscription(Action<int?> nullableIntSubscription)
            {
                _nullableInt.Subscribe(nullableIntSubscription);
            }

            public void AddSubscriptions(Action<int?> nullableIntSubscription, Action<int?> property1Subscription, Action<int?> property2Subscription,
                Action<int?> propertyComputedFromProperty1Subscription)
            {
                _nullableInt.Subscribe(nullableIntSubscription);
                _property1ComputedFromNullableInt.Subscribe(property1Subscription);
                _property2ComputedFromNullableInt.Subscribe(property2Subscription);
                _propertyComputedFromProperty1ComputedFromNullableInt.Subscribe(propertyComputedFromProperty1Subscription);
            }
        }
    }
}