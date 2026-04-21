using Godot;
using System;
using System.Linq;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation
{
	
	public partial class PermanentMark : RigidBody2D
	{

		public float rotationSpeed = 0;
		private float minRotationSpeed = MathF.Tau * 1;
		private float maxRotationSpeed = MathF.Tau * 4;

		public float projectionAngle = -MathF.PI /2;
		public float randomAngle = MathF.Tau /3;

		public float speed = 800;

		private float marginBeforeDestruction = 500;

		[Export] public Node2D spriteContainer;


		public override void _Ready()
		{
			projectionAngle = (GD.Randf()-0.5f) * randomAngle + projectionAngle;
			rotationSpeed = GD.Randf() * (maxRotationSpeed - minRotationSpeed ) + minRotationSpeed;

			LinearVelocity = new Vector2(MathF.Cos(projectionAngle), MathF.Sin(projectionAngle)) * speed;
			AngularVelocity = rotationSpeed;

			foreach (Sprite2D lSprite in spriteContainer.GetChildren().OfType<Sprite2D>())
			{
				lSprite.Offset = -((Marker2D)lSprite.GetChild(0)).Position;
			}

		}

		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

			if (GlobalPosition.Y > CameraManager.GetInstance().GetLevelYBondaries().Y + marginBeforeDestruction) QueueFree();


		}

		protected override void Dispose(bool pDisposing)
		{

		}
	}
}
