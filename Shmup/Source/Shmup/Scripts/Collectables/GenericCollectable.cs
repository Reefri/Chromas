using Godot;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class GenericCollectable : ColoredEntity
	{

		Vector2 baseScale;
		public override void _Ready()
		{
			base._Ready();

			baseScale = Scale;

			color = new Color(1,1,1);

			JuiceManager.GetInstance().OnCollectableBeat += OnBeat;
		}
		
		public override void _Process(double pDelta)
		{
			base._Process(pDelta);
			Rotation = Mathf.Sin(JuiceManager.GetInstance().globalTime * Mathf.Tau / 2 / 1.5f) * Mathf.Tau /8;
		}

		protected virtual void DoOnCollected()
		{
			Destroy();
		}

        protected override void OnAreaEntered(Area2D pArea)
        {
            base.OnAreaEntered(pArea);

            ColoredEntity lCollidedColoredEntity = (ColoredEntity)pArea.GetParent();


            if (lCollidedColoredEntity is Player)
			{

                DoOnCollected();
			}
        }

		private void OnBeat()
		{
			Tween lTween = CreateTween()
				.SetTrans(Tween.TransitionType.Expo)
				.SetEase(Tween.EaseType.Out);

			lTween.TweenProperty(this, "scale", Vector2.One * 1.6f * baseScale, 0.15f);
			lTween.SetEase(Tween.EaseType.In);
			lTween.TweenProperty(this, "scale", Vector2.One * baseScale , 0.05f);
		}

        public override void Destroy()
        {
            base.Destroy();
            JuiceManager.GetInstance().OnCollectableBeat -= OnBeat;

        }
    }
}
