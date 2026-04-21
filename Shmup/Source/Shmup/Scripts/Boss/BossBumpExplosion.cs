using Godot;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation {
	
	public partial class BossBumpExplosion : Explosion
	{


        private const string BOSSBUMPEXPLOSION_SCENE_PATH = "res://Scenes/BossBumpExplosion.tscn";
        static private PackedScene bossBumpExplosionFactory = (PackedScene)GD.Load(BOSSBUMPEXPLOSION_SCENE_PATH);


        protected float bossBumpExplosionMaxScale = 10;


        protected float bossBUmpExplosionDuration = 5f;

		private float damage = 10;



        public override void _Ready()
		{
			maxScale = bossBumpExplosionMaxScale;

			explosionDuration = bossBUmpExplosionDuration;

			base._Ready();

		}

		public static BossBumpExplosion Create()
		{
			return (BossBumpExplosion)bossBumpExplosionFactory.Instantiate();

        }

        protected override void OnAreaEntered(Area2D pArea)
        {
            ColoredEntity lCollidedColoredEntity = (ColoredEntity)pArea.GetParent();



            if ((lCollidedColoredEntity is Player) && !Player.GetInstance().isCamouflage && ! ColorManager.AreColorsCloseEnough(lCollidedColoredEntity.color, color))
            {
                Player.GetInstance().GetDamaged(damage);

                colliderNode.AreaEntered -= OnAreaEntered;

                return;
            }
           


        }

        protected override void UpdateColor()
        {
            base.UpdateColor();


            if (ColorManager.AreColorsCloseEnough(Player.GetInstance().color, color))
            {
                Modulate = new Color(1, 1, 1, 0.5f);
            }
            else
            {
                Modulate = new Color(1, 1, 1, 1);
            }
        }

        private void MakeSemiTransparent()
        {
           
        }

    }
}
