// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class ColorFilterPanel : UIPanel
	{
		public override void _Ready()
		{
			base._Ready();

			buttons[0].ButtonDown += UIManager.GetInstance().MakeColorBlind; 
			buttons[1].ButtonDown += UIManager.GetInstance().CorrectColorBlind;
            buttons[2].ButtonDown += UIManager.GetInstance().UseNormalVision;
		}


	}
}
