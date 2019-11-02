using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Geisha.Editor.Core.Controls
{
    public class NumericTextBox : TextBox
    {
        private bool _shouldSelectAll = true;

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(double), typeof(NumericTextBox),
            new PropertyMetadata(default(double), OnValueChanged));

        public NumericTextBox()
        {
            Text = Value.ToString(CultureInfo.InvariantCulture);
        }

        public double Value
        {
            get => (double) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            if (double.TryParse(Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
            {
                Value = value;
                base.OnTextChanged(e);
            }
            else
            {
                Text = Value.ToString(CultureInfo.InvariantCulture);
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
            var numericTextBox = (NumericTextBox) d;
            var newValue = (double) e.NewValue;

            numericTextBox.Text = newValue.ToString(CultureInfo.InvariantCulture);
        }
    }
}