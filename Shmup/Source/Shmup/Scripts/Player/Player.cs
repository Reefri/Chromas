using Godot;
using System;
using System.Collections.Generic;

// Author : Sacha Gramatikoff

namespace Com.IsartDigital.Chromaberation {
	
	public partial class Player : KillableEntity
	{


        private const string SMARTBOMB_SCENE_PATH = "res://Scenes/SmartBomb.tscn";
		private PackedScene smartBombFactory = (PackedScene)GD.Load(SMARTBOMB_SCENE_PATH);

        private const string BULLET_SCENE_PATH = "res://Scenes/ColoredEntity/Bullet.tscn";
		private PackedScene bulletFactory = (PackedScene)GD.Load(BULLET_SCENE_PATH);

		private const string COLORWHEEL_SCENES_PATH = "res://Scenes/ColorWheel/ColorWheel.tscn";
		private PackedScene colorWheelFactory = (PackedScene)GD.Load(COLORWHEEL_SCENES_PATH);
        public float wheelRadius = 100;
		public float wheelOffset = Mathf.Pi;


		private const string RIPPLEEFFECTCONTAINER_NODE_PATH = "CamouflageShaderSupportContainer";


		private const string RIPPLEEFFECTSHADER_SCENE_PATH = "res://Scenes/RippleEffect.tscn";
		private PackedScene rippleEffectFactory = (PackedScene)GD.Load(RIPPLEEFFECTSHADER_SCENE_PATH);

		//[Export] PackedScene rippleEffectFactory;


        [Signal] public delegate void OnCamouflageEventHandler();
        [Signal] public delegate void OnNotCamouflageEventHandler();


		public int score = 0;



        private	const string BULLETCONTAINER_NODE_PATH = "BulletContainer";



		private Bullet lastBullet;
		private float marginBeforeShooting = 200;
		private bool canShot = false;


        public float YSpeed = 0;
        public float XSpeed = 0;
        public float maxSpeed = 700;
        public float acceleration = 14000;




		private int maxLevel = 2;
        [Export] public int level = 0;

		private bool godMode = false;

		[Export] private ColorWheel colorWheel;

		[Export] private Node2D bulletSpawnPosition;


		public float timeProgression;


		private const string HIDINGSHADERSUPORT_NODE_PATH = "CamouflageShaderSuport";
		ShaderMaterial camouflageShaderMateriel;

		private RippleEffect currentCamouflageShader;



		public bool isCamouflage = false;
        private List<ColorBlock> hideBlocks = new List<ColorBlock>();

		private bool canUseSmartbomb = true;
		private Timer smartbombCouldown = new Timer();
		private float smartbombCouldownDuration = 0.7f;

		private int maxSmartBombCount = 3;
		private int smartBombCount = 3;


		[Export] public bool isAlive = false;

		public bool canDie = true;

        static private Player instance;
        const string PLAYER_SCENE_PATH = "res://Scenes/ColoredEntity/Player.tscn";
        static private PackedScene playerFactory = (PackedScene)GD.Load(PLAYER_SCENE_PATH);

		private bool isRotating = false;



		[Export] Node2D particulesContainer;

		[Export] PackedScene healParticulesFactory;
		[Export] PackedScene levelUpParticulesFactory;
		[Export] PackedScene bombParticulesFactory;


		[Export] Node2D weaponContainer;


		private List<float> bulletDamageCoef = new List<float>() {1.9f,0.7f,0.4f };

        private Player() : base()
        {
            if (instance != null || IsInstanceValid(instance))
            {
                instance.QueueFree();
                GD.Print(nameof(Player) + " Instance already exist, destroying the last added.");
                return;
            }

            instance = this;
        }

        static public Player GetInstance()
        {
            if (instance == null) instance = (Player)playerFactory.Instantiate();
            return instance;

        }


		public override void _Ready()
		{



            color = new Color(1,0,0);

            base._Ready();

			hp = 100;

			colorWheel.radius = wheelRadius;



			smartbombCouldown.WaitTime = smartbombCouldownDuration;
			smartbombCouldown.Autostart = false;
			smartbombCouldown.OneShot = true;

			smartbombCouldown.Timeout += () => canUseSmartbomb = true;
			AddChild(smartbombCouldown);
        }



		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

			base._Process(lDelta);


			
            XSpeed += (direction.X - XSpeed / maxSpeed) * acceleration * lDelta;
            YSpeed += (direction.Y - YSpeed / maxSpeed) * acceleration * lDelta;


			if (GlobalPosition.X < CameraManager.GetInstance().GetLevelXBondaries().X && XSpeed < 0 || 
				GlobalPosition.X > CameraManager.GetInstance().GetLevelXBondaries().Y && XSpeed > 0) { XSpeed = 0; }

			if (GlobalPosition.Y < CameraManager.GetInstance().GetLevelYBondaries().X && YSpeed < 0 ||
				GlobalPosition.Y > CameraManager.GetInstance().GetLevelYBondaries().Y && YSpeed > 0) { YSpeed = 0; }

			NaturalScrolling(lDelta);



            if (canShot)
			{


				if (!IsInstanceValid(lastBullet))
				{
					lastBullet = CreateBullet();
				}
				else if(!lastBullet.isDead) 
				{
					if (lastBullet.GlobalPosition.X - GlobalPosition.X > marginBeforeShooting) lastBullet = CreateBullet();
				}
				else
				{
					lastBullet = CreateBullet();
				}
			}

        }

        protected override void Dispose(bool pDisposing)
		{
			instance = null;

            base.Dispose(pDisposing);
		}



        public override void _Input(InputEvent pEvent)
        {
			if (!isAlive) { return; }

			direction = new Vector2();
			if (Input.IsActionPressed(ControlProp.UP))
			{
				direction += Vector2.Up;
			}
			if (Input.IsActionPressed(ControlProp.DOWN))
			{
				direction += Vector2.Down;

			}
			if (Input.IsActionPressed(ControlProp.LEFT))
			{
				direction += Vector2.Left;

			}
			if (Input.IsActionPressed(ControlProp.RIGHT))
			{
				direction += Vector2.Right;
			}



			direction += new Vector2(Input.GetJoyAxis(0, JoyAxis.LeftX), Input.GetJoyAxis(0, JoyAxis.LeftY));

			if (direction.DistanceTo(Vector2.Zero)< 0.2f) {  direction = Vector2.Zero; }


            direction = direction.Normalized();

			if (Input.IsActionJustPressed(ControlProp.GODMODE))
			{
				godMode = !godMode;
				UIManager.GetInstance().hud.godLabel.Visible = godMode;
			}
			

			if (Input.IsActionJustReleased(ControlProp.SMARTBOMB) && canUseSmartbomb)
			{
				CallSmartBomb();
			}


			if (Input.IsActionPressed(ControlProp.SHOOT))
			{ StartShooting(); }
			else
			{ StopShooting(); }



            if (Input.IsActionJustPressed(ControlProp.SF) && !isRotating)
            {
				isRotating = true;
                colorWheel.StartRotating();
            }
            if (Input.IsActionJustReleased(ControlProp.SF) && isRotating)
            {
				isRotating = false;
                colorWheel.StopRotating();
            }


        }

        private void EmitOnCamouflage()
		{
            EmitSignal(SignalName.OnCamouflage);
        }
		private void EmitOnNotCamouflage()
		{
            EmitSignal(SignalName.OnNotCamouflage);
        }

		public void MakeAlive()
		{
			isAlive = true;
		}

        public override void Move(float pDelta)
        { Position += new Vector2(XSpeed, YSpeed) * pDelta; }


		private void StartShooting()
		{ canShot = true; }

		public void StopShooting()
		{ canShot = false; }



		private Bullet CreateBullet()
		{
			Bullet lBullet = new Bullet();

			SoundManager.GetInstance().PlayPlayerSHoot();

			bool lUpDown;

            for (int i = level; i >=0; i--)
			{
				lUpDown = false;
				foreach (Node2D lChild in bulletSpawnPosition.GetChild(i).GetChildren())
				{
					lBullet = (Bullet)bulletFactory.Instantiate();
					lBullet.damage *= bulletDamageCoef[i];

                    lBullet.Position = lChild.GlobalPosition;
					if (level == 2 && i == level) 
					{
						lBullet.color = ColorManager.ColorRotation(color,2f/9 * (lUpDown?1:-1) );
					}
					else
					{
                        lBullet.color = color;

                    }

					lUpDown = true;

                    GameManager.GetInstance().GetNode(BULLETCONTAINER_NODE_PATH).AddChild(lBullet);
				}
			}

			return lBullet;
        }


		private void CallSmartBomb()
		{
			if (smartBombCount == 0 && !godMode) { return; }
			canUseSmartbomb = false;
			SmartBomb lSmartBomb = (SmartBomb)smartBombFactory.Instantiate();
			lSmartBomb.GlobalPosition = GlobalPosition;

			GameManager.GetInstance().GetExplosionContainer().AddChild(lSmartBomb);

			if (!godMode) 
			{
				smartBombCount--;
                UIManager.GetInstance().hud.RemoveSmartBomb(smartBombCount);
            }

			smartbombCouldown.Start();
			SoundManager.GetInstance().PlaySmartBomb();
		}



		public void LevelUp()
		{

            level = Math.Clamp(level + 1, 0, maxLevel);

            particulesContainer.AddChild(levelUpParticulesFactory.Instantiate());


            ((Sprite2D)weaponContainer.GetChild(level-1)).Visible = true;



        }
        public void LevelDown()
		{

            level = Math.Clamp(level - 1, 0, maxLevel);
            ((Sprite2D)weaponContainer.GetChild(level)).Visible = false;

        }

        public void Heal(float pHeal)
		{
            hp = Math.Clamp(hp + pHeal, 0, maxHp);
            UpdateHealth();

			particulesContainer.AddChild(healParticulesFactory.Instantiate());



        }

        public void AddSmartBomb()
		{
            if (smartBombCount != maxSmartBombCount)
			{
                UIManager.GetInstance().hud.AddSmartBomb(smartBombCount);
                smartBombCount++;
            }

			particulesContainer.AddChild(bombParticulesFactory.Instantiate());

        }


        protected override void OnAreaEntered(Area2D pArea)
        {
            base.OnAreaEntered(pArea);

			ColoredEntity lCollidedColoredEntity = (ColoredEntity)pArea.GetParent();



			if (lCollidedColoredEntity is ColorBlock)
			{
				hideBlocks.Add((ColorBlock)pArea.GetParent());
				ShouldDoCamouflage();

                return;
			}



			if((lCollidedColoredEntity is Enemy )&& !isCamouflage)
			{
				GetDamaged(((Enemy)lCollidedColoredEntity).collisionDamage);
                return;

            }




        }


        protected override void OnAreaExited(Area2D pArea)
		{
			base.OnAreaExited(pArea);

            ColoredEntity lCollidedColoredEntity = (ColoredEntity)pArea.GetParent();


            if (lCollidedColoredEntity is ColorBlock)
			{
				hideBlocks.Remove((ColorBlock)lCollidedColoredEntity);

				ShouldDoCamouflage();
            }


        }

		public void ShouldDoCamouflage()
		{
            if (CheckIsCamouflage() && !isCamouflage) Camouflage();
            if (!CheckIsCamouflage() && isCamouflage) StopCamouflage();
        }

        private void Camouflage()
        {
			isCamouflage = true;
            CamouflageShader();
			EmitOnCamouflage();
        }

        private void StopCamouflage()
        {
			isCamouflage = false;
			StopCamouflageShader();

			EmitOnNotCamouflage();
        }

        private bool CheckIsCamouflage()
		{
            foreach (ColorBlock lColorBlock in hideBlocks)
            {
                if (ColorManager.AreColorsCloseEnough(lColorBlock.color, color))
                {
					return true;
                }
            }

			return false;
        }

		



		private void CamouflageShader()
		{
            CanvasLayer lRippleScene = (CanvasLayer)rippleEffectFactory.Instantiate();

			currentCamouflageShader = (RippleEffect)lRippleScene.GetChild(0);


            currentCamouflageShader.centerPosition = GetGlobalTransformWithCanvas().Origin/GetViewportRect().Size;

			SoundManager.GetInstance().camouflage_IN.Play();


            Main.GetInstance().postProcessingNode.
				AddChild(lRippleScene);
		}

		private void StopCamouflageShader()
		{
			currentCamouflageShader.StopCamouflage();
            SoundManager.GetInstance().camouflage_OUT.Play();


        }



        public void KnockBack(float pStrength, Node2D lNode, bool pFullX)
		{
			Vector2 lKnockbackDirection = (lNode.GlobalPosition - GlobalPosition).Normalized();

			XSpeed = pStrength * maxSpeed * (pFullX? 1 : lKnockbackDirection.X);
			YSpeed = pFullX? 0 : pStrength * maxSpeed * lKnockbackDirection.Y ;
		}

        public override void GetDamaged(float pDamage)
        {
			if (!godMode)
			{
				base.GetDamaged(pDamage);
				SoundManager.GetInstance().loselife.Play();
				UpdateHealth();
				LevelDown();

            }
			else DoFlash();
        }

		public Node2D GetRenderer()
		{
			return rendererNode;
		}


		public void HideColorWheel()
		{
			colorWheel.Hide();
		}

		public void ShowColorWheel()
		{
			colorWheel.Show();
		}

		public void UpdateHealth()
        {
            UIManager.GetInstance().hud.UpdateHealthBar();
            LevelManager.GetInstance().UpdatePixelShader(hp / maxHp);
        }

		protected override void UpdateColor()
		{
			base.UpdateColor();
			UIManager.GetInstance().hud.ChangeColor(color);
		}

		public void ScorePoints(int pScore)
		{
			score += pScore;
			UIManager.GetInstance().hud.UpdateScore(score);
		}

        public override void Destroy()
        {
			if (canDie)
			{
                SoundManager.GetInstance().playerExplosion.Play();

				isAlive = false;
				canDie = false;

				isRotating = false;
				direction = Vector2.Zero;
				canShot = false;

				UIManager.GetInstance().gameOver.Enable();
			}

        }
	}
}
