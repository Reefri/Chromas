using Godot;
using System.Collections.Generic;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class UIManager : Control
	{

		[Export] private PauseMenu pauseMenu;
        [Export] public LoadingScreen loadingScreen;
        [Export] private ColorRect isartLogo;
        [Export] private TitleScreen titleScreen;
        [Export] private HelpScreen helpScreen;

        [Export] public HUD hud;

        [Export] public Credits creditsScreen;

        [Export] private ColorRect background;

        [Export] public GameOver gameOver;

        private const string UIMANAGER_SCENE_PATH = "res://Scenes/UI/UIManager.tscn";
        private static PackedScene uiManagerFactory = (PackedScene)GD.Load(UIMANAGER_SCENE_PATH);

        private static UIManager instance;

        public bool canPause = true;


        private bool isMouseMooving = false;


        private Timer launchTimer = new Timer();
        private List<System.Action> launchSteps;
        private List<float> launchStepsTime;
        private int launchProgression = 0;

        private Timer startGameTimer = new Timer();
        private List<System.Action> startGameSteps;
        private List<float> startGameStepsTime;
        private int startGameProgression = 0;

        private Timer retryFromGameOverTimer = new Timer();
        private List<System.Action> retryFromGameOverSteps;
        private List<float> retryFromGameOverStepsTime;
        private int retryFromGameOverProgression = 0;


        private Timer retryFromPauseTimer = new Timer();
        private List<System.Action> retryFromPauseSteps;
        private List<float> retryFromPauseStepsTime;
        private int retryFromPauseProgression = 0;




        private Color selectedColorMode;

        private bool toggleAbleToMoveMouse = true;


        public bool canGameOver = true;

        private UIManager() : base()
        {
            if (instance != null || IsInstanceValid(instance))
            {
                instance.QueueFree();
                GD.Print(nameof(UIManager) + " Instance already exist, destroying the last added.");
                return;
            }

            instance = this;


        }

        static public UIManager GetInstance()
        {
            if (instance == null) instance = (UIManager)uiManagerFactory.Instantiate();
            return instance;

        }


        public override void _Ready()
        {

            instance = this;



            canPause = false;

            pauseMenu.Disable();
			ZIndex = 100;


            titleScreen.Disable();
            helpScreen.Disable();
            creditsScreen.Disable();

            int lZindex = loadingScreen.ZIndex ;

            helpScreen.ZIndex = lZindex-1;

            isartLogo.ZIndex = lZindex-1;

            gameOver.ZIndex = lZindex+1;

            isartLogo.Show();

            launchSteps = new List<System.Action>
            {
                (System.Action)(() => loadingScreen.CloseTo(0.5f,0)),
                (System.Action)(() => loadingScreen.CloseTo(0)),
                (System.Action)(isartLogo.Hide),
                (System.Action)(titleScreen.Enable),
                (System.Action)(background.Hide),

            };

            launchStepsTime = new List<float>
            {
                3f,
                3f,
                1f,
                1f,
                1f,
                1f,
            };

            startGameSteps = new List<System.Action>
            {
                (System.Action)(titleScreen.Disable),
                (System.Action)(helpScreen.Enable),

                (System.Action)(() => loadingScreen.CloseTo(0.75f)),

                (System.Action)(() => loadingScreen.CloseTo(0)),
                (System.Action)(helpScreen.Disable),

                (System.Action)(() => Main.GetInstance().AddChild(GameManager.GetInstance())),
                (System.Action)(SoundManager.GetInstance().StopUI),
                (System.Action)((hud).Restart),
                (System.Action)(() => SetMouseTo(false)),
                (System.Action)(() => loadingScreen.CloseTo(2)),

                (System.Action)(() => canPause = true),
                (System.Action)(() => canGameOver = true),
            };

            startGameStepsTime = new List<float>
            {
                0.01f,
                0.01f,

                0.1f,
                
                5,
                3f,
                
                0.1f,
                0.1f,
                0.1f,
                0.1f,
                0.1f,
                
                0.1f,
                0.1f,
                
                0.1f,
                1,
            };

            retryFromPauseSteps = new List<System.Action>
            {
                (System.Action)(TogglePause),
                (System.Action)(() => canPause = false),
                (System.Action)(() => canGameOver = false),
                (System.Action)(() => loadingScreen.CloseTo(0)),
                (System.Action)(() => Player.GetInstance().canDie = false),
                (System.Action)(hud.Disable),
                (System.Action)(() => GameManager.GetInstance().QueueFree()),
                (System.Action)(() => Main.GetInstance().AddChild(GameManager.GetInstance())),
                (System.Action)(SoundManager.GetInstance().StopUI),

                (System.Action)(((HUD)hud).Restart),

                (System.Action)(() => loadingScreen.CloseTo(2)),
                (System.Action)(() => canPause = true),
                (System.Action)(() => canGameOver = true),
            };

            retryFromPauseStepsTime = new List<float>
            {
                0.01f,
                0.01f,
                0.01f,
                0.01f,
                0.01f,
                2,
                0.01f,
                0.01f,
                0.01f,
                0.01f,
                0.01f,
                0.01f,
                0.01f,
                0.01f,
            };


            retryFromGameOverSteps = new List<System.Action>
            {
                (System.Action)(() => canGameOver = false),
                (System.Action)(() => canPause = false),
                (System.Action)(() => Player.GetInstance().canDie = false),
                (System.Action)(hud.Disable),
                (System.Action)(gameOver.Disable),               
                (System.Action)(() => GameManager.GetInstance().QueueFree()),
                (System.Action)(() => Main.GetInstance().AddChild(GameManager.GetInstance())),
                (System.Action)(SoundManager.GetInstance().StopUI),
                (System.Action)(((HUD)hud).Restart),

                (System.Action)(() => loadingScreen.CloseTo(2)),
                (System.Action)(() => canPause = true),
                (System.Action)(() => canGameOver = true),

            };

            retryFromGameOverStepsTime = new List<float>
            {
                0.01f,
                0.01f,
                0.01f,
                0.01f,
                0.01f,
                0.01f,
                0.01f,
                0.01f,
                0.01f,
                0.01f,
                0.01f,
                0.01f,
                0.01f,
                0.01f,
                0.01f,
 
            };

            launchTimer = Main.CreateTimer(launchStepsTime[0], true, true, LaunchNextStep);
            AddChild(launchTimer);


            startGameTimer = Main.CreateTimer(startGameStepsTime[0],false, true, StartGameNextStep);
            AddChild(startGameTimer);


            retryFromPauseTimer = Main.CreateTimer(startGameStepsTime[0], false, true, RetryFromPauseNextStep);
            AddChild(retryFromPauseTimer);

            retryFromGameOverTimer = Main.CreateTimer(startGameStepsTime[0], false, true, RetryFromGameOverNextStep);
            AddChild(retryFromGameOverTimer);


            SoundManager.GetInstance().UI.Play();


            if (Main.GetInstance().startRightAway)
            {
                launchTimer.Stop();
                CallDeferred("StartRightAway");
                SoundManager.GetInstance().StopUI();
                return;
            }


        }


        private void StartRightAway()
        {
            Main.GetInstance().AddChild(GameManager.GetInstance());
            SetMouseTo(false);
            loadingScreen.CloseTo(2);

            canPause = true;


            isartLogo.Hide();
            background.Hide();
        }

        public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

            MouseParticule.enableParticule = isMouseMooving && toggleAbleToMoveMouse;
            isMouseMooving = false;
        }


        public override void _Input(InputEvent pEvent)
		{
			base._Input(pEvent);
            if (Input.IsActionJustPressed(ControlProp.PAUSE) && canPause)
            {
                TogglePause();
            }


            if (pEvent is InputEventMouseMotion)
            {
                isMouseMooving = true;
            }
        }


  

        private void MakeVisible()
        {
            Visible = true;
        }

        private void MakeInvsible()
        {
            Visible = false;
        }

        public void TogglePause()
        {
            GetTree().Paused = !GetTree().Paused;
            if (GetTree().Paused)
            {
                pauseMenu.Enable();
            }
            else 
            {
                pauseMenu.Disable();
                SoundManager.GetInstance().StopUI();
            }
            SetMouseTo(GetTree().Paused);
        }


        public void SetMouseTo(bool pMouseState)
        {
            toggleAbleToMoveMouse = pMouseState;
            Input.MouseMode = (pMouseState ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured);
        }



        public void StartGame()
        {
            startGameTimer.Start();
        }

        public void StartRetryFromPause()
        {
            retryFromPauseProgression = 0;
            retryFromPauseTimer.Start();
        }
        public void StartRetryFromGameOver()
        {
            retryFromGameOverProgression = 0;
            retryFromGameOverTimer.Start();
        }

        private void LaunchNextStep()
        {
            if (launchProgression >= launchSteps.Count)
            {
                launchTimer.Stop();
                launchProgression = 0;

                launchTimer.WaitTime = launchStepsTime[launchProgression];
                return;

            }
            launchSteps[launchProgression].Invoke();

            launchProgression++;

            launchTimer.WaitTime = launchStepsTime[launchProgression];

            launchTimer.Start();
        }

        private void StartGameNextStep()
        {
            if (startGameProgression >= startGameSteps.Count)
            {
                startGameTimer.Stop();
                startGameProgression = 0;
                startGameTimer.WaitTime = startGameStepsTime[startGameProgression];


                return;

            }
            startGameSteps[startGameProgression].Invoke();

            startGameProgression++;
            startGameTimer.WaitTime = startGameStepsTime[startGameProgression];

            startGameTimer.Start();

        }
        private void RetryFromPauseNextStep()
        {
            if (retryFromPauseProgression >= retryFromPauseSteps.Count)
            {
                retryFromPauseTimer.Stop();
                retryFromPauseProgression = 0;
                retryFromPauseTimer.WaitTime = retryFromPauseStepsTime[retryFromPauseProgression];


                return;

            }
            retryFromPauseSteps[retryFromPauseProgression].Invoke();

            retryFromPauseProgression++;
            retryFromPauseTimer.WaitTime = retryFromPauseStepsTime[retryFromPauseProgression];

            retryFromPauseTimer.Start();

        }

        private void RetryFromGameOverNextStep()
        {
            if (retryFromGameOverProgression >= retryFromGameOverSteps.Count)
            {
                retryFromGameOverTimer.Stop();
                retryFromGameOverProgression = 0;

                retryFromGameOverTimer.WaitTime = retryFromGameOverStepsTime[retryFromGameOverProgression];


                return;

            }


            retryFromGameOverSteps[retryFromGameOverProgression].Invoke();

            retryFromGameOverProgression++;
            retryFromGameOverTimer.WaitTime = retryFromGameOverStepsTime[retryFromGameOverProgression];

            retryFromGameOverTimer.Start();

        }


        public void GoToTitleScreenFromPause()
        {


            canGameOver = false;

            if (GetTree().Paused) TogglePause();
            SetMouseTo(true);
            loadingScreen.CloseTo(0);
            hud.Disable();

            canPause = false;


            SoundManager.GetInstance().ResumeUI();

            SoundManager.GetInstance().level.Stop();
            SoundManager.GetInstance().boss.Stop();


            Timer lTimerToTitle = new Timer();
            lTimerToTitle.WaitTime = 2f;
            lTimerToTitle.OneShot = true;
            AddChild(lTimerToTitle);
            lTimerToTitle.Start();

            lTimerToTitle.Timeout += titleScreen.Enable;
            lTimerToTitle.Timeout += GameManager.GetInstance().QueueFree;
            lTimerToTitle.Timeout += lTimerToTitle.QueueFree;
            canGameOver = true;
        }
        public void GoToTitleScreenFromGameOver()
        {
            gameOver.Disable();
            canGameOver = false;

            SetMouseTo(true);
            hud.Hide();



            titleScreen.Enable();
            GameManager.GetInstance().QueueFree();
            canGameOver = true;
        }



        public void MakeColorBlind()
        {
            Main.GetInstance().SetShaderColor(ColorProp.COLORBLIND_COLOR);
        }

        public void CorrectColorBlind()
        {
            Main.GetInstance().SetShaderColor(ColorProp.CORRECTEDCOLORBLIND_COLOR);
        }

        public void UseNormalVision()
        {
            Main.GetInstance().SetShaderColor(ColorProp.NORMAL_COLOR);
        }
    
    }
}
