using Godot;
using System;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation
{
	
	public partial class EnemyOne : Enemy
	{
		private const string ENEMYONE_SCENE_PATH = "res://Scenes/ColoredEntity/Enemy/EnemyOne.tscn";
		private static PackedScene enemyOneFactory = (PackedScene)GD.Load(ENEMYONE_SCENE_PATH);

		Vector2 basePosition;
		float amplitude = 40;
		float xSpeed = 60;
		float sinSpeed = MathF.Tau / 2.5f;

		float timeProgression = 0;


        public override void _Ready()
		{
			base._Ready();

			hp = 0;
			score = 50;


		}

        public override void _Process(double pDelta)
        {
			float lDelta = (float)pDelta;
            base._Process(pDelta);


			GlobalPosition = basePosition +  new Vector2(-xSpeed * timeProgression,Mathf.Sin( sinSpeed * timeProgression) *amplitude);
			timeProgression += lDelta;

        }

        public static EnemyOne Create()
		{
			return (EnemyOne)enemyOneFactory.Instantiate();
		}

        public override void FullStart()
        {
            base.FullStart();
			basePosition = GlobalPosition;
        }

        public override void Destroy()
        {
            base.Destroy();
			SoundManager.GetInstance().PlayEnemyOneExplosion();
        }
	}
}
