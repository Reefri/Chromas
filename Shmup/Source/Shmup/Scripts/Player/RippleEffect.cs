using Godot;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class RippleEffect : ColorRect
	{
		//[Export] Vector2 position;

		private Tween radiusTween;
		private float radiusProgression=0;
		private const float maxRadius = 0.2f;
		private float duration = 2;
		private float intensity = 30;
		private float edge = 0.08f;


		private ShaderMaterial shaderMaterial = new ShaderMaterial();
		private const string RIPPLEEFFECT_SHADER_PATH = "res://Resources/Shaders/RippleEffect.tres";

        private Shader rippleEffect = (Shader)GD.Load(RIPPLEEFFECT_SHADER_PATH);



		public Vector2 centerPosition;


        private float camouflageShaderTimeIn = 1f;
        private float camouflageShaderValueIn = 0.4f;


        public override void _Ready()
		{

			SetAnchorsPreset(LayoutPreset.FullRect);
			this.Material = shaderMaterial;
			shaderMaterial.Shader = rippleEffect;

			shaderMaterial.SetShaderParameter(ShaderProp.RIPPLE_INTENSITY,intensity);
			shaderMaterial.SetShaderParameter(ShaderProp.RIPPLE_MAXRADIUS, maxRadius);
			shaderMaterial.SetShaderParameter(ShaderProp.RIPPLE_CENTERPOSITION, centerPosition);
			shaderMaterial.SetShaderParameter(ShaderProp.RIPPLE_EDGE, edge);

			radiusTween = CreateTween()
				.SetTrans(Tween.TransitionType.Expo)
				.SetEase(Tween.EaseType.Out);


			radiusTween.TweenProperty(this, "radiusProgression", camouflageShaderValueIn,camouflageShaderTimeIn).From(0);

		}

		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

			shaderMaterial.SetShaderParameter(ShaderProp.RIPPLE_RADIUS,radiusProgression);
            shaderMaterial.SetShaderParameter(ShaderProp.RIPPLE_CENTERPOSITION, Player.GetInstance().GetGlobalTransformWithCanvas().Origin / GetViewportRect().Size);


			
        }



		public void StopCamouflage()
		{
			radiusTween?.Kill();


            radiusTween = CreateTween()
                .SetTrans(Tween.TransitionType.Expo)
                .SetEase(Tween.EaseType.Out);


            radiusTween.TweenProperty(this, "radiusProgression", 1, duration - camouflageShaderTimeIn);

			radiusTween.Finished += GetParent().QueueFree;
        }
	}
}
