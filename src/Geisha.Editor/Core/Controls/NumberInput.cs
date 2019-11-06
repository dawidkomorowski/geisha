using System.Windows;
using System.Windows.Controls;

namespace Geisha.Editor.Core.Controls
{
    public abstract class NumberInput<TNumber> : TextBox
    {
        private bool _shouldSelectAll = true;

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(TNumber), typeof(NumberInput<TNumber>),
            new FrameworkPropertyMetadata(default(TNumber), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        public NumberInput()
        {
            Text = Convert(Value);
        }

        public TNumber Value
        {
            get => (TNumber) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        protected abstract string Convert(TNumber value);
        protected abstract bool TryConvert(string text, out TNumber value);

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            if (TryConvert(Text, out var value))
            {
                Value = value;
                base.OnTextChanged(e);
            }
            else
            {
                Text = Convert(Value);
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            _shouldSelectAll = true;
        }

        protected override void OnSelectionChanged(RoutedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (_shouldSelectAll && IsFocused)
            {
                _shouldSelectAll = false;
                SelectAll();
            }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericTextBox = (NumberInput<TNumber>) d;
            var newValue = (TNumber) e.NewValue;

            numericTextBox.Text = numericTextBox.Convert(newValue);
        }
    }
}