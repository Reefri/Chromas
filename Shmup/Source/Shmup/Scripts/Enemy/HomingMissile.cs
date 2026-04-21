using Godot;
using System;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class HomingMissile : EnemyBullet
	{

        [Export] protected bool doFlashShader = true;

        private ShaderMaterial flashShader = new ShaderMaterial();

        private Timer flashTimer = new Timer();
        private float flashDuration = 0.01f;

        private Timer inBetweenFlashTimer = new Timer();
        private float inBetweenFlashDuration = 1f;

        private Vector2 baseSpeed;

        private float slowingDown = 0.9f;
        private float timeProgression = 0;

        private float rotationSpeed = MathF.Tau * 0.4f ;


        private float slowingFlashingTime = 0.7f;


        private Timer aliveTimer = new Timer();
        private float aliveDuration = 2f;


        private float explosionScale = 1;
        private float explosionDuration = 0.3f;

        private Timer blindTimer = new Timer();
        private float blindDuration = 0.2f;
        private bool rightOrLeftToggle = false;



		public override void _Ready()
		{
            damage = 1;

            base._Ready();

            baseSpeed = speed;

            if (doFlashShader)
            {

                rendererNode.Material = flashShader;

                flashShader.Shader = (Shader)GD.Load("res://Resources/Shaders/FlashDamage.tres");

                flashShader.SetShaderParameter("isActive", false);

                foreach (Node2D lRendererElement in rendererNode.GetChildren())
                {
                    lRendererElement.UseParentMaterial = true;
                }



                flashTimer.WaitTime = flashDuration;
                flashTimer.Autostart = true;
                flashTimer.OneShot = true;

                flashTimer.Timeout += StopFlash;
                AddChild(flashTimer);


                inBetweenFlashTimer.WaitTime = inBetweenFlashDuration;
                inBetweenFlashTimer.Autostart = false;
                inBetweenFlashTimer.OneShot = true;

                inBetweenFlashTimer.Timeout += DoFlash;
                AddChild(inBetweenFlashTimer);

            }


            aliveTimer.WaitTime = aliveDuration;
            aliveTimer.Autostart = true;
            aliveTimer.OneShot = true;

            aliveTimer.Timeout += Destroy;
            AddChild(aliveTimer);



            blindTimer.WaitTime = blindDuration;
            blindTimer.Autostart = true;
            blindTimer.OneShot = false;

            blindTimer.Timeout += () => rightOrLeftToggle = !rightOrLeftToggle;
            AddChild(blindTimer);

            Player.GetInstance().OnCamouflage += StartBlindTimer;

            if (Player.GetInstance().isCamouflage) {
                blindTimer.Start();
            }

        }

		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

			base._Process(lDelta);



            if (!Player.GetInstance().isCamouflage)
            {
                direction = direction.Rotated(
                    Mathf.Sign(direction.AngleTo(ToLocal(Player.GetInstance().GlobalPosition).Rotated(Rotation))) 
                     * rotationSpeed * lDelta
                    );
            }
            else
            {
                direction = direction.Rotated(rotationSpeed * lDelta * (rightOrLeftToggle?-1:1));
            }

            timeProgression += lDelta;

            speed = baseSpeed * MathF.Pow(slowingDown,timeProgression);

            Rotation = direction.Angle();

        }


        protected override void DestroyWhenOutsideScreen()
        {
			return;
        }


        private void StopFlash()
        {
            flashShader.SetShaderParameter("isActive", false);
            inBetweenFlashTimer.WaitTime *= slowingFlashingTime;


            inBetweenFlashTimer.Start();
        }

        protected void DoFlash()
        {

            Color lColor = ColorManager.ColorSetRelativeSaturation(ColorManager.GetOpposite(color), -0.5f);

            flashShader.SetShaderParameter("Color", new Vector3(lColor.R, lColor.G, lColor.B));
            flashShader.SetShaderParameter("isActive", true);

            flashTimer.Start();

            
        }

        public override void Destroy()
        {
            Player.GetInstance().OnCamouflage -= StartBlindTimer;
            blindTimer.Stop();

            Explosion.Create(GlobalPosition, color, explosionScale, explosionDuration);

            base.Destroy();
        }

        private void StartBlindTimer()
        {
            blindTimer.Start();
        }


    }
}
