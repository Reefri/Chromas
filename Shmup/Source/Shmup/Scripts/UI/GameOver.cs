using Godot;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class GameOver : Control
	{
		[Export] private Label gameOverLabel;

		[Export] private Control labelContainer;

		[Export] private Label scoreLabel;

		[Export] private Button retryButton;
		[Export] private Button quitButton;

		[Export] Node2D partsContainer;

		Timer timer = new Timer();
		float timerDuration = 2f;

		int visualScore = 0;


		public override void _Ready()
		{


            quitButton.Pressed += ((UIManager)GetParent()).GoToTitleScreenFromGameOver;
			retryButton.Pressed += ((UIManager)GetParent()).StartRetryFromGameOver;

			foreach (Label lLabel in labelContainer.GetChildren())
				lLabel.Visible = false; 

			Hide();

			timer.WaitTime = timerDuration;
			timer.Autostart = false;
			timer.OneShot = true;

			timer.Timeout += OnScreenClosed;

            AddChild(timer);


        }


        public override void _Process(double delta)
        {
            base._Process(delta);
			scoreLabel.Text = "Score : " + visualScore;
        }


		public void Enable()
		{
			if (!UIManager.GetInstance().canGameOver) { return; }

			SoundManager.GetInstance().StopLevel();

			if (!Player.GetInstance().isAlive)
			{
				UIManager.GetInstance().loadingScreen.SetIntensity(0.05d);
				SoundManager.GetInstance().gameOverJingle.Play();
			}
			else
			{
                SoundManager.GetInstance().winJingle.Play();



			}

            UIManager.GetInstance().canPause = false;
			UIManager.GetInstance().canGameOver = false;
			//UIManager.GetInstance().hud.Disable();
			((Label)labelContainer.GetChild(0)).Visible = Player.GetInstance().isAlive; 
			((Label)labelContainer.GetChild(1)).Visible = !Player.GetInstance().isAlive; 
			Show();

			UIManager.GetInstance().loadingScreen.CloseTo(0);

			timer.Start();


			UIManager.GetInstance().SetMouseTo(true);


			gameOverLabel.Visible = false;
			labelContainer.Visible = false;
            scoreLabel.Visible = false;
            retryButton.Visible = false;
			quitButton.Visible = false;
		}

		public void Disable()
		{
			UIManager.GetInstance().loadingScreen.DecreaseIntensity();
			UIManager.GetInstance().canGameOver = true;
			UIManager.GetInstance().canPause = true;
			Hide();
		}

		private void OnScreenClosed()
		{
			gameOverLabel.PivotOffset = gameOverLabel.Size / 2;
            labelContainer.PivotOffset = labelContainer.Size / 2;
            scoreLabel.PivotOffset = scoreLabel.Size / 2;
            quitButton.PivotOffset = quitButton.Size / 2;
            retryButton.PivotOffset = retryButton.Size / 2;
			


			Tween tween = CreateTween()
				.SetTrans(Tween.TransitionType.Expo)
				.SetEase(Tween.EaseType.Out);

			gameOverLabel.Visible = true;

			tween.TweenProperty(gameOverLabel,"scale",Vector2.One,1).From(Vector2.One*5);




            tween.TweenCallback(
               Callable.From(() =>
               {
                   labelContainer.Visible = true;

               }
           ));

            tween.TweenProperty(labelContainer,"scale",Vector2.One,1).From(Vector2.One*5);




          



            tween.TweenCallback(
               Callable.From(() =>
               {
				   quitButton.Visible = true ;
				   retryButton.Visible = true ;
               }
           ));

            tween.TweenProperty(quitButton,"scale",Vector2.One,1).From(Vector2.One*5);
			tween.Parallel().TweenProperty(retryButton,"scale",Vector2.One,1).From(Vector2.One*5);


            tween.Chain().TweenCallback(
             Callable.From(() =>
             {
                 scoreLabel.Visible = true;

             }
         ));

            tween.TweenProperty(scoreLabel, "scale", Vector2.One, 1).From(Vector2.One * 5);

            tween 
            .SetTrans(Tween.TransitionType.Expo)
            .SetEase(Tween.EaseType.Out);


            tween.TweenProperty(this, "visualScore", Player.GetInstance().score, 3).From(0);

            tween.Chain().TweenCallback(
			   Callable.From(() =>
			   {
				   if (Player.GetInstance().isAlive)
				   {
					   foreach (GpuParticles2D lParts in partsContainer.GetChildren())
					   {
                           lParts.Emitting = true;
					   }
				   }
               }
			));
        }

    }
}
