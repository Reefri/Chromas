using Com.IsartDigital.Utils.Effects;
using Godot;
using System;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class HUD : Control
	{

		
		[Export] private TextureProgressBar healthBar;
		[Export] private TextureProgressBar followBar;


		[Export] private Node2D partsContainer;


		private Tween followBarTween;
		private Tween showTween;

        [Export] private Label scoreLabel;

		private Tween scoreLabelTween;
		private float scoreLabelScaleTweenDurationIn = 0.3f;
		private float scoreLabelScaleTweenDurationOut = 0.3f;
		private float tweenBaseRotation = MathF.Tau / 8;
        private float scoreLabelRotationTweenDuration = 0.3f;

		float maxScoreLabelScale = 2.5f;

        private const string SCORELABEL_SEPARATOR = "SCORE : ";

		[Export] private Control smartBombContainer;


        private Tween smartBombTween;

        private float margin = 100;


		[Export] public Label godLabel;

		private float timeProgression = 0;
		private float timeProgressionSpeed = 0.2f;

		private Tween scoreSaturationTween;
		private float scoreSaturationTweenDuration = 3;
		private float currentScoreSaturation = 0;


        private Tween bossFollowBarTween;


		[Export] Control bossBar;

        [Export] TextureProgressBar bossHealthBar;
		[Export] TextureProgressBar bossFollowBar;
		[Export] TextureProgressBar bossAfterBar;
		[Export] TextureRect locker;
		[Export] Shaker lockerShaker;
		[Export] Shaker bossBarShaker;


		[Export] GpuParticles2D lockerParticules;
		private int maxPartsNumber = 4;




		Timer glitchTimer = new Timer();
		float glitchTimerDuration = 1f;

		private float maxAfterBarValue = -1000;
		public override void _Ready()
		{
			Hide();

			scoreLabel.PivotOffset = scoreLabel.Size / 2;


            scoreLabel.Text = SCORELABEL_SEPARATOR + 0;

			foreach(TextureRect lTextureRect in smartBombContainer.GetChildren())
			{
				lTextureRect.PivotOffset = Vector2.One * lTextureRect.Size.Y /2;
			}

			godLabel.Visible = false;

			bossBar.Visible = false;


            bossAfterBar.MinValue = -5000;
            bossAfterBar.MaxValue = -500;

			glitchTimer.WaitTime = glitchTimerDuration;
			glitchTimer.Autostart = false;
			glitchTimer.OneShot = true;

			AddChild(glitchTimer);
		}

		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

			timeProgression+= lDelta*timeProgressionSpeed;
			timeProgression %= 1;

			scoreLabel.Modulate = Color.FromHsv(timeProgression, currentScoreSaturation, 1);

			//if (glitchTimer.TimeLeft>0)
			//{
			//	//GD.Print("yo");
			//}
			//else
			//{

   //         }
        }


		public void Enable()
		{
			healthBar.MaxValue = Player.GetInstance().maxHp;
			followBar.MaxValue = Player.GetInstance().maxHp;
			bossHealthBar.MaxValue = BossManager.GetInstance().maxHp;
			bossFollowBar.MaxValue = BossManager.GetInstance().maxHp;


			ChangeColor(Player.GetInstance().color);



            UpdateHealthBar();
			UpdateBossHealthBar();

			Show();

			showTween = CreateTween()
				.SetTrans(Tween.TransitionType.Expo)
				.SetEase(Tween.EaseType.Out)
				.SetParallel()
				;

			showTween.TweenProperty(healthBar, TweenProp.POSITION, healthBar.Position, 1).From(healthBar.Position + Vector2.Left * margin);
			showTween.TweenProperty(followBar, TweenProp.POSITION, followBar.Position, 1).From(followBar.Position + Vector2.Left * margin);
			showTween.TweenProperty(scoreLabel, TweenProp.POSITION, scoreLabel.Position, 1).From(scoreLabel.Position + Vector2.Up * margin);

		}

		public void Disable()
		{
			showTween?.Kill();
			Hide();
		}




		public void UpdateHealthBar()
		{
			healthBar.Value = Player.GetInstance().hp;


			followBarTween?.Kill();
			followBarTween = CreateTween()
				.SetTrans(Tween.TransitionType.Expo)
				.SetEase(Tween.EaseType.In)
				;

			followBar.TintProgress = ColorManager.ColorSetHue(followBar.TintProgress, Player.GetInstance().color.H);

			followBarTween.TweenProperty(followBar, TweenProp.VALUE, Player.GetInstance().hp, 1);

		}



		public void ChangeColor(Color color)
		{
			healthBar.TintOver = color;
		}



		public void UpdateScore(int pNewScore)
		{
			scoreLabelTween?.Kill();

            scoreLabel.Text = SCORELABEL_SEPARATOR + pNewScore;

			scoreLabelTween = CreateTween()
				.SetTrans(Tween.TransitionType.Expo)
				.SetEase(Tween.EaseType.Out)
				.SetParallel(true)
				;

			scoreLabelTween.TweenProperty(scoreLabel, TweenProp.SCALE, Vector2.One *((maxScoreLabelScale - Mathf.Abs(scoreLabel.Scale.X))/maxScoreLabelScale) * 0.4f, scoreLabelScaleTweenDurationIn).AsRelative();

			scoreLabelTween = CreateTween()
				.SetTrans(Tween.TransitionType.Elastic)
				.SetEase(Tween.EaseType.Out);


            scoreLabelTween.TweenProperty(scoreLabel, TweenProp.ROTATION, 0, scoreLabelRotationTweenDuration).From(tweenBaseRotation);

			scoreLabelTween = CreateTween()
				.SetTrans(Tween.TransitionType.Elastic)
				.SetEase(Tween.EaseType.Out)
				.Chain()
				;

            scoreLabelTween.TweenProperty(scoreLabel, TweenProp.SCALE, Vector2.One, scoreLabelScaleTweenDurationIn);


			currentScoreSaturation = 1;

			scoreSaturationTween = CreateTween()
				.SetTrans(Tween.TransitionType.Linear)
				.SetEase(Tween.EaseType.Out);

			scoreSaturationTween.TweenProperty(this, "currentScoreSaturation", 0, scoreSaturationTweenDuration);
        }


		public void Restart()
		{
			foreach(TextureRect lSmartBomb in smartBombContainer.GetChildren())
			{
				lSmartBomb.Scale = Vector2.One * 1.5f;

			}

            foreach (GpuParticles2D lParticule in partsContainer.GetChildren())
            {
				lParticule.Emitting = false;
				lParticule.Visible = false;

            }

            godLabel.Visible = false;

			bossBar.Visible = false;




			Main.GetInstance().chromaAberation.amplitudeMult = 0.05f;
			Main.GetInstance().chromaAberation.duration = 0.1f;
		}


		public void RemoveSmartBomb(int pSmartBombCount)
		{

			smartBombTween = CreateTween()
				.SetTrans(Tween.TransitionType.Back)
				.SetEase(Tween.EaseType.In)
				;

			smartBombTween.TweenProperty(smartBombContainer.GetChild(pSmartBombCount), TweenProp.SCALE, Vector2.Zero, 1);


		}

		public void AddSmartBomb(int pSmartBombCount)
		{

			smartBombTween = CreateTween()
				.SetTrans(Tween.TransitionType.Back)
				.SetEase(Tween.EaseType.Out)
				;

			smartBombTween.TweenProperty(smartBombContainer.GetChild(pSmartBombCount), TweenProp.SCALE, Vector2.One * 1.5f , 1);

		}


        public void UpdateBossHealthBar()
        {
            bossHealthBar.Value = BossManager.GetInstance().hp;




            bossFollowBarTween?.Kill();
            bossFollowBarTween = CreateTween()
                .SetTrans(Tween.TransitionType.Expo)
                .SetEase(Tween.EaseType.In)
                ;


            bossFollowBarTween.TweenProperty(bossFollowBar, TweenProp.VALUE, BossManager.GetInstance().hp, 0.1f);


			if (BossManager.GetInstance().hp < 0 && BossManager.GetInstance().hp>maxAfterBarValue)
			{
				lockerShaker.amplitudeMult = 0.7f+ 1f * (BossManager.GetInstance().hp) / maxAfterBarValue;

				lockerShaker.Start();

				if (!((GpuParticles2D)partsContainer.GetChild(0)).Emitting)
				{
                    foreach (GpuParticles2D lParticule in partsContainer.GetChildren())
                    {
						lParticule.Emitting = true;
                        lParticule.Visible = true;
                    }
                }

				foreach (GpuParticles2D lParticule in partsContainer.GetChildren())
				{
					lParticule.Amount = 1+ (int) ((BossManager.GetInstance().hp) /maxAfterBarValue * maxPartsNumber);

                }

            }
            else if (BossManager.GetInstance().hp <= maxAfterBarValue && locker.Visible)
			{
                foreach (GpuParticles2D lParticule in partsContainer.GetChildren())
                {
                    lParticule.Emitting = false;
                }
                lockerShaker.Stop();

                GameManager.GetInstance().AddPermentMarkControl(locker);


                locker.Visible = false;
				lockerParticules.Emitting = true;
				bossBarShaker.Start();

				Main.GetInstance().chromaAberation.Start();
				Main.GetInstance().glitchEffet.Start();

				glitchTimer.Start();
			}
			else if (BossManager.GetInstance().hp <= maxAfterBarValue)
			{
                Player.GetInstance().ScorePoints(BossA.GetInstance().scoreOnHit);

                bossAfterBar.Value = bossAfterBar.MinValue - BossManager.GetInstance().hp ;
                bossBarShaker.Start();


            }


        }



		public void EnableBossBar()
		{
			bossBar.Visible = true;
			locker.Visible= true;

			bossAfterBar.Value = bossAfterBar.MaxValue;
			bossFollowBar.Value = bossFollowBar.MaxValue;
            bossAfterBar.Value = bossAfterBar.MinValue;


		}

    }
}
