using System;
using Geisha.Editor.Core.ViewModels.Infrastructure;
using NUnit.Framework;

namespace Geisha.Editor.Core.UnitTests.ViewModels.Infrastructure
{
    [TestFixture]
    public class ViewModelTests
    {
        [TestCase(null, null, null, false)]
        [TestCase(null, 1, 1, true)]
        [TestCase(1, null, null, true)]
        [TestCase(1, 1, 1, false)]
        [TestCase(1, 2, 2, true)]
        public void Set_ShouldSetBackingFieldCorrectlyAndRaisePropertyChanged(object initialValue, object newValue, object expectedValue,
            bool expectedPropertyChanged)
        {
            // Arrange
            var viewModel = new TestViewModel(initialValue);

            var wasPropertyChangedRaised = false;
            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(TestViewModel.Object))
                {
                    wasPropertyChangedRaised = true;
                }
            };

            // Act
            viewModel.Object = newValue;

            // Assert
            Assert.That(viewModel.Object, Is.EqualTo(expectedValue));
            Assert.That(wasPropertyChangedRaised, Is.EqualTo(expectedPropertyChanged));
        }

        [Test]
        public void Set_ShouldExecuteSubscribedActionsRecursively()
        {
            // Arrange
            var objectSubscriptionCalled = false;
            var property1SubscriptionCalled = false;
            var property2SubscriptionCalled = false;
            var propertyDependentOnProperty1SubscriptionCalled = false;
            var propertyNotDependentSubscription = false;

            var viewModel = new TestViewModel
            (
                () => objectSubscriptionCalled = true,
                () => property1SubscriptionCalled = true,
                () => property2SubscriptionCalled = true,
                () => propertyDependentOnProperty1SubscriptionCalled = true,
                () => propertyNotDependentSubscription = true
            );

            // Act
            viewModel.Object = new object();

            // Assert
            Assert.That(objectSubscriptionCalled, Is.True);
            Assert.That(property1SubscriptionCalled, Is.True);
            Assert.That(property2SubscriptionCalled, Is.True);
            Assert.That(propertyDependentOnProperty1SubscriptionCalled, Is.True);
            Assert.That(propertyNotDependentSubscription, Is.False);
        }

        [Test]
        public void Set_ShouldRaisePropertyChangedForDependentPropertiesRecursively()
        {
            // Arrange
            var viewModel = new TestViewModel();

            var wasObjectPropertyChangedRaised = false;
            var wasProperty1PropertyChangedRaised = false;
            var wasProperty2PropertyChangedRaised = false;
            var wasPropertyDependentOnProperty1PropertyChangedRaised = false;
            var wasPropertyNotDependentPropertyChangedRaised = false;

            viewModel.PropertyChanged += (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case nameof(TestViewModel.Object):
                        wasObjectPropertyChangedRaised = true;
                        break;
                    case nameof(TestViewModel.Property1DependentOnObject):
                        wasProperty1PropertyChangedRaised = true;
                        break;
                    case nameof(TestViewModel.Property2DependentOnObject):
                        wasProperty2PropertyChangedRaised = true;
                        break;
                    case nameof(TestViewModel.PropertyDependentOnProperty1DependentOnObject):
                        wasPropertyDependentOnProperty1PropertyChangedRaised = true;
                        break;
                    case nameof(TestViewModel.PropertyNotDependentOnObject):
                        wasPropertyNotDependentPropertyChangedRaised = true;
                        break;
                    default:
                        Assert.Fail("Test has wrong setup or functionality is broken!");
                        break;
                }
            };

            // Act
            viewModel.Object = new object();

            // Assert
            Assert.That(wasObjectPropertyChangedRaised, Is.True);
            Assert.That(wasProperty1PropertyChangedRaised, Is.True);
            Assert.That(wasProperty2PropertyChangedRaised, Is.True);
            Assert.That(wasPropertyDependentOnProperty1PropertyChangedRaised, Is.True);
            Assert.That(wasPropertyNotDependentPropertyChangedRaised, Is.False);
        }

        private class TestViewModel : ViewModel
        {
            private object _object;

            public object Object
            {
                get { return _object; }
                set { Set(ref _object, value); }
            }

            public object PropertyNotDependentOnObject { get; set; }

            [DependsOnProperty(nameof(Object))]
            public object Property1DependentOnObject { get; set; }

            [DependsOnProperty(nameof(Object))]
            public object Property2DependentOnObject { get; set; }

            [DependsOnProperty(nameof(Property1DependentOnObject))]
            public object PropertyDependentOnProperty1DependentOnObject { get; set; }

            public TestViewModel(Action objectSubscription, Action property1Subscription, Action property2Subscription,
                Action propertyDependentOnProperty1Subscription, Action propertyNotDependentSubscription)
            {
                Subscribe(nameof(Object), objectSubscription);
                Subscribe(nameof(Property1DependentOnObject), property1Subscription);
                Subscribe(nameof(Property2DependentOnObject), property2Subscription);
                Subscribe(nameof(PropertyDependentOnProperty1DependentOnObject), propertyDependentOnProperty1Subscription);
                Subscribe(nameof(PropertyNotDependentOnObject), propertyNotDependentSubscription);
            }

            public TestViewModel(object @object)
            {
                _object = @object;
            }

            public TestViewModel()
            {
            }
        }
    }
}