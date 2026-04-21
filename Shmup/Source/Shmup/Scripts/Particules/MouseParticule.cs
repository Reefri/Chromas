using Godot;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class MouseParticule : GpuParticles2D
	{


        public static bool enableParticule = true;


		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

            if (enableParticule)
            {
                Emitting = true;

                LookAt(GetViewport().GetMousePosition());
                Position = GetViewport().GetMousePosition();
            }
            else
            {
                Emitting = false;

            }


        }

	}
}
