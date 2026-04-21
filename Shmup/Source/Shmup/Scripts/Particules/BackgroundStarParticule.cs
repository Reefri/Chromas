using Godot;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class BackgroundStarParticule : GpuParticles2D
	{

		private float margin = 100;



		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

			((ParticleProcessMaterial)ProcessMaterial).EmissionBoxExtents =  new Vector3(GetViewport().GetVisibleRect().Size.X + margin, GetViewport().GetVisibleRect().Size.Y,0);
		}

	}
}
