using Godot;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation
{
	
	public partial class Obstacle : KillableEntity
	{

		public float collisionDamage = 10;
		protected float knockBackStrength = -4f;
		protected bool knockBackFullX = false;
		public override void _Ready()
		{
			colorGivenTroughtCode = false;
			base._Ready();
		}

        protected override void OnAreaEntered(Area2D pArea)
        {

            ColoredEntity lCollidedColoredEntity = (ColoredEntity)pArea.GetParent();

            if (lCollidedColoredEntity is Player)
            {
				OnCollisionWithPlayer((Player)lCollidedColoredEntity);

            }

            if (lCollidedColoredEntity is Bullet)
            {
                OnCollisionWithBullet((Bullet)lCollidedColoredEntity);
                ((Bullet)lCollidedColoredEntity).OnCollision(this);

            }


        }

		public virtual void OnCollisionWithBullet(Bullet pCollidedColoredEntity)
		{
            pCollidedColoredEntity.Destroy();

        }

		protected virtual void OnCollisionWithPlayer(Player pPlayer)
		{
            pPlayer.GetDamaged(collisionDamage);
            pPlayer.KnockBack(knockBackStrength, this, knockBackFullX);
        }

    }
}
