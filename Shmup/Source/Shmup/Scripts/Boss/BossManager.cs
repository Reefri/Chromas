using Godot;
using System;
using System.Collections.Generic;

// Author : Sacha Gramatikoff

namespace Com.IsartDigital.Chromaberation {
	
	public partial class BossManager : MovableEntity
	{

		static private BossManager instance;
		static private PackedScene factory = GD.Load<PackedScene>("res://Scenes/Manager/BossManager.tscn");

		[Export] private Node tickEventsContainer;

		private float marginBeforeActivating = 100f;
        private bool isActive = false;



        public float hp = 4500;
        public float maxHp = 4500;


		[Export] PackedScene smartBomb;



		private List<Action> phaseOneAttackSet = new List<Action>();
		private List<Action> phaseTwoAttackSet = new List<Action>();
		private List<Action> phaseThreeAttackSet = new List<Action>();

		public int currentPhase = 0;

		Timer preExplosionTimer = new Timer();
		float preExplosionTimerDuration = 2.3f;
		Timer afterExplosionTimer = new Timer();
        float afterExplosionTimerDuration = 3f;


		Timer beforeGameOverTimer = new Timer();
		float beforeGameOverDuration = 4f;


		public bool isFinished = false;


        private BossManager():base()
        {
            if (instance != null || IsInstanceValid(instance))
            {
                instance.QueueFree();
                GD.Print(nameof(BossManager) + " Instance already exist, destroying the last added.");
				return;
			}

			instance = this;	
		}

		static public BossManager GetInstance()
		{
			if (instance == null) instance = (BossManager)factory.Instantiate();
			return instance;

		}

		public override void _Ready()
		{
			base._Ready();

			Scale = Vector2.Zero;

	
            foreach (AudioStreamPlayer lAudioStreamPlayer in tickEventsContainer.GetChildren())
            { lAudioStreamPlayer.Stop(); }


			preExplosionTimer.WaitTime = preExplosionTimerDuration;
			preExplosionTimer.Autostart = false;
			preExplosionTimer.OneShot = true;

			preExplosionTimer.Timeout += BossExplosion;

			AddChild(preExplosionTimer);

			afterExplosionTimer.WaitTime = afterExplosionTimerDuration;
			afterExplosionTimer.Autostart = false;
			afterExplosionTimer.OneShot = true;

			afterExplosionTimer.Timeout += AfterBossExplosion;
			AddChild(afterExplosionTimer);


			beforeGameOverTimer.WaitTime = beforeGameOverDuration;
			beforeGameOverTimer.Autostart = false;
			beforeGameOverTimer.OneShot = true;
			beforeGameOverTimer.Timeout += UIManager.GetInstance().gameOver.Enable;
			AddChild(beforeGameOverTimer);
            

        }

        public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;


			GlobalPosition = new Vector2(GlobalPosition.X, (CameraManager.GetInstance().GetLevelYBondaries().Y -
                (CameraManager.GetInstance().GetLevelYBondaries().Y - CameraManager.GetInstance().GetLevelYBondaries().X) / 2));


			if (!isActive)
			{

                GlobalPosition = new Vector2(GlobalPosition.X, (CameraManager.GetInstance().GetLevelYBondaries().X +
                (CameraManager.GetInstance().GetLevelYBondaries().Y - CameraManager.GetInstance().GetLevelYBondaries().X) / 2));

				if (CameraManager.GetInstance().GetLevelXBondaries().Y > GlobalPosition.X + marginBeforeActivating)
				{

					
					StartBossFight();

				}
				else { return; }
			}




	

		}

		protected override void Dispose(bool pDisposing)
		{
			instance = null;

            base.Dispose(pDisposing);
		}





        private void StartBossFight()
        {
			Scale = Vector2.One;
            SoundManager.GetInstance().PlayBoss();
            SoundManager.GetInstance().level.Stop();

            foreach (AudioStreamPlayer lAudioStreamPlayer in tickEventsContainer.GetChildren())
            { lAudioStreamPlayer.Play(); }

            BossA.GetInstance().Start();

            isActive = true;

            UIManager.GetInstance().hud.EnableBossBar();


        }





        public void ChangePhase()
		{
			if (isFinished) return;

			if (currentPhase == 1)
			{
				BossA.GetInstance().ChangeForLastPhase();
			}

			if (currentPhase >= 2 && !isFinished)
			{
				isFinished = true;


				WhenBossDefeated();
				return;
			}
			currentPhase += 1;
			BossA.GetInstance().ResetPosition();
            ((Sprite2D)BossA.GetInstance().rendererNode.GetChild(currentPhase)).Visible = true;


        }



		private void WhenBossDefeated()
		{
			Player.GetInstance().StopShooting();

			UIManager.GetInstance().hud.Disable();

			LevelManager.currentLevelSpeed = 0;

			Vector2 lPosition = new Vector2(
					CameraManager.GetInstance().GetCameraXBondaries().X + CameraManager.GetInstance().GetCameraXBondaries().Y,
					CameraManager.GetInstance().GetCameraYBondaries().X + CameraManager.GetInstance().GetCameraYBondaries().Y) / 2;


            BossA.GetInstance().isMoving = false;
			BossA.GetInstance().TeleporteTo(lPosition).Finished += () => BossA.GetInstance().EmitteParticules();


            foreach (AudioStreamPlayer lAudioStreamPlayer in tickEventsContainer.GetChildren())
            { 
				lAudioStreamPlayer.QueueFree();
            }


            Player.GetInstance().isAlive = hp < 0;
			Player.GetInstance().canDie = false;

            preExplosionTimer.Start();
			SoundManager.GetInstance().PlayBossPreExplosion();





            SmartBomb lSmartBomb = (SmartBomb)smartBomb.Instantiate();

			lSmartBomb.explosionDuration = 2.6f;
            lSmartBomb.GlobalPosition = lPosition;
            lSmartBomb.isReversed = true;
            GameManager.GetInstance().GetExplosionContainer().AddChild(lSmartBomb);

        }


        private void BossExplosion()
		{

            SoundManager.GetInstance().PlayBossExplosion();
			afterExplosionTimer.Start();



            Main.GetInstance().chromaAberation.amplitudeMult = 2;
            Main.GetInstance().chromaAberation.duration = 3;
            Main.GetInstance().chromaAberation.Start();


            SmartBomb lSmartBomb = (SmartBomb)smartBomb.Instantiate();

            lSmartBomb.explosionDuration = 2.6f;
            lSmartBomb.GlobalPosition = BossA.GetInstance().GlobalPosition;
            GameManager.GetInstance().GetExplosionContainer().AddChild(lSmartBomb);


        }

        private void AfterBossExplosion()
		{



			BossA.GetInstance().finalExplosion.Emitting = true;
            BossA.GetInstance().explosionParticules.QueueFree();
            BossA.GetInstance().rendererNode.Visible = false;
			BossA.GetInstance().enemyOneContainer.Visible = false;

			foreach (Sprite2D lSprite in BossA.GetInstance().rendererNode.GetChildren())
			{
				GameManager.GetInstance().AddPermentMark(lSprite, new Color(1, 1, 1));
			}

			beforeGameOverTimer.Start();

        }
    }
}
