using System.Windows;
using System.Windows.Controls;
using Geisha.Common.Math;

namespace Geisha.Editor.Core.Controls
{
    /// <summary>
    /// Interaction logic for Vector2Editor.xaml
    /// </summary>
    public partial class Vector2Editor : UserControl
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(Vector2), typeof(Vector2Editor),
            new PropertyMetadata(default(Vector2), OnValueChanged));

        public Vector2Editor()
        {
            InitializeComponent();
        }

        public Vector2 Value
        {
            get => (Vector2) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (Vector2Editor) d;
            var newValue = (Vector2) e.NewValue;

            editor.ValueX.Value = newValue.X;
            editor.ValueY.Value = newValue.Y;
        }

        private void ValueX_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            Value = Value.WithX(ValueX.Value);
        }

        private void ValueY_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            Value = Value.WithY(ValueY.Value);
        }
    }
}