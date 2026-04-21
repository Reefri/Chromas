using Godot;
using System;

// Author : Sacha Gramatikoff

namespace Com.IsartDigital.Chromaberation {
	
	public partial class BaseBoss : KillableEntity
	{






		public override void _Ready()
		{
			base._Ready();
		}

		public override void _Process(double pDelta)
		{
			base._Process(pDelta);
			float lDelta = (float)pDelta;
		}



        protected override void UpdateColor()
        {
        }








      
    }
}
