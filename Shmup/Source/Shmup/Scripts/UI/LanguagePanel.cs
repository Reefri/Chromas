using Godot;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class LanguagePanel : UIPanel
	{
		public override void _Ready()
		{
			base._Ready();
			buttons[0].ButtonDown += () => TranslationServer.SetLocale(LanguageProp.FRENCH);
			buttons[1].ButtonDown += () => TranslationServer.SetLocale(LanguageProp.ENGLISH);
		}
	}
}
