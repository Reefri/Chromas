using Godot;
using System;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.ProjectName {
	
	public partial class BossExplosion : GpuParticles2D
	{
		public override void _Ready()
		{
			SetProcess(false);

		}

        public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;
            Emitting = true;


        }

        protected override void Dispose(bool pDisposing)
		{

		}
	}
}
