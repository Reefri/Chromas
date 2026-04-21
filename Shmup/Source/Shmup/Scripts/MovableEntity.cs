using Godot;
using System;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class MovableEntity : Node2D
	{
		public Vector2 direction = new Vector2 (0, 0);
		public Vector2 speed = new Vector2 (0, 0);

		public override void _Ready()
		{

		}

		public override void _Process(double pDelta)
		{

			Move((float)pDelta);
		}

		protected override void Dispose(bool pDisposing)
		{

		}

		public virtual void Move(float pDelta)
		{
			Position += direction.Normalized() * speed * pDelta;
		}

		protected virtual void NaturalScrolling(float pDelta)
		{
            GlobalPosition += Vector2.Right * LevelManager.currentLevelSpeed * pDelta;
        }

    }
}
