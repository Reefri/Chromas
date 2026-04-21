using Godot;
using System.Collections.Generic;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class TitleScreen : Control
	{

		[Export] private Label labelLeft;
		[Export] private Label labelRight;
		[Export] private Control bossIcon;
		[Export] private CustomButton startButton;
		[Export] private CustomButton creditsButton;




		private float margin = 400;
        private float baseRotation = Mathf.Tau / 4;



        private List<Vector2> positions;

        private Tween titleTween;




        public override void _Ready()
		{
			base._Ready();

  

            startButton.Pressed += ((UIManager)GetParent()).StartGame;

			creditsButton.Pressed += UIManager.GetInstance().creditsScreen.Enable;


            positions = new List<Vector2>();

            positions.Add(labelLeft.Position);
            positions.Add(labelRight.Position);
            positions.Add(bossIcon.Position);
            positions.Add(startButton.Position);
            positions.Add(creditsButton.Position);


            Enable();


        }


		public void Enable()
		{
			UIManager.GetInstance().canPause = false;
			SoundManager.GetInstance().ResumeUI();


            startButton.Hide();
			creditsButton.Hide();
			bossIcon.Hide();
			labelRight.Hide();

			titleTween?.Kill();

            ResetPositions();


            Show();

			titleTween = CreateTween()
				.SetTrans(Tween.TransitionType.Elastic)
				.SetEase(Tween.EaseType.Out);

            titleTween.TweenProperty(labelLeft, TweenProp.POSITION, labelLeft.Position, 0.5f).From(labelLeft.Position+ Vector2.Left * margin);

            titleTween.TweenCallback(
			Callable.From(() =>
			{
				labelRight.Show();
			}
			));

            titleTween.TweenProperty(labelRight, TweenProp.POSITION, labelRight.Position, 0.5f).From(labelRight.Position + Vector2.Right * margin);


			
			titleTween
				.SetTrans(Tween.TransitionType.Expo)
				.SetEase(Tween.EaseType.Out)
				//.Chain()
				;

            titleTween.TweenCallback(
                Callable.From(() =>
                {
                    bossIcon.Show();
                }
                ));

            titleTween.TweenProperty(bossIcon,TweenProp.POSITION,bossIcon.Position,1f).From(bossIcon.Position + Vector2.Up * margin*0.5f);



            titleTween.TweenCallback(
            Callable.From(() =>
            {
                startButton.Show();
                creditsButton.Show();
            }
            ));


            titleTween 
				.SetTrans(Tween.TransitionType.Elastic)
				.SetEase(Tween.EaseType.Out)
				.SetParallel()
				;

            titleTween.TweenProperty(startButton, TweenProp.SCALE, startButton.Scale, 1f).From(Vector2.Zero);
            titleTween.Parallel().TweenProperty(creditsButton, TweenProp.SCALE, creditsButton.Scale, 1f).From(Vector2.Zero);


            titleTween
				.SetTrans(Tween.TransitionType.Elastic)
				.SetEase(Tween.EaseType.Out)
				;


            titleTween.Parallel().TweenProperty(startButton, TweenProp.ROTATION, startButton.Rotation, 1f).From(baseRotation);
            titleTween.Parallel().TweenProperty(creditsButton, TweenProp.ROTATION, creditsButton.Rotation, 1f).From(-baseRotation);

         



		}

		public void Disable()
		{
			titleTween = CreateTween()
				.SetTrans(Tween.TransitionType.Back)
				.SetEase(Tween.EaseType.In)
				.SetParallel()
				;

			titleTween.TweenProperty(labelLeft, TweenProp.POSITION, labelLeft.Position + Vector2.Right * margin, 1);
			titleTween.TweenProperty(labelRight, TweenProp.POSITION, labelRight.Position + Vector2.Right * margin, 1);
			titleTween.TweenProperty(bossIcon, TweenProp.POSITION, bossIcon.Position + Vector2.Right * margin, 1);
			titleTween.TweenProperty(startButton, TweenProp.POSITION, startButton.Position + Vector2.Right * margin, 1);
			titleTween.TweenProperty(creditsButton, TweenProp.POSITION, creditsButton.Position + Vector2.Right * margin, 1);
            
			titleTween.TweenCallback(


           Callable.From(() =>
           {
               Hide();
			   ResetPositions();
           }
           ));


		}

		private void ResetPositions()
		{

            labelLeft.Position = positions[0];
            labelRight.Position = positions[1];
            bossIcon.Position = positions[2];
            startButton.Position = positions[3];
            creditsButton.Position = positions[4];
        }
	}
}
