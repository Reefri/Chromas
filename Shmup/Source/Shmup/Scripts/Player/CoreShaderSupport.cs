using Godot;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class CoreShaderSupport : ColorRect
	{
		private const string CORE_SHADER_PATH = "res://Resources/Shaders/CoreShader.tres";
		private Shader shader = (Shader)GD.Load(CORE_SHADER_PATH);

        private ShaderMaterial myShader = new ShaderMaterial();


        private float shaderSpeed = 1f;

		private float progression = 0;

        private Tween shaderProgressionTween;
		public override void _Ready()
		{
			ZIndex = -1;

            Material = myShader;

            myShader.Shader = shader;

            shaderProgressionTween = CreateTween()
				.SetTrans(Tween.TransitionType.Expo)
				.SetEase(Tween.EaseType.Out);

			shaderProgressionTween.TweenProperty(this, "progression", 1, shaderSpeed);
		}

		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

			(myShader).SetShaderParameter(ShaderProp.CORE_PROGRESSION, progression);
		}

		protected override void Dispose(bool pDisposing)
		{

		}
	}
}
