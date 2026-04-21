using Godot;
using System;
using System.Collections.Generic;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class GlitchShake : ColorRect
	{


        


        [ExportGroup("General")]

        [Export] public float duration = 2f;
        [Export] private Vector2 amplitude = Vector2.One * 5f;
        [Export] public float amplitudeMult = 1;
        [Export(PropertyHint.Range, "0,1,or_greater")] private float step = 0.048f;
        [Export(PropertyHint.Range, "0,30,radians_as_degrees")] private float noise = 15f;

        [ExportGroup("Attack")]

        [Export] private Tween.TransitionType transition_ = Tween.TransitionType.Sine;
        [Export] private Tween.EaseType ease_ = Tween.EaseType.InOut;
        [Export] private float duration_ = 0.25f;

        [ExportGroup("Release")]

        [Export] private Tween.TransitionType _transition = Tween.TransitionType.Sine;
        [Export] private Tween.EaseType _ease = Tween.EaseType.InOut;
        [Export] private float _duration = 0.25f;

        private Vector2 currentGlitchPosition;
        private Vector2 next;

        public float amplitudeMax;
        private Vector2 currentAmplitude;

        private Tween shake;
        private Tween loop;
        private float intensity;

        public RandomNumberGenerator random = new RandomNumberGenerator();


        private Vector2 glitchPosition = Vector2.Zero;


        private ShaderMaterial material;

        public override void _Ready()
        {
            base._Ready();
            material = (ShaderMaterial)Material;
            amplitude = amplitude.Abs() * amplitudeMult;


        }

        public override void _Process(double delta)
        {

            material.SetShaderParameter("position", glitchPosition);
            


        }
        public void Start()
        {

            amplitudeMax = Mathf.Max(amplitude.X, amplitude.Y);

            currentGlitchPosition = Vector2.FromAngle(Mathf.Pi * 2 * random.Randf()) * amplitudeMax;
            Stop();

            material.SetShaderParameter("accuracy", 0.2f);


            intensity = 0f;

            shake = CreateTween();

            shake.TweenProperty(this, nameof(intensity), 1, duration_).SetTrans(transition_).SetEase(ease_);
            shake.TweenInterval(duration);
            shake.TweenProperty(this, nameof(intensity), 0, _duration).SetTrans(_transition).SetEase(_ease);
            shake.Finished += Stop;

            Loop();

        }

        public void Stop()
        {


            glitchPosition = Vector2.Zero;
            material.SetShaderParameter("accuracy", 7);

            loop?.Kill();
            shake?.Kill();
            shake = null;


        }

        public bool isPlaying()
        {
            return shake != null;
        }

        public void Loop()
        {

            next = -Vector2.FromAngle(currentGlitchPosition.Angle() + random.RandfRange(-noise, noise)) * amplitudeMax;

            // correction de l'angle pour une amplitude non homogène
            if (amplitude.X < amplitudeMax && Mathf.Abs(next.X) > amplitude.X)
            {
                next.X = Mathf.Sign(next.X) * amplitude.X;
                next.Y = Mathf.Sign(next.Y) * Mathf.Sqrt(amplitudeMax * amplitudeMax - next.X * next.X);
            }
            else if (amplitude.Y < amplitudeMax && Mathf.Abs(next.Y) > amplitude.Y)
            {
                next.Y = Mathf.Sign(next.Y) * amplitude.Y;
                next.X = Mathf.Sign(next.X) * Mathf.Sqrt(amplitudeMax * amplitudeMax - next.Y * next.Y);
            }

            loop = CreateTween().SetParallel();

            loop.TweenProperty(this, "glitchPosition",  next * intensity, step);

            currentGlitchPosition = next;



            loop.Finished += Loop;
        }

    }
}

