using Godot;
using Godot.Collections;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class UIPanel : Control
	{

		[Export] private Button mainButton;
		[Export] private Panel panel;

		[Export] protected Array<Button> buttons;

		public override void _Ready()
		{
			mainButton.ButtonDown += ToggleShowPanel;
			panel.Hide();
		}



		protected void ToggleShowPanel()
		{
			panel.Visible = !panel.Visible;
		}
	}
}
