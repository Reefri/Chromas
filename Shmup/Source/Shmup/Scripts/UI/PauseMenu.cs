using Godot;


// Author : 

namespace Com.IsartDigital.Chromaberation {
	
	public partial class PauseMenu : Control
	{

		private float margin = 100;

		[Export] private ColorRect top;
		[Export] private ColorRect bottom;

		[Export] private Label pauseLabel;
		[Export] private Button quitButton;
		[Export] private Button resumeButton;
		//[Export] private Button retryButton;

		[Export] private TextureRect pauseIcon;

        private Tween rectsTween;

        private Tween iconTweenScale;
        private Tween iconTweenRotation;
		private float baseRotation = Mathf.Tau / 4;


		private ShaderMaterial gradientShaderTop;
		private ShaderMaterial gradientShaderBottom;

		private Tween gradientRespirationTween;

		private float currentGradientRespiration = 0.5f;
        private float minGradientRespiration = 0.55f;
        private float maxGradientRespiration = 0.15f;
		private float gradientRespirationTime = 2f;

		private float gradientSaturation = 0.9f;
		private float gradientValue = 0.8f;

        public override void _Ready()
		{
			base._Ready();

			currentGradientRespiration = minGradientRespiration;

			gradientRespirationTween = CreateTween()
				.SetTrans(Tween.TransitionType.Sine)
				.SetEase(Tween.EaseType.InOut)
				.SetLoops();

			gradientRespirationTween.TweenProperty(this, ShaderProp.GRADIENT_CURRENTRESPIRATION, maxGradientRespiration, gradientRespirationTime);
			gradientRespirationTween.TweenProperty(this, ShaderProp.GRADIENT_CURRENTRESPIRATION, minGradientRespiration, gradientRespirationTime);


            gradientShaderTop = (ShaderMaterial)top.Material;
            gradientShaderBottom = (ShaderMaterial)bottom.Material;
            gradientShaderBottom.SetShaderParameter(ShaderProp.GRADIENT_TOGGLEUPDOWN, true);


            pauseIcon.PivotOffset = pauseIcon.Size / 2;


            resumeButton.Pressed += ((UIManager)GetParent()).TogglePause;


			quitButton.Pressed += ((UIManager)GetParent()).GoToTitleScreenFromPause;
			//retryButton.Pressed += ((UIManager)GetParent()).StartRetryFromPause;


        }

        public override void _Process(double delta)
        {
            gradientShaderTop.SetShaderParameter(ShaderProp.GRADIENT_MARGIN, currentGradientRespiration);
            gradientShaderBottom.SetShaderParameter(ShaderProp.GRADIENT_MARGIN, currentGradientRespiration);
        }



        public void Enable()
		{

			SoundManager.GetInstance().StopLevel();

            SoundManager.GetInstance().ResumeUI();
			SoundManager.GetInstance().uiPause.Play();

            Show();

			UIManager.GetInstance().canPause = false;


            resumeButton.Visible = false;
			quitButton.Visible = false;
			//retryButton.Visible = false;

			if (Player.GetInstance().IsInsideTree())
			{
				Color lPlayerColor = Player.GetInstance().color;

                top.Color = Color.FromHsv(lPlayerColor.H,lPlayerColor.S * gradientSaturation ,lPlayerColor.V * gradientValue);
				bottom.Color = top.Color;
			}


            rectsTween = CreateTween()
				.SetTrans(Tween.TransitionType.Expo)
				.SetEase(Tween.EaseType.Out);


            rectsTween.TweenProperty(top, TweenProp.POSITION, top.Position, 1f).From(new Vector2(0,-top.Size.Y-margin));
            rectsTween.Parallel().TweenProperty(bottom, TweenProp.POSITION, bottom.Position, 1f).From(new Vector2(0, bottom.Size.Y + margin + bottom.Position.Y));


			iconTweenScale = CreateTween()
				.SetTrans(Tween.TransitionType.Elastic)
				.SetEase(Tween.EaseType.Out)
				;


            iconTweenRotation = CreateTween()
				.SetTrans(Tween.TransitionType.Elastic)
				.SetEase(Tween.EaseType.Out)
				;

            iconTweenScale.TweenProperty(pauseIcon, TweenProp.SCALE, pauseIcon.Scale, 1f).From(Vector2.Zero);

            iconTweenRotation.TweenProperty(pauseIcon, TweenProp.ROTATION, pauseIcon.Rotation, 1f).From(baseRotation);



            iconTweenScale.TweenCallback(
				Callable.From( () =>
				{
					resumeButton.Visible = true;
					quitButton.Visible = true;
					//retryButton.Visible = true;
				}
				));


			iconTweenScale.TweenProperty(resumeButton, TweenProp.SCALE, resumeButton.Scale, 1f).From(Vector2.Zero);
			iconTweenRotation.TweenProperty(resumeButton, TweenProp.ROTATION, resumeButton.Rotation, 1f).From(baseRotation);

			iconTweenScale.Parallel().TweenProperty(quitButton, TweenProp.SCALE, quitButton.Scale, 1f).From(Vector2.Zero);


            //iconTweenRotation.Parallel().TweenProperty(retryButton, TweenProp.ROTATION, retryButton.Rotation, 1f).From(baseRotation);
            //iconTweenRotation.Parallel().TweenProperty(retryButton, TweenProp.ROTATION, retryButton.Rotation, 1f).From(-baseRotation);




            iconTweenScale.TweenCallback(
                Callable.From(() =>
                {
					UIManager.GetInstance().canPause = true ;
                }
                ));


        }

        public void Disable()
		{
			SoundManager.GetInstance().ResumeLevel();
			Hide();
        }


    }
}
