using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace PCSX2Bonus.CustomControls.ColorPicker {
	public sealed class BindOnEnterTextBox : TextBox {
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if (e.Key != Key.Return) return;
			var bindingExpression = BindingOperations.GetBindingExpression(this, TextProperty);
			if (bindingExpression != null)
				bindingExpression.UpdateSource();
		}
	}
}

