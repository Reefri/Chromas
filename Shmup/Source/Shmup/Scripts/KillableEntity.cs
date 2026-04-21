using Godot;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation
{
	
	public partial class KillableEntity : ColoredEntity
	{

        [Export] protected bool doFlashShader = true;

        private ShaderMaterial flashShader = new ShaderMaterial();

        private Timer flashTimer = new Timer();
        private float flashDuration = 0.1f;

        private bool isDead = false;

        public float maxHp = 100;
        public float hp = 100;
        public override void _Ready()
		{
            base._Ready();

            hp = maxHp;

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
                flashTimer.Autostart = false;
                flashTimer.OneShot = true;

                flashTimer.Timeout += StopFlash;
                AddChild(flashTimer);
            }



        }
        

        public virtual void GetDamaged(float pDamage)
        {
            DoFlash();

            hp -= pDamage;


            if (hp<=0)
            {
                Destroy();
            }
          

        }

        

        protected void DoFlash()
        {
            if (!doFlashShader) return;
            Color lColor = ColorManager.ColorSetRelativeSaturation(ColorManager.GetOpposite(color),-0.5f);

            flashShader.SetShaderParameter("Color", lColor );
            flashShader.SetShaderParameter("isActive", true);

            flashTimer.Start();
        }

        private void StopFlash()
        {
            flashShader.SetShaderParameter("isActive", false);
        }


        public override void Destroy()
        {
            LevelManager.GetInstance().onKillShaker.Start();
            base.Destroy();
        }

    }
}
