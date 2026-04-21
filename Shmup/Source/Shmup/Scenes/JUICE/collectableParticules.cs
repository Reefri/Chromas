using Godot;
using System;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.ProjectName {
	
	public partial class collectableParticules : GpuParticles2D
	{
		public override void _Ready()
		{
			Emitting = true;

			Finished += QueueFree;
		}

		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

		}

		protected override void Dispose(bool pDisposing)
		{

		}
	}
}
