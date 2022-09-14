using System.Windows;
using System.Windows.Controls;
using Geisha.Engine.Core.Math;

namespace Geisha.Editor.Core.Controls
{
    /// <summary>
    /// Interaction logic for Vector3Editor.xaml
    /// </summary>
    public partial class Vector3Editor : UserControl
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(Vector3), typeof(Vector3Editor),
            new FrameworkPropertyMetadata(default(Vector3), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        public Vector3Editor()
        {
            InitializeComponent();
        }

        public Vector3 Value
        {
            get => (Vector3) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (Vector3Editor) d;
            var newValue = (Vector3) e.NewValue;

            editor.X.Value = newValue.X;
            editor.Y.Value = newValue.Y;
            editor.Z.Value = newValue.Z;
        }

        private void X_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            Value = Value.WithX(X.Value);
        }

        private void Y_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            Value = Value.WithY(Y.Value);
        }

        private void Z_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            Value = Value.WithZ(Z.Value);
        }
    }
}