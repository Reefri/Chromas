using Godot;
using System;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class StartButton : CustomButton 
	{
        protected override void PlaySound()
        {
            SoundManager.GetInstance().uiStart.Play();
        }

	}
}
