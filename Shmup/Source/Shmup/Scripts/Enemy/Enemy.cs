using Godot;
using System;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class Enemy : KillableEntity
	{

		public int score = 0;

		public float collisionDamage=10;

		public bool makeExplosion = false;


		private float explosionScale = 5;
		private float explosionDuration = 0.4f;


		private Tween spawnScaleTween;

		public override void _Ready()
		{
			colorGivenTroughtCode = false;
            speed = Vector2.Zero;
			SetProcess(false);

            base._Ready();


            colliderNode.AreaEntered -= OnAreaEntered;
            colliderNode.AreaExited -= OnAreaExited;

            Rotate(MathF.PI);


            Hide();
        }



		
		protected virtual void Shoot()
		{

		}

        protected override void OnAreaEntered(Area2D pArea)
        {
            ColoredEntity lCollidedColoredEntity = (ColoredEntity)pArea.GetParent();


            if (lCollidedColoredEntity is Player)
			{
				Destroy();
				return;
			}

			

            if (lCollidedColoredEntity is SmartBomb)
            {
                makeExplosion = true;
				Destroy();
                return;
            }

            if (lCollidedColoredEntity is Explosion && ColorManager.AreEnemyColorsCloseEnough(color, ColorManager.GetOpposite(lCollidedColoredEntity.color)))
            {
                makeExplosion = true;
                Destroy();
                return;

            }
        }

		public override void Destroy()
		{
			if (makeExplosion) { Explosion.Create(GlobalPosition,color,explosionScale,explosionDuration); }

			base.Destroy();
            GameManager.GetInstance().CreateCollectable(GlobalPosition);

			Player.GetInstance().ScorePoints(score);

        }


		public virtual void FullStart()
		{
			SetProcess(true);

			Show();
			spawnScaleTween = CreateTween()
				.SetTrans(Tween.TransitionType.Elastic)
				.SetEase(Tween.EaseType.Out);

			spawnScaleTween.TweenProperty(this,"scale",Scale,1).From(Vector2.Zero);



            colliderNode.AreaEntered += OnAreaEntered;
            colliderNode.AreaExited += OnAreaExited;
        }


		public void Appear()
		{
            Show();
            spawnScaleTween = CreateTween()
                .SetTrans(Tween.TransitionType.Elastic)
                .SetEase(Tween.EaseType.Out);

            spawnScaleTween.TweenProperty(this, "scale", Scale, 1).From(Vector2.Zero);

        }

		public void Start()
		{
            SetProcess(true);

            colliderNode.AreaEntered += OnAreaEntered;
            colliderNode.AreaExited += OnAreaExited;
        }

    }
}
