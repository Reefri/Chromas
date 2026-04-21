using Godot;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class SmartBomb : Explosion
	{

		private ShaderMaterial explosionShader;

		public float damage = 100; 

		public bool isReversed = false;
		public override void _Ready()
		{
			maxScale = 60;


			base._Ready();

            explosionShader = (ShaderMaterial)rendererNode.Material;

			explosionShader.SetShaderParameter("Duration", explosionDuration);

			LevelManager.GetInstance().smartBombShaker.Start();

        }

        public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

			base._Process(lDelta);

			explosionShader.SetShaderParameter("ExplosionProgression", isReversed? explosionTimer.TimeLeft :(explosionDuration - explosionTimer.TimeLeft) );

		}

		protected override void Dispose(bool pDisposing)
		{

		}

        protected override void ExplosionTween()
        {
            
        }

        protected override void OnAreaEntered(Area2D pArea)
        {

            ColoredEntity lCollidedColoredEntity = (ColoredEntity)pArea.GetParent();

            if (lCollidedColoredEntity is DestroyableObstacle )
            {
			
                ((DestroyableObstacle)lCollidedColoredEntity).Destroy();
				return;
            }

			//if (lCollidedColoredEntity is Bullet)
			//{
			//	lCollidedColoredEntity.Destroy();
			//	return;
			//}



        }
	}
}
