using System;
using System.Windows;
using System.Windows.Controls;

namespace Geisha.Editor.Core.Controls
{
    /// <summary>
    /// Interaction logic for EnumEditor.xaml
    /// </summary>
    public partial class EnumEditor : UserControl
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(object), typeof(EnumEditor),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        public EnumEditor()
        {
            InitializeComponent();
        }

        public object? Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (EnumEditor)d;
            var newValue = e.NewValue;

            if (newValue is not Enum)
            {
                return;
            }

            editor.ComboBox.ItemsSource = Enum.GetValues(newValue.GetType());
            editor.ComboBox.SelectedValue = newValue;
        }

        private void ComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Value = ComboBox.SelectedValue;
        }
    }
}