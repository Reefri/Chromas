using Godot;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class Bullet : ColoredEntity
	{

		float marginBeforeDestroy = 50f;	



		public bool isDead = false;

		public float damage = 7;

        public override void _Ready()
		{
			base._Ready();

			direction = new Vector2(1, 0);


			speed = Vector2.Right * 2000;

		}

		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;


			if (Position.X > CameraManager.GetInstance().GetCameraXBondaries().Y + marginBeforeDestroy) { Destroy(); }

			base._Process(lDelta);

		}

		protected override void Dispose(bool pDisposing)
		{
			isDead = true;
		}

        protected override void OnAreaEntered(Area2D pArea)
        {
            ColoredEntity lCollidedColoredEntity = (ColoredEntity)pArea.GetParent();


            if (lCollidedColoredEntity is EnemyBullet && ! (lCollidedColoredEntity is BossBullet))
			{
				lCollidedColoredEntity.Destroy();
                OnCollision(lCollidedColoredEntity);

                Destroy();
				return;
            }



           

            if (lCollidedColoredEntity is Enemy)
            {
                ((Enemy)lCollidedColoredEntity).makeExplosion = ColorManager.AreEnemyColorsCloseEnough(color, ColorManager.GetOpposite(lCollidedColoredEntity.color));

                ((Enemy)lCollidedColoredEntity).GetDamaged(damage);
				OnCollision(lCollidedColoredEntity);

                Destroy();

                return;
            }
        }

		public void OnCollision(Node2D pNode)
		{
			//myParticule.GlobalPosition = Vector2.Right * pNode.Position -
			//	Vector2.Right *
			//	((Sprite2D)((ColoredEntity)pNode).rendererNode.GetChild(0)).Texture.GetSize().X *
			//	((Node2D)((ColoredEntity)pNode).rendererNode.GetChild(0)).Scale.X *
			//	((ColoredEntity)pNode).rendererNode.Scale.X *
			//	((ColoredEntity)pNode).Scale.X


			//	+ Vector2.Down * GlobalPosition.Y;

			//;

		}



	}
}
