using Godot;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class MyBaseButton : Button
	{
		public override void _Ready()
		{
			ButtonDown += PlaySound;
		}

		protected virtual void PlaySound()
		{
			SoundManager.GetInstance().PlayUIClick();

        }
	}
}
