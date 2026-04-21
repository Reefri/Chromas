using Godot;
using System;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class ConstrolSpiner : TextureRect
	{
        [Export] public float spinSpeed;
        public override void _Ready()
        {
            PivotOffset = Size / 2;

        }

        public override void _Process(double pDelta)
        {
            float lDelta = (float)pDelta;

            Rotation += lDelta * MathF.Tau / spinSpeed;

        }

        protected override void Dispose(bool pDisposing)
        {

        }
    }
}
