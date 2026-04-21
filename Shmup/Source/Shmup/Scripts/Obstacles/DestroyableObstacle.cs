using Godot;
// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation
{
	
	public partial class DestroyableObstacle : Obstacle
	{

        private bool isVisible = false;
        private float marginBeforeActivating = 10;

        public override void _Ready()
		{
			base._Ready();

            hp = 300;

            colliderNode.AreaEntered -= OnAreaEntered;
            colliderNode.AreaExited -= OnAreaExited;
        }

		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

			base._Process(lDelta);

            if (!isVisible && CameraManager.GetInstance().GetLevelXBondaries().Y > GlobalPosition.X + marginBeforeActivating)
            {
              
                colliderNode.AreaEntered += OnAreaEntered;
                colliderNode.AreaExited += OnAreaExited;
                isVisible = true;

            }


        }


        public override void Destroy()
        {
            foreach ( Node2D lRenderedElement in rendererNode.GetChildren()) GameManager.GetInstance().AddPermentMark(lRenderedElement,color);

            SoundManager.GetInstance().PlayObstacleDestruction();

			base.Destroy();
        }


        public override void OnCollisionWithBullet(Bullet pCollidedColoredEntity)
        {
			GetDamaged(pCollidedColoredEntity.damage);
            base.OnCollisionWithBullet(pCollidedColoredEntity);
        }
	}
}