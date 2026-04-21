using Godot;
using System;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation
{
	
	public partial class EnemyTwo : Enemy
	{
		protected const string ENEMYBULLET_SCENE_PATH = "res://Scenes/ColoredEntity/Enemy/EnemyBullet.tscn";
		protected PackedScene enemyBulletFactory = (PackedScene)GD.Load(ENEMYBULLET_SCENE_PATH);


        protected Timer paternTimer = new Timer();
		protected float paternTime = 1;

        protected Timer shootingTimer = new Timer();
        protected float shootingTime = 0.4f;
		protected int numberOfShooting = 5;
		protected int shootingCount = 0;

        protected Timer teleportationTimer = new Timer();
        protected float teleportationTime = 0.2f;
        protected int numberOfTeleportation = 7;
        protected int teleportationCount = 0;

        protected float teleportationRadius = 100f;


        protected float teleportationInTweenDuration = 0.1f;
        protected float teleportationOutTweenDuration = 0.1f;

        protected float innacuracyWhenBlind = 0.1f;
        
		public override void _Ready()
		{
			base._Ready();

            hp = 200;

            score = 100;

			paternTimer.WaitTime = paternTime;
			paternTimer.Autostart = false;
			paternTimer.OneShot = true;
            paternTimer.Timeout += OnPaternTimerFinish;
			AddChild(paternTimer);

			shootingTimer.WaitTime = shootingTime;
			shootingTimer.Autostart = false;
			shootingTimer.OneShot = false;
            shootingTimer.Timeout += ShootingNextStep;
			AddChild(shootingTimer);


			teleportationTimer.WaitTime = teleportationTime - teleportationInTweenDuration;
			teleportationTimer.Autostart = false;
			teleportationTimer.OneShot = true;
            teleportationTimer.Timeout += TeleportationInNextStep;
			AddChild(teleportationTimer);

        }

        private void TeleportationInNextStep()
        {
			teleportationCount++;

			


			Tween lTeleportationInTween = CreateTween()
				.SetTrans(Tween.TransitionType.Circ)
				.SetEase(Tween.EaseType.In);

			lTeleportationInTween.TweenProperty(this, "scale:y", 0, teleportationInTweenDuration);

			lTeleportationInTween.Finished += () => TeleportationOutNextStep(FindNextPosition());

        }

		protected virtual Vector2 FindNextPosition()
		{
            Vector2 lTryPosition = new Vector2();

            do
            {
                float lAngle = MathF.PI * 2 * GD.Randf();
                lTryPosition = new Vector2(Mathf.Cos(lAngle), MathF.Sin(lAngle)) * teleportationRadius;

            } while (!(
                (GlobalPosition + lTryPosition).X > CameraManager.GetInstance().GetLevelXBondaries().X &&
                (GlobalPosition + lTryPosition).X < CameraManager.GetInstance().GetLevelXBondaries().Y &&
                (GlobalPosition + lTryPosition).Y > CameraManager.GetInstance().GetLevelYBondaries().X &&
                (GlobalPosition + lTryPosition).Y < CameraManager.GetInstance().GetLevelYBondaries().Y)
                );

			return GlobalPosition + lTryPosition;
        }


		private void TeleportationOutNextStep(Vector2 pTryPosition)
		{
            GlobalPosition = pTryPosition;

            Tween lTeleportationInTween = CreateTween()
				.SetTrans(Tween.TransitionType.Elastic)
				.SetEase(Tween.EaseType.Out);

			lTeleportationInTween.TweenProperty(this, "scale:y", 1, teleportationOutTweenDuration);

			if (teleportationCount >= numberOfTeleportation)
            {
                teleportationCount = 0;
                teleportationTimer.Stop();
                shootingTimer.Start();
            }
            else
            {
                teleportationTimer.Start();
            }

        }


        protected virtual void ShootingNextStep()
        {
			Shoot();

            shootingCount++;

			if (shootingCount >= numberOfShooting)
			{
				shootingCount = 0;
				shootingTimer.Stop();
				paternTimer.Start();
			}
        }

        private void OnPaternTimerFinish()
        {

            teleportationTimer.Start();
        }

        public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

			base._Process(lDelta);
            NaturalScrolling(lDelta);

        }

     

        protected override void Shoot()
        {
            base.Shoot();

            SoundManager.GetInstance().enemyShoot.Play();

            LookAt(Player.GetInstance().GlobalPosition);


            if (Player.GetInstance().isCamouflage)
			{
				float lRandAngle = (GD.Randf() - 0.5f) * MathF.Tau * innacuracyWhenBlind;
				Rotate(lRandAngle); 
			}

			EnemyBullet lBullet = (EnemyBullet)enemyBulletFactory.Instantiate();

			lBullet.GlobalPosition = GlobalPosition;
			lBullet.direction = Vector2.Right.Rotated(Rotation);
            lBullet.color = color;

			GameManager.GetInstance().GetBulletContainer().AddChild(lBullet);


        }

        public override void FullStart()
        {
            base.FullStart();
            shootingTimer.Start();
        }

        public override void Destroy()
        {
            SoundManager.GetInstance().enemyExplosion.Play();
            base.Destroy();
        }

	}
}
