using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

// Author : Sacha Gramatikoff

namespace Com.IsartDigital.Chromaberation {
	
	public partial class BossA : KillableEntity
	{

        private float collideDamage = 10;

        private Tween bumpTween;
        private float bumpTweenDuration = 0.2f;
        private Vector2 bumpTweenScale = Vector2.One * 1.5f;

        private Tween shakeTween;
        private float shakeTweenDuration = 0.2f;
        private float shakeTweenAngle = MathF.Tau / 32;
        private int shakeNumber = 2;

        public int scoreOnHit = 10;

        static private BossA instance;
		static private PackedScene factory = GD.Load<PackedScene>("res://Scenes/Manager/BossA.tscn");

		[Export] public Node2D enemyOneContainer;
		[Export] private float enemyOneSpawnRadius = 100;



        [Export] public GpuParticles2D explosionParticules;
        [Export] public GpuParticles2D finalExplosion;


        public bool isMoving = true;
        private Vector2 basePosition;

        private float timeProgression = Mathf.Pi / 2;
        private float timeProgressionSpeed = 1;
        private float xAmplitude = 200;
        private float yAmplitude = 200;

        private float amplitudeMargin = 50;

        public float baseYPosition;




        private Timer afterLaunchTimer = new Timer();
		private float afterLaunchDuration = 0.1f;



		private Timer launchEnemyOnePhaseThreeTimer = new Timer();
		private float betweenlaunchDuration = 0.2f;
		private int launchEnemyOnePhaseThreeProgression = 0;


        private int launchEnemyProgression = 0;

		private int fillBulletStartIndex = 0;


		private Timer  waveTimer = new Timer();
		private float waveDuration = 4;

		private Timer betweenWaveSpawnTimer = new Timer();
		private float betweenWaveDuration = 0.1f;
		private float rotationSpeed = MathF.Tau / 4;


		private List<int> colorIndexList = new List<int>()
		{
			5, 7, 0, 2, 4, 6, 8, 1, 3, 5, 7, 0, 2, 4,
			5, 7, 0, 2, 4, 6, 8, 1, 3, 5, 7, 0, 2, 4,
			5, 7, 0, 2, 4, 6, 8, 1, 3, 5, 7, 0, 2, 4,
			5, 7, 0, 2, 4, 6, 8, 1, 3, 5, 7, 0, 2, 4,
		};

		private int currentColorListProgression = 0;



		private List<int> fullBulletListIndex = new List<int>() ;


		private List<Action> attacks;

		private List<float> renderedRotationSpeed = new List<float>() { Mathf.Tau/10, -Mathf.Tau / 5, Mathf.Tau / 2, } ;

        private List<Action> phaseMove = new List<Action>()
        {

        } ;

		private float globalDelta = 0;

        private BossA():base()
        {
            if (instance != null || IsInstanceValid(instance))
            {
                instance.QueueFree();
                GD.Print(nameof(BossA) + " Instance already exist, destroying the last added.");
				return;
			}

			instance = this;

			for (int i = 0; i < GameManager.GetInstance().colorNumber; i++) 
			{
				fullBulletListIndex.Add(i);
			}

		}

		static public BossA GetInstance()
		{
			if (instance == null) instance = (BossA)factory.Instantiate();
			return instance;

		}

		public override void _Ready()
		{
			base._Ready();

            yAmplitude = (CameraManager.GetInstance().GetLevelYBondaries().Y - CameraManager.GetInstance().GetLevelYBondaries().X) / 2 - amplitudeMargin;

            baseYPosition = 0;


			for (int i = 0; i < rendererNode.GetChildCount(); i++)
			{
				if (i!=0) ((Sprite2D)rendererNode.GetChild(i)).Visible = false;
				((Spiner)rendererNode.GetChild(i)).spinSpeed = renderedRotationSpeed[i];
			}



            colorIndex = colorIndexList[currentColorListProgression];
            SetColorToIndex();

            attacks = new List<Action>()
			{
				(System.Action)LaunchBulletPhaseOne,
				(System.Action)LaunchBulletPhaseTwo,
				(System.Action)LaunchBulletPhaseThree

			};

            afterLaunchTimer.WaitTime = afterLaunchDuration;
			afterLaunchTimer.Autostart = false;
			afterLaunchTimer.OneShot = true;

			afterLaunchTimer.Timeout += FillBullet;
			AddChild(afterLaunchTimer);

			waveTimer.WaitTime = waveDuration;
			waveTimer.Autostart = false;
			waveTimer.OneShot = true;
			waveTimer.Timeout += betweenWaveSpawnTimer.Stop;
			waveTimer.Timeout += () => isMoving = true;

			AddChild(waveTimer);

			betweenWaveSpawnTimer.WaitTime = betweenWaveDuration;
			betweenWaveSpawnTimer.Autostart = false;
			betweenWaveSpawnTimer.OneShot = false;
			betweenWaveSpawnTimer.Timeout += LaunchNextWave;
			AddChild(betweenWaveSpawnTimer); 

            FillBullet();

			SetProcess(false);

			phaseMove = new List<Action>
			{
				MovePhaseOne, MovePhaseTwo, MovePhaseThree,
			};
		}

		public override void _Process(double pDelta)
		{
			base._Process(pDelta);
			float lDelta = (float)pDelta;

			if (!waveTimer.IsStopped())
			{
				Rotate(-rotationSpeed*lDelta);
			}

            globalDelta = lDelta;

            if (!BossManager.GetInstance().isFinished)
            {

                phaseMove[BossManager.GetInstance().currentPhase].Invoke();



                NaturalScrolling(lDelta);
            }
        }

        public void ResetPosition()
		{
            TeleporteTo(basePosition);
		}

        public Tween TeleporteTo(Vector2 pPosition)
        {

            Tween lTPTween = CreateTween()
                .SetTrans(Tween.TransitionType.Expo)
                .SetEase(Tween.EaseType.Out);

            lTPTween.TweenProperty(this, "scale", Vector2.Zero, 0.1f);

            lTPTween.TweenCallback(
                Callable.From(() =>
                {
                    GlobalPosition = pPosition;
                }
            ));

            lTPTween.SetTrans(Tween.TransitionType.Elastic).SetEase(Tween.EaseType.Out);

            lTPTween.TweenProperty(this, "scale", Vector2.One, 0.5f);


            return lTPTween;
        }


        private void MovePhaseOne()
		{
			GlobalPosition = basePosition;
		}

		private void MovePhaseTwo()
		{
            GlobalPosition = basePosition + new Vector2(
           0,
           Mathf.Cos(timeProgression) * yAmplitude
           );

            if (isMoving) { timeProgression += globalDelta * timeProgressionSpeed; }


        }

        private void MovePhaseThree()
		{
            GlobalPosition = basePosition + new Vector2(
               Mathf.Sin(timeProgression * 2) * xAmplitude,
               Mathf.Cos(timeProgression) * yAmplitude
               );

            if (isMoving) { timeProgression += globalDelta * timeProgressionSpeed; }


        }


        protected override void NaturalScrolling(float pDelta)
        {
			
            basePosition += Vector2.Right * LevelManager.currentLevelSpeed * pDelta;
        }

        public void Start()
		{
			SetProcess(true);

            basePosition = GlobalPosition;

            ResetPosition();

        }

        public void Attack()
		{
			attacks[BossManager.GetInstance().currentPhase].Invoke();
		}

		protected override void Dispose(bool pDisposing)
		{
			instance = null;
			base.Dispose(pDisposing);
		}


		public void LaunchBulletPhaseOne()
		{
			foreach (BossBullet lBossBullet in enemyOneContainer.GetChildren())
			{
				lBossBullet.Rotation = Vector2.Left.Angle();
				lBossBullet.direction = Vector2.FromAngle(lBossBullet.Rotation);
				lBossBullet.Start();
                lBossBullet.Reparent(GameManager.GetInstance().GetBulletContainer());

            }

            afterLaunchTimer.Start();
		}

        public void LaunchBulletPhaseTwo()
        {
            foreach (BossBullet lBossBullet in enemyOneContainer.GetChildren())
            {

				lBossBullet.LookAt(Player.GetInstance().GlobalPosition);
				lBossBullet.direction = Vector2.FromAngle(lBossBullet.GlobalRotation);
                lBossBullet.speed *= 1.5f;
                lBossBullet.Start(); 
				lBossBullet.Reparent(GameManager.GetInstance().GetBulletContainer());


            }

            afterLaunchTimer.Start();
        }

        public void LaunchBulletPhaseThree()
        {
			launchEnemyOnePhaseThreeProgression = 0;
			LaunchWave();
            LaunchNextEnemyOne();
        }

		public void LaunchNextEnemyOne()
		{
			EnemyOne lEnemyOne;


			for (int i = 0;i<GameManager.GetInstance().colorNumber;i++) {

				
				lEnemyOne = EnemyOne.Create();



                lEnemyOne.colorIndex = i;
				lEnemyOne.SetColorToIndex();

                LevelManager.GetInstance().gameLayer.AddChild(lEnemyOne);
                lEnemyOne.GlobalPosition = GlobalPosition + Vector2.Right.Rotated(MathF.Tau * (i) / 9) * enemyOneSpawnRadius;


                lEnemyOne.FullStart();
				lEnemyOne.Appear();



            }
        }

		private void CreateBullet()
		{
            BossBullet lTempBullet;

            lTempBullet = BossBullet.Create();


            lTempBullet.colorIndex = colorIndex;
            lTempBullet.SetColorToIndex();


            enemyOneContainer.AddChild(lTempBullet);
            lTempBullet.Position = Vector2.Right.Rotated(MathF.Tau * (launchEnemyOnePhaseThreeProgression) / GameManager.GetInstance().colorNumber) * enemyOneSpawnRadius;
            lTempBullet.Rotation = (MathF.Tau * launchEnemyOnePhaseThreeProgression / GameManager.GetInstance().colorNumber);
            lTempBullet.Stop();


            lTempBullet.Appear();
        }
		


        public void FillBullet()
		{
            BossBullet lTempBullet;

            fillBulletStartIndex = GD.RandRange(0, 8);

			int lI;

            for (int i = 0; i < GameManager.GetInstance().colorNumber; i++)
			{
				lI = i + fillBulletStartIndex;
				lTempBullet = BossBullet.Create();


                lTempBullet.colorIndex = colorIndex;
                lTempBullet.SetColorToIndex();


                enemyOneContainer.AddChild(lTempBullet);
                lTempBullet.Position = Vector2.Right.Rotated(MathF.Tau * (lI) / 9) * enemyOneSpawnRadius;
                lTempBullet.Rotation = (MathF.Tau * lI / 9);
                lTempBullet.Stop();


                lTempBullet.Appear();


			}

        }

		public void LaunchWave()
		{
            isMoving = false;

            waveTimer.Start();
			betweenWaveSpawnTimer.Start();
        }

		private void LaunchNextWave()
		{
            BossBullet lNewBossBullet;


            foreach (BossBullet lBossBullet in enemyOneContainer.GetChildren().OfType<EnemyBullet>())
            {
                lNewBossBullet = (BossBullet)lBossBullet.Duplicate();

                lNewBossBullet.GlobalPosition = lBossBullet.GlobalPosition;

                lNewBossBullet.Rotation = lBossBullet.GlobalRotation + Mathf.Pi/2;
				lNewBossBullet.direction = Vector2.FromAngle(lBossBullet.GlobalRotation);
                GameManager.GetInstance().GetBulletContainer().AddChild(lNewBossBullet);
                lNewBossBullet.speed *= 1.5f;

            }
        }


		public void NextColor()
		{
            if (BossManager.GetInstance().currentPhase == 2) return;

			currentColorListProgression++;
			colorIndex = colorIndexList[currentColorListProgression];
			SetColorToIndex();
		}




		public void CreateBumpExplosion()
		{
			BossBumpExplosion lBossBumpExplosion = BossBumpExplosion.Create();
            SoundManager.GetInstance().bossShoot.Play();


            lBossBumpExplosion.GlobalPosition = GlobalPosition;
			lBossBumpExplosion.colorIndex = colorIndex;
			lBossBumpExplosion.SetColorToIndex();

			GameManager.GetInstance().GetExplosionContainer().AddChild(lBossBumpExplosion);

		}


		public void ChangeForLastPhase()
		{
            basePosition -= Vector2.Right *200;
			color = new Color(1, 1, 1);
			foreach (BossBullet lBossBullet in enemyOneContainer.GetChildren())
			{
				lBossBullet.QueueFree();
			}


            BossBullet lTempBullet;

            fillBulletStartIndex = GD.RandRange(0, 8);

            int lI;

            for (int i = 0; i < GameManager.GetInstance().colorNumber; i++)
            {
                lI = i + fillBulletStartIndex;
                lTempBullet = BossBullet.Create();


                lTempBullet.colorIndex = lI;
                lTempBullet.SetColorToIndex();


                enemyOneContainer.AddChild(lTempBullet);
                lTempBullet.Position = Vector2.Right.Rotated(MathF.Tau * (lI) / 9) * enemyOneSpawnRadius;
                lTempBullet.Rotation = (MathF.Tau * lI / 9);
                lTempBullet.Stop();


                lTempBullet.Appear();


            }
        }


        public void Bump()
        {
            bumpTween?.Kill();
            bumpTween = CreateTween()
                .SetTrans(Tween.TransitionType.Expo)
                .SetEase(Tween.EaseType.In)

                ;

            bumpTween.TweenProperty(this, "scale", bumpTweenScale, bumpTweenDuration / 2);

            bumpTween
                .SetTrans(Tween.TransitionType.Expo)
                .SetEase(Tween.EaseType.Out)

                ;
            bumpTween.TweenProperty(this, "scale", Vector2.One, bumpTweenDuration / 2);
        }

        public void Shake()
        {
            shakeTween?.Kill();

            shakeTween = CreateTween()
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.Out);

            shakeTween.TweenProperty(this, "rotation", shakeTweenAngle / 2, shakeTweenDuration / 4 / shakeNumber).AsRelative();

            shakeTween
                .SetEase(Tween.EaseType.InOut);

            for (int i = 0; i < shakeNumber - 1; i++)
            {
                shakeTween.TweenProperty(this, "rotation", -shakeTweenAngle, shakeTweenDuration / 2 / shakeNumber).AsRelative();
                shakeTween.TweenProperty(this, "rotation", shakeTweenAngle, shakeTweenDuration / 2 / shakeNumber).AsRelative();
            }

            shakeTween.TweenProperty(this, "rotation", 0, shakeTweenDuration / 4 / shakeNumber).AsRelative();

        }

        protected override void OnAreaEntered(Area2D pArea)
        {
            base.OnAreaEntered(pArea);

            ColoredEntity lCollidedColoredEntity = (ColoredEntity)pArea.GetParent();


            if (lCollidedColoredEntity is Player)
            {
                Player.GetInstance().GetDamaged(collideDamage);

                return;
            }

            if (lCollidedColoredEntity is Bullet)
            {
                GetDamaged(((Bullet)lCollidedColoredEntity).damage);
                lCollidedColoredEntity.QueueFree();

                return;
            }

            if (lCollidedColoredEntity is SmartBomb)
            {
                GetDamaged(((SmartBomb)lCollidedColoredEntity).damage);
                return;
            }
        }

        public override void GetDamaged(float pDamage)
        {
            DoFlash();

            BossManager.GetInstance().hp -= pDamage;

			UIManager.GetInstance().hud.UpdateBossHealthBar();

        }


		public void EmitteParticules()
		{
			explosionParticules.SetProcess(true);
		}



    }
}
