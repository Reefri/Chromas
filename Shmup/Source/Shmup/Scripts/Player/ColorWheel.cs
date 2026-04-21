using Com.IsartDigital.Utils.Effects;
using Godot;


// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class ColorWheel : Node2D
	{

		[Export] private PlayerCore playerCore;

		private float currentRotation = 0;

        private Tween rotationTween;
		private float rotationTweenTime = 0.8f;

        private Timer rotationTimer = new Timer();
		private float rotationTimerTime = 0.2f;

        private Tween visibilityTween;
        private Tween invisibilityTween;
		private float visibilityTweenTime = 0.1f;
		private float invisibilityTweenTime = 0.5f;
		 
		public float radius = 100;

		private int colorNumber;



		private ColorRect colorRect;
		private const string SHADERSUPPORT_NODE_PATH = "ColorRect";
		private float wheelRotation = 0;

		private ShaderMaterial wheelShader;

        public override void _Ready()
		{
			colorRect = ((ColorRect)GetNode(SHADERSUPPORT_NODE_PATH));
			wheelShader = (ShaderMaterial)colorRect.Material;

			colorNumber = GameManager.GetInstance().colorNumber;



			rotationTimer.Timeout += RotateColor;
			rotationTimer.WaitTime = rotationTimerTime;
			AddChild(rotationTimer);



			currentRotation = 1f/(colorNumber*2);
            wheelRotation = currentRotation;
        }
		

        public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

			wheelShader.SetShaderParameter(ShaderProp.COLORWHEEL_ROTATION,wheelRotation);


		}




		public void StartRotating()
		{
      

			RotateColor();

			rotationTimer.Start();
		}

		public void StopRotating()
		{
            rotationTimer.Stop();

		}

		private void RotateColor()
		{


			currentRotation += 1f / colorNumber ;


            rotationTween = CreateTween();

            rotationTween.SetTrans(Tween.TransitionType.Elastic);
            rotationTween.SetEase(Tween.EaseType.Out);


            rotationTween.TweenProperty(this, "wheelRotation", currentRotation, rotationTweenTime);


			Player.GetInstance().colorIndex += 1;
            Player.GetInstance().colorIndex %= GameManager.GetInstance().colorNumber;
			Player.GetInstance().SetColorToIndex();

			playerCore.CreateShaderSupport(Player.GetInstance().color);
			Player.GetInstance().ShouldDoCamouflage();


			SoundManager.GetInstance().PlayColorSwap();

        }
    }
}
