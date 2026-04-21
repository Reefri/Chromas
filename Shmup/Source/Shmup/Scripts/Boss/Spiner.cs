using Godot;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class Spiner : Sprite2D
	{

		public float spinSpeed;
		public override void _Ready()
		{

		}

		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

			Rotate(lDelta*spinSpeed);

		}

		protected override void Dispose(bool pDisposing)
		{

		}
	}
}
