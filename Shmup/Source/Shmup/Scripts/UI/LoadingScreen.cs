using Godot;
using System;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class LoadingScreen : ColorRect
	{
        private ShaderMaterial backgroundShader;
        [Export] private float mouseOffsetSpeed = 1;


        private Tween loadingscreenTween;
        private float loadingscreenTweenProgression = 0;
        private float loadingscreenTweenSpeed = 1f;


        public bool isFollowingMouse = true;

        private double intensity = 0;

        public override void _Ready()
		{
            backgroundShader = (ShaderMaterial)Material;


        }


        
        public override void _Process(double pDelta)
        {
            float lDelta = (float)pDelta;

            backgroundShader.SetShaderParameter("Intensity", intensity);


            Size = Vector2.One * Mathf.Max(GetViewport().GetVisibleRect().Size.X, GetViewport().GetVisibleRect().Size.Y);

            Vector2 lTargetPosition = 
                (
                    (GetViewport().GetMousePosition() - GetViewport().GetVisibleRect().Size / 2) * mouseOffsetSpeed * (isFollowingMouse ? 1 : 0)
                    + GetViewport().GetVisibleRect().Size / 2
                )
                / Mathf.Max(GetViewport().GetVisibleRect().Size.X, GetViewport().GetVisibleRect().Size.Y);

            Vector2 lCurrentPosition = (Vector2)backgroundShader.GetShaderParameter(ShaderProp.RAINBOWCIRCLE_CENTER);

            backgroundShader.SetShaderParameter(
                ShaderProp.RAINBOWCIRCLE_CENTER,
                lCurrentPosition.Lerp(lTargetPosition,0.05f)
            );

            backgroundShader.SetShaderParameter(ShaderProp.RAINBOWCIRCLE_CLOSE,loadingscreenTweenProgression);

        }


        public void CloseTo(float pCloseProgression, float pStart = -1)
        {
            if (pStart < 0f) pStart = loadingscreenTweenProgression;
          

            loadingscreenTween?.Kill();

            loadingscreenTween = CreateTween()
                .SetTrans(Tween.TransitionType.Linear)
                .SetEase(Tween.EaseType.In);

            loadingscreenTween.TweenProperty(this, "loadingscreenTweenProgression", pCloseProgression, Mathf.Abs(pStart-pCloseProgression)).From(pStart);
        }


        public void SetProgression(float pProgression)
        {
            loadingscreenTweenProgression =  Math.Clamp(0,1,pProgression);
        }

        public void SetIntensity(double pIntensity)
        {
            intensity = pIntensity;
        }

        public void DecreaseIntensity()
        {
            Tween lTween = CreateTween()
                .SetTrans(Tween.TransitionType.Circ)
                .SetEase(Tween.EaseType.Out);

            lTween.TweenProperty(this, "intensity",0,10);
        }
	}
}
