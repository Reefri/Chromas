using Godot;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class SpamControl : Node2D
	{

		[Export] Node2D top;
		[Export] Node2D bottom;


		private float margin = 700;
		public override void _Ready()
		{

			if (!Main.GetInstance().isFirstTimePlaying)
			{
				QueueFree();
			}

			ShowSpamControl();

        }

		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

			Scale = CameraManager.GetInstance().GetCamera().Scale;

		}



		public void ShowSpamControl()
		{

			Tween lShowTween = CreateTween()
				.SetTrans(Tween.TransitionType.Elastic)
				.SetEase(Tween.EaseType.Out)
				.SetParallel();
				;

			lShowTween.TweenProperty(top, TweenProp.POSITION, top.Position, 1f).From(top.Position + Vector2.Up * margin);


            lShowTween.TweenProperty(bottom, TweenProp.POSITION, bottom.Position, 1f).From(bottom.Position + Vector2.Down * margin);
        }

		public void HideSpamControl()
		{
			Tween lHideTween = CreateTween()
				.SetTrans(Tween.TransitionType.Back)
				.SetEase(Tween.EaseType.In);

            lHideTween.TweenProperty(bottom, TweenProp.POSITION, Vector2.Down * margin, 1f).AsRelative().SetDelay(1f);

            lHideTween.TweenProperty(top, TweenProp.POSITION, Vector2.Up * margin, 1f).AsRelative();


		}


	
	}
}
