using Godot;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation
{

	public partial class PassableObstacle : Obstacle
	{
        private float transparency = 0.5f;

		[Export] private ColorRect visibleRect;

        //private float marging = 20;

        [Export] private GpuParticles2D enableParticles;
        [Export] private GpuParticles2D disableParticles;

        private bool isActive = true;


        public override void _Ready()
		{
			base._Ready();
			knockBackFullX = true;
		}

		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;
			base._Process(lDelta);

            //particles.Emitting = (GlobalPosition.X + marging < CameraManager.GetInstance().GetCameraXBondaries().Y ||
            //                      GlobalPosition.X - marging > CameraManager.GetInstance().GetCameraXBondaries().X);





        }


        protected override void OnAreaEntered(Area2D pArea)
		{

			ColoredEntity lCollidedColoredEntity = (ColoredEntity)pArea.GetParent();


            if (!ColorManager.AreColorsCloseEnough(Player.GetInstance().color, color))
            {
                base.OnAreaEntered(pArea);
			}


		}


        protected override void UpdateColor()
        {
            base.UpdateColor();

            //GD.Print(color, " : " , Player.GetInstance().color ," : ", ColorManager.AreColorsCloseEnough(Player.GetInstance().color, color));

            if (ColorManager.AreColorsCloseEnough(Player.GetInstance().color, color))
            {
                MakeSemiTransparent();
                if (!isActive)
                {
                    isActive = true;
                    disableParticles.Emitting = true;
                    enableParticles.Visible = false;

                }
            }
            else
            {
                isActive = false;
                enableParticles.Visible = true;
                disableParticles.Emitting = false;

            }
        }

        private void MakeSemiTransparent()
        {
            Color lColor = color;
            lColor.A = 0.5f;
            rendererNode.Modulate = lColor;
        }


    }
}
