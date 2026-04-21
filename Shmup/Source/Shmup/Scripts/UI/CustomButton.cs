using Godot;
using System;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class CustomButton : MyBaseButton
	{


        private Tween buttonScaleTween;
        private Tween buttonRotationTween;

        private float buttonScale = 2f;
        private float buttonScaleSpeed = 0.5f;

        private float buttonRotation = MathF.Tau / 10;
        private float buttonRotationSpeed = 3f;

        
        public override void _Ready()
		{
            base._Ready();
            MouseEntered += EnableHoverButton;
            MouseExited += DisableHoverButton;

            PivotOffset = Size / 2;

        }



        public void EnableHoverButton()
        {

            buttonScaleTween = CreateTween()
                .SetTrans(Tween.TransitionType.Elastic)
                .SetEase(Tween.EaseType.Out);

            buttonRotationTween = CreateTween()
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.InOut)
                .SetLoops();

            buttonScaleTween.TweenProperty(this, TweenProp.SCALE, Vector2.One * buttonScale, buttonScaleSpeed);

            buttonRotationTween.TweenProperty(this, TweenProp.ROTATION, buttonRotation, buttonRotationSpeed);
            buttonRotationTween.TweenProperty(this, TweenProp.ROTATION, -buttonRotation, buttonRotationSpeed);

        }

        public void DisableHoverButton()
        {

            buttonRotationTween.Kill();
            buttonScaleTween.Kill();

            Scale = Vector2.One;
            Rotation = 0;

        }
    }
}
