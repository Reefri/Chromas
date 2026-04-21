using Godot;

// Author : Gramatikoff Sacha
namespace Com.IsartDigital.Chromaberation
{
	
	public partial class EnemyBullet : ColoredEntity
	{
		private const string ENEMYBULLET_SCENE_PATH = "res://Scenes/ColoredEntity/Enemy/EnemyBullet.tscn";
		private static PackedScene enemyBulletFactory = (PackedScene)GD.Load(ENEMYBULLET_SCENE_PATH);

        private Tween spawnScaleTween;

        private float marginBeforeDestruction = 100;

        private float rotationSpeed = Mathf.Tau;


        public float damage = 5;

		public override void _Ready()
		{
			base._Ready();

            rotationSpeed /= (GD.Randf() - 0.5f) * 2;


			speed = Vector2.One * 1000;

			Rotation = direction.Angle();

			Appear();
		}
        
		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

			base._Process(lDelta);

			DestroyWhenOutsideScreen();

            Rotate(lDelta*rotationSpeed);

        }

        protected override void UpdateColor()
        {
            base.UpdateColor();

            if (ColorManager.AreColorsCloseEnough(Player.GetInstance().color, color))
            {
                MakeSemiTransparent();
            }
        }

        private void MakeSemiTransparent()
        {
            Color lColor = color;
            lColor.A = 0.5f;
            rendererNode.Modulate = lColor;
        }

		public static EnemyBullet Create()
		{
            return (EnemyBullet)enemyBulletFactory.Instantiate();
        }

		protected virtual void DestroyWhenOutsideScreen()
		{
            if (GlobalPosition.X < CameraManager.GetInstance().GetCameraXBondaries().X - marginBeforeDestruction || GlobalPosition.X > CameraManager.GetInstance().GetCameraXBondaries().Y + marginBeforeDestruction ||
                GlobalPosition.Y < CameraManager.GetInstance().GetCameraYBondaries().X - marginBeforeDestruction || GlobalPosition.Y > CameraManager.GetInstance().GetCameraYBondaries().Y + marginBeforeDestruction)
            {
                Destroy();
            }
        }

        public void Appear()
        {
            Show();
            spawnScaleTween = CreateTween()
                .SetTrans(Tween.TransitionType.Elastic)
                .SetEase(Tween.EaseType.Out);

            spawnScaleTween.TweenProperty(this, "scale", Scale, 1).From(Vector2.Zero);

        }

        protected override void OnAreaEntered(Area2D pArea)
        {
            base.OnAreaEntered(pArea);


            ColoredEntity lCollidedColoredEntity = (ColoredEntity)pArea.GetParent();

          
            if ((lCollidedColoredEntity is Player) && !Player.GetInstance().isCamouflage && ! ColorManager.AreColorsCloseEnough(lCollidedColoredEntity.color, color))
            {
                Player.GetInstance().GetDamaged(damage);
                Destroy();
                return;
            }

            if (lCollidedColoredEntity is SmartBomb)
            {
                Destroy();
            }
        }
    }
}
