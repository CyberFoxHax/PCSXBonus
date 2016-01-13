namespace ColorPicker
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;

    public sealed class BindOnEnterTextBox : TextBox
    {
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Return)
            {
                BindingExpression bindingExpression = BindingOperations.GetBindingExpression(this, TextBox.TextProperty);
                if (bindingExpression != null)
                {
                    bindingExpression.UpdateSource();
                }
            }
        }
    }
}

