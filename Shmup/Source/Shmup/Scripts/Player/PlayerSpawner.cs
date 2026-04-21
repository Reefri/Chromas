using Godot;
using System;
using System.Collections.Generic;

// Author : Sacha Gramatikoff

namespace Com.IsartDigital.Chromaberation
{
	
	public partial class PlayerSpawner : Node2D
	{

		private const string PLAYERSPAWNER_SCENE_PATH = "res://Scenes/Spawner/PlayerSpawner.tscn";
		private static PackedScene playerSpawnerFactory = (PackedScene)GD.Load(PLAYERSPAWNER_SCENE_PATH);

        static private PlayerSpawner instance;

		private Timer betweenInputTimer = new Timer();
		private float bewteenInputDuration = 0.3f;

		private Timer betweenCreationTimer = new Timer();
		private float betweenCreationDuration = 1f;
		private float betweenCreationAcceleration = 2f;


        private Timer waitBeforeStartingLevelTimer = new Timer();
		private float waitBeforeStartingLevelDuration = 4;

		private Timer waitBeforeShowingHUDTimer = new Timer();
		private float waitBeforeShowingHUDDuration = 2f;

		private Player player;
		private List<Vector2> playerRendererPosition = new List<Vector2>();
		private int indexOfCurrentRenderer = 0;

		private bool isPlayerCreated = false;

		private Tween rendererTween;

		private float distanceRenderer = 100000;

		private List<String> actionInputs;
        private int numberOfInput;


		[Export] private bool doSpawn = true;


        private PlayerSpawner() : base()
        {
            if (instance != null || IsInstanceValid(instance))
            {
                instance.QueueFree();
                GD.Print(nameof(PlayerSpawner) + " Instance already exist, destroying the last added.");
                return;
            }

            instance = this;
        }

        static public PlayerSpawner GetInstance()
		{
			if (instance == null) instance = (PlayerSpawner)playerSpawnerFactory.Instantiate();
			return instance;

		}

		public override void _Ready()
		{


			betweenInputTimer.WaitTime = bewteenInputDuration;
			betweenInputTimer.Autostart = false;
			betweenInputTimer.OneShot = true;

            betweenInputTimer.Timeout += StopCreatingPlayer;
			AddChild(betweenInputTimer);



			LevelManager.GetInstance().StopLevel();


			betweenCreationTimer.WaitTime = betweenCreationDuration;
			betweenCreationTimer.Autostart = false;
			betweenCreationTimer.OneShot = false;

			betweenCreationTimer.Timeout += PlayerRendererTween;
			AddChild(betweenCreationTimer);

			waitBeforeStartingLevelTimer.WaitTime = waitBeforeStartingLevelDuration;
			waitBeforeStartingLevelTimer.Autostart = false;
			waitBeforeStartingLevelTimer.OneShot = true;

			waitBeforeStartingLevelTimer.Timeout += FinishCreatingPlayer;
			AddChild(waitBeforeStartingLevelTimer);


			waitBeforeShowingHUDTimer.WaitTime = waitBeforeShowingHUDDuration;
			waitBeforeShowingHUDTimer.Autostart=false;
			waitBeforeShowingHUDTimer.OneShot=true;

			waitBeforeShowingHUDTimer.Timeout += ShowHud;
			AddChild(waitBeforeShowingHUDTimer);

			actionInputs = new List<string>
			{
				ControlProp.UP,
				ControlProp.DOWN,
				ControlProp.LEFT,
				ControlProp.RIGHT,
				ControlProp.SF,
				ControlProp.SHOOT,
				ControlProp.SMARTBOMB,
			};

			numberOfInput = actionInputs.Count;
		}


		public void Start()
		{
			if (!Main.GetInstance().isFirstTimePlaying || !doSpawn) 
			{
				FinishCreatingPlayer();

                return; 
			}



            CutPlayer();
        }


		protected override void Dispose(bool pDisposing)
		{
			instance = null;
			base.Dispose(pDisposing);
		}

        public override void _Input(InputEvent pEvent)
        {
            if (Main.GetInstance().isFirstTimePlaying && doSpawn)
            {

                for (int i = 0; i < numberOfInput; i++)
				{
					if (Input.IsActionJustPressed(actionInputs[i]))
					{
						betweenInputTimer.Start();
						StartCreatingPlayer();
					}
				}
			}
        }

		private void CutPlayer()
		{
			ShuffleChildren();

            foreach (Node2D lRendererChild in Player.GetInstance().GetRenderer().GetChildren())
            {
                playerRendererPosition.Add(lRendererChild.Position);
            }

            float randAngle;
			foreach (Node2D lRendererChild in Player.GetInstance().GetRenderer().GetChildren())
			{
				randAngle = GD.Randf() * MathF.Tau;

				lRendererChild.Position = new Vector2(MathF.Cos(randAngle), MathF.Sin(randAngle)) * distanceRenderer;
			}

 

            indexOfCurrentRenderer = 0;
		}

		private void ShuffleChildren()
		{
			List<Sprite2D> lChildren = new List<Sprite2D>();

			foreach (Node2D lChild in Player.GetInstance().GetRenderer().GetChildren())
			{
				if (lChild is Sprite2D)
				{
					lChildren.Add((Sprite2D)lChild);
				}
			}


            RandomNumberGenerator lRng = new RandomNumberGenerator();


            for (int i = lChildren.Count - 1; i > 0; i--)
            {
                int j = lRng.RandiRange(0,i);
                (lChildren[i], lChildren[j]) = (lChildren[j], lChildren[i]);
            }

            for (int i = 0; i < lChildren.Count; i++)
                Player.GetInstance().GetRenderer().MoveChild(lChildren[i], i);

        }

		private void StartCreatingPlayer()
		{
			if (isPlayerCreated) return;

            if (betweenCreationTimer.TimeLeft == 0)
				betweenCreationTimer.Start();

			if (!CameraManager.GetInstance().isZooming)
				CameraManager.GetInstance().ActivateZooming();
		}

		private void StopCreatingPlayer()
		{

            betweenCreationTimer.Stop();

			CameraManager.GetInstance().DeactivateZooming();

		}

        private void FinishCreatingPlayer()
		{

			LevelManager.GetInstance().StartLevel();
			waitBeforeShowingHUDTimer.Start();

		}

		private void ShowHud()
		{
			UIManager.GetInstance().hud.Enable();
			QueueFree();

        }


        private void PlayerRendererTween()
		{

			betweenCreationTimer.WaitTime /= betweenCreationAcceleration;

            LevelManager.GetInstance().creatingPlayerShaker.Start();
			SoundManager.GetInstance().construction.Play();

            Node2D lRendererChild = (Node2D)(Player.GetInstance().GetRenderer().GetChild(indexOfCurrentRenderer));
			Vector2 lPosition = playerRendererPosition[indexOfCurrentRenderer];


            rendererTween = CreateTween()
                .SetTrans(Tween.TransitionType.Elastic)
                .SetEase(Tween.EaseType.Out);

			rendererTween.TweenProperty(lRendererChild, "position", lPosition, 0.5f);


			

            indexOfCurrentRenderer++;

            if (indexOfCurrentRenderer == playerRendererPosition.Count)
            {
				betweenCreationTimer.Stop();

				isPlayerCreated = true;
                Main.GetInstance().isFirstTimePlaying = false;

				((SpamControl)GetNode("SpamControls")).HideSpamControl();

                StopCreatingPlayer();

                betweenInputTimer.Timeout -= StopCreatingPlayer;

                betweenCreationTimer.Timeout -= PlayerRendererTween;



                waitBeforeStartingLevelTimer.Start();


                return;
            }
        }
    }
}
