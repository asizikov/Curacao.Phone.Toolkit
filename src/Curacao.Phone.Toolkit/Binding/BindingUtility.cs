using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;

namespace Curacao.Phone.Toolkit.Binding
{
    public class BindingUtility
    {
        [PublicAPI]
        public static bool GetUpdateSourceOnChange(DependencyObject d)
        {
            return (bool)d.GetValue(UpdateSourceOnChangeProperty);
        }

        [PublicAPI]
        public static void SetUpdateSourceOnChange(DependencyObject d, bool value)
        {
            d.SetValue(UpdateSourceOnChangeProperty, value);
        }

        public static readonly DependencyProperty
            UpdateSourceOnChangeProperty =
                DependencyProperty.RegisterAttached(
                    "UpdateSourceOnChange",
                    typeof(bool),
                    typeof(BindingUtility),
                    new PropertyMetadata(false, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var textBox = d as TextBox;
            if (textBox == null)
                return;
            if ((bool)e.NewValue)
            {
                textBox.TextChanged += OnTextChanged;
            }
            else
            {
                textBox.TextChanged -= OnTextChanged;
            }
        }

        private static void OnTextChanged(object s, TextChangedEventArgs e)
        {
            var textBox = s as TextBox;
            if (textBox == null)
                return;

            var bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);
            if (bindingExpression != null)
            {
                bindingExpression.UpdateSource();
            }
        }
    }
}