using Godot;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class Credits : Control
	{
		public override void _Ready()
		{
            GuiInput += OnActionDetected; ;
		}

        private void OnActionDetected(InputEvent pEvent)
        {
            if (!(pEvent is InputEventMouseMotion))
            {
                Disable();
            }
        }

 
		public void Enable()
		{
			Show();
		}

		public void Disable()
		{
			Hide();
		}
	}
}
